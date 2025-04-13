using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class MinigamesManager : MonoSingleton<MinigamesManager>
{
    
    //Just fucking send it
    private const float MINIGAME_OFFSET = 100f;
    
    /// <summary>
    /// Minigame entered, on side of player id, by player id
    /// </summary>
    public static event Action<MinigameController, int, int> MinigameEnteredAction;
    public static Action<int> MinigameLeftAction;
    
    //public bool IsInMinigame { get; private set; } = false;
    
    [SerializeField] private List<MinigameData> m_MinigameDatas;
    
    [SerializeField] private MashingPVPMinigameController m_PVPMinigameInstance;
    private Dictionary<string,MinigameController> _minigamesDict=new();
    
    private MinigameController _player1Minigame;
    private MinigameController _player2Minigame;

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
            Instantiate(minigamePrefab,
                new Vector3((_minigamesDict.Count+1)*MINIGAME_OFFSET,0,0),Quaternion.identity).GetComponent<MinigameController>();
        return minigameInstance;
    }

    public void LaunchMinigame(string minigameName, int onSideOf, int byPlayer, WorkstationController caller)
    {
        
        if (_minigamesDict.TryGetValue(minigameName, out MinigameController instance))
        {
           //Debug.Log($"Launching {minigameName} minigame");
           MinigameEnteredAction?.Invoke(instance, onSideOf, byPlayer);
           instance.Launch(byPlayer,onSideOf, caller);
        }
        else
        {
            Debug.LogWarning($"Attempted to launch {minigameName} that is not in the list!!!");
        }
    }

    public bool CanOpenMinigame(string minigameName, int playerAttempting)
    {
        
        MinigameController minigameInstance = _minigamesDict[minigameName];
        int onPlayerSideHackyFucky=int.Parse(minigameName.Split("_")[1]);
        //Debug.Log($"Checking can open minigame for playerattempting {playerAttempting} and minigame name {minigameName}, with minigame on player side {minigameInstance.OnPlayerSide}, hacky fucky playerside {onPlayerSideHackyFucky} and isCompleted {minigameInstance.IsCompleted()}");
        if (minigameInstance.UsedByPlayer != 0)
            return false;
        //only allow for sabotage if its completed
        if (minigameInstance.IsCompleted())
            return playerAttempting != onPlayerSideHackyFucky;
        
        //only allow for completion if its not already completed
        return playerAttempting == onPlayerSideHackyFucky;
        
        //return true;
    }

    public void LaunchPVPMinigame()
    {
        PlayerController.Player1.transform.position = m_PVPMinigameInstance.Player1Pos.position;
        PlayerController.Player2.transform.position = m_PVPMinigameInstance.Player2Pos.position;
        PlayerController.Player1.OnPVPMinigameEntered(m_PVPMinigameInstance);
        PlayerController.Player2.OnPVPMinigameEntered(m_PVPMinigameInstance);
        m_PVPMinigameInstance.LaunchGame();
    }
}


[Serializable]
public struct MinigameData
{
    public GameObject Prefab;
    public string MinigameName;
}
