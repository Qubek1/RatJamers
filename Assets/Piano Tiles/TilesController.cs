using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TilesController : MonoBehaviour
{
    [NonSerialized]
    public Action<int> missedTimeStampOnLane;
    [NonSerialized]
    public float maxError;

    [SerializeField]
    private bool removeDuplicates = true;
    [SerializeField]
    private float distancePerSecond = 5;
    [SerializeField]
    private float maxHeight = 5;
    [SerializeField]
    private float minHeight = -5;
    [SerializeField]
    private float snap = 0.1f;
    [SerializeField]
    private MusicController musicController;
    [SerializeField]
    private GameObject tilePrefab;
    [SerializeField]
    private List<Transform> lanes;
    [SerializeField]
    private List<Color> lanesColors;
    [SerializeField]
    private List<TimeStampOnLane> timeStampsOnLanes;

    private List<TimeStampOnLane> timeStampsNotYetClicked;
    [SerializeField]
    private List<Tile> tilesHidden = new List<Tile>();
    [SerializeField]
    private List<Tile> tilesInUse = new List<Tile>();

    private List<Vector3> tilesInUsePosition = new List<Vector3>();
    private List<TimeStampOnLane> timeStampsToTiles = new List<TimeStampOnLane>();

    //private float[] m_aduioSpectrum;
    //private float spectrumValue;

    // Start is called before the first frame update
    private void Start()
    {
        //m_aduioSpectrum = new float[128];
        Restart();
        if (removeDuplicates)
        {
            for (int i = timeStampsOnLanes.Count - 1; i >= 0; i--)
            {
                var duplicates = timeStampsOnLanes.Where((timeStamp) => 
                timeStamp.lane == timeStampsOnLanes[i].lane && SnappedTime(timeStamp.time) == SnappedTime(timeStampsOnLanes[i].time));
                if (duplicates.Count() > 1)
                {
                    timeStampsOnLanes.RemoveAt(i);
                }
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        //AudioListener.GetSpectrumData(m_aduioSpectrum, 0, FFTWindow.Hamming);
        //if (m_aduioSpectrum != null && m_aduioSpectrum.Length > 0)
        //{
        //    spectrumValue = m_aduioSpectrum[0] * 100;
        //}
        //v.position = Vector3.up * spectrumValue;
        for (int lane = 0; lane < lanes.Count; lane++)
        {
            for (int childIndex = 0; childIndex < lanes[lane].childCount; childIndex++)
            {
                Transform childTransform = lanes[lane].GetChild(childIndex);
                Tile tile;
                if (childTransform.gameObject.activeInHierarchy &&
                    childTransform.TryGetComponent(out tile) &&
                    !tilesInUse.Contains(tile) &&
                    tile.readyToUse)
                {
                    tile.time += snap;
                    tilesInUse.Add(tile);
                    tilesInUsePosition.Add(tile.transform.position);
                    timeStampsToTiles.Add(new TimeStampOnLane(tile.time, tile.lane));
                    timeStampsOnLanes.Add(timeStampsToTiles.Last());
                }
            }
        }

        for (int i = 0; i < tilesInUse.Count; i++)
        {
            if (!tilesInUse[i])
            {
                tilesInUse.RemoveAt(i);
                tilesInUsePosition.RemoveAt(i);
                timeStampsOnLanes.Remove(timeStampsToTiles[i]);
                timeStampsToTiles.RemoveAt(i);
                timeStampsNotYetClicked = new List<TimeStampOnLane>(timeStampsToTiles);
            }
            else if (tilesInUse[i].transform.position != tilesInUsePosition[i])
            {
                ModifyTimeStamp(tilesInUse[i], CalculateTileTimeFromPosition(tilesInUse[i]));
                timeStampsToTiles[i] = new TimeStampOnLane(tilesInUse[i].time, tilesInUse[i].lane);
            }
        }

        for (int i = tilesInUse.Count - 1; i >= 0; i--)
        {
            Tile tile = tilesInUse[i];
            float position = CalculateTilePosition(tile.time);
            if (position < minHeight || position > maxHeight || tilesInUse.Where((tileInUse) => tileInUse.time == tile.time).Count() == 0)
            {
                tile.transform.position += Vector3.up * maxHeight;
                tile.gameObject.SetActive(false);
                tilesHidden.Add(tile);
                tilesInUse.RemoveAt(i);
                tilesInUsePosition.RemoveAt(i);
                timeStampsToTiles.RemoveAt(i);
            }
            else
            {
                tile.transform.position = lanes[tile.lane].position + Vector3.up * position;
            }
        }
        foreach (TimeStampOnLane timeStampOnLane in timeStampsNotYetClicked)
        {
            float time = timeStampOnLane.time;
            int lane = timeStampOnLane.lane;
            float position = CalculateTilePosition(time);
            if (position >= minHeight && position <= maxHeight &&
                tilesInUse.Where((tile) => tile.time == time && tile.lane == lane).Count() == 0)
            {
                Tile tile = tilesHidden.FirstOrDefault((tile) => tile.readyToUse);
                if (tile)
                {
                    tile.gameObject.SetActive(true);
                    tile.Restart();
                    tile.transform.position = lanes[lane].position + Vector3.up * position;
                    tile.transform.SetParent(lanes[lane]);
                    tile.time = time;
                    tile.lane = lane;
                    tile.ChangeColor(lanesColors[lane]);
                    tilesInUse.Add(tile);
                    tilesInUsePosition.Add(tilesInUse.Last().transform.position);
                    timeStampsToTiles.Add(new TimeStampOnLane(time, lane));
                    tilesHidden.Remove(tile);
                }
                else
                {
                    tile = Instantiate(tilePrefab, lanes[lane]).GetComponent<Tile>();
                    tile.time = time;
                    tile.lane = lane;
                    tile.transform.position = lanes[lane].position + Vector3.up * position;
                    tile.ChangeColor(lanesColors[lane]);
                    tile.transform.SetParent(lanes[lane]);
                    tilesInUse.Add(tile);
                    tilesInUsePosition.Add(tilesInUse.Last().transform.position);
                    timeStampsToTiles.Add(new TimeStampOnLane(time, lane));
                }
            }
        }
        for (int i = timeStampsNotYetClicked.Count - 1; i >= 0; i--)
        {
            TimeStampOnLane timeStampToClick = timeStampsNotYetClicked[i];
            if (timeStampToClick.time < musicController.progressInSeconds && !CheckTime(timeStampToClick.time))
            {
                if (missedTimeStampOnLane != null)
                {
                    missedTimeStampOnLane.Invoke(timeStampToClick.lane);
                    timeStampsNotYetClicked.RemoveAt(i);
                }
            }
        }
        for (int i=0; i<tilesInUse.Count; i++)
        {
            tilesInUsePosition[i] = tilesInUse[i].transform.position;
        }
    }

    public void Restart()
    {
        timeStampsNotYetClicked = new List<TimeStampOnLane>(timeStampsOnLanes);
    }

    public bool ActionOnLane(int lane)
    {
        var possibleTimeStamps = timeStampsNotYetClicked.Where(
            (timeStamp) => timeStamp.lane == lane && CheckTime(timeStamp.time));
        if (possibleTimeStamps.Count() > 0)
        {
            Debug.Log("yup");
            TimeStampOnLane firstTimeStamp = possibleTimeStamps.OrderBy((timeStamp) => timeStamp.time).First();
            timeStampsNotYetClicked.Remove(firstTimeStamp);
            Tile tile = tilesInUse.Find((tile) => tile.time == firstTimeStamp.time && tile.lane == lane);
            //Debug.Log(tile);
            if (tile)
            {
                tile.Click();
                int tileIndex = tilesInUse.IndexOf(tile);
                tilesInUse.RemoveAt(tileIndex);
                tilesInUsePosition.RemoveAt(tileIndex);
                timeStampsToTiles.RemoveAt(tileIndex);
                tilesHidden.Add(tile);
            }
            return true;
        }
        else
        {
            Debug.Log("NOPE");
            return false;
        }
    }

    public void AddTimeStamp(int lane)
    {
        timeStampsOnLanes.Add(new TimeStampOnLane(musicController.progressInSeconds, lane));
        timeStampsNotYetClicked = new List<TimeStampOnLane>(timeStampsOnLanes);
    }

    public void RemoveTimeStamp(Tile tile)
    {
        timeStampsOnLanes.Remove(new TimeStampOnLane(tile.time, tile.lane));
        timeStampsNotYetClicked = new List<TimeStampOnLane>(timeStampsOnLanes);
    }

    public void ModifyTimeStamp(Tile tile, float newTime)
    {
        timeStampsOnLanes.Remove(new TimeStampOnLane(tile.time, tile.lane));
        timeStampsOnLanes.Add(new TimeStampOnLane(newTime, tile.lane));
        tile.time = newTime;
        timeStampsNotYetClicked = new List<TimeStampOnLane>(timeStampsOnLanes);
    }

    private bool CheckTime(float time)
    {
        float snappedTime = SnappedTime(time);
        return snappedTime <= musicController.progressInSeconds + maxError && snappedTime >= musicController.progressInSeconds - maxError;
    }

    private float SnappedTime(float time)
    {
        return Mathf.Round(time / snap) * snap;
    }

    private float CalculateTilePosition(float time)
    {
        return (SnappedTime(time) - musicController.progressInSeconds) * distancePerSecond;
    }

    private float CalculateTileTimeFromPosition(Tile tile)
    {
        return (tile.transform.position.y - lanes[tile.lane].position.y) / distancePerSecond + musicController.progressInSeconds;
    }
}

[Serializable]
public struct TimeStampOnLane
{
    public float time;
    public int lane;

    public TimeStampOnLane(float time, int lane)
    {
        this.time = time;
        this.lane = lane;
    }
}