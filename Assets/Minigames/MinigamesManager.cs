using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class MinigamesManager : MonoSingleton<MinigamesManager>
{
    
    //Just fucking send it
    private const float MINIGAME_OFFSET = 100f;
    
    public static event Action<MinigameController> MinigameEnteredAction;
    public static Action MinigameLeftAction;
    
    //public bool IsInMinigame { get; private set; } = false;
    
    [SerializeField] private List<MinigameData> m_MinigameDatas;
    private Dictionary<string,MinigameController> _minigamesDict=new();

    private void Start()
    {
        foreach (var data in m_MinigameDatas)
        {
            MinigameController minigameInstance=CreateMinigameInstance(data.Prefab);
            _minigamesDict.Add(data.MinigameName, minigameInstance);
        }
    }

    private MinigameController CreateMinigameInstance(GameObject minigamePrefab)
    {
        MinigameController minigameInstance= 
            Instantiate(minigamePrefab).GetComponent<MinigameController>();
        
        minigameInstance.transform.position=new Vector3((_minigamesDict.Count+1)*MINIGAME_OFFSET,0,0);

        return minigameInstance;
    }

    public void LaunchMinigame(string minigameName)
    {
        
        if (_minigamesDict.TryGetValue(minigameName, out MinigameController instance))
        {
           //Debug.Log($"Launching {minigameName} minigame");
           instance.Launch();
           MinigameEnteredAction?.Invoke(instance);
        }
        else
        {
            Debug.LogWarning($"Attempted to launch {minigameName} that is not in the list!!!");
        }
    }
}


[Serializable]
public struct MinigameData
{
    public GameObject Prefab;
    public string MinigameName;
}
