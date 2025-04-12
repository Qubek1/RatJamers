using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class QATestMinigame : MonoBehaviour
{
    public int progressState = 0;

    [SerializeField]
    private Transform spawnPosition;
    [SerializeField]
    private QAPlayerController player;

    [SerializeField]
    private List<GameObject> levels;

    private void Awake()
    {
        player.onFinishEnter += OnFinishEnter;
        player.onHazardEnter += Respawn;
        Restart();
    }

    private void Restart()
    {
        Respawn();
    }

    private void Respawn()
    {
        player.transform.position = spawnPosition.position;
        player.ResetVelocity();
    }

    private void UpdateProgress()
    {
        foreach (GameObject g in levels)
        {
            g.SetActive(false);
        }
        levels[progressState].SetActive(true);
    }

    private void OnFinishEnter()
    {
        Debug.Log("Finish!");
        Respawn();
        progressState++;
        UpdateProgress();
    }
}

[Serializable]
public struct ListOfGameObjects
{
    public List<GameObject> list;
    public ListOfGameObjects(List<GameObject> list)
    {
        this.list = list;
    }
}