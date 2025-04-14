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
    public delegate void MinigameEnterEvent(MinigameController minigameController, PlayerController ownerPlayer, PlayerController interactingPlayer);
    public static MinigameEnterEvent minigameEnterEvent;
    public delegate void MinigameLeftEvent(PlayerController interactingPlayer);
    public static MinigameLeftEvent minigameLeftEvent;
    
    //public bool IsInMinigame { get; private set; } = false;
    
    [SerializeField] private PvPMinigameController m_PVPMinigameInstance;
    [SerializeField] private List<WorkstationController> workStations;

    private MinigameController _player1Minigame;
    private MinigameController _player2Minigame;

    private void Start()
    {
        Vector3 position = Vector3.zero;
        foreach (var workStation in workStations)
        {
            position += Vector3.right * MINIGAME_OFFSET;
            workStation.CreateMinigameInstance(position, transform);
        }
    }

    //public bool CanOpenMinigame(string minigameName, int playerAttempting)
    //{
        
    //    MinigameController minigameInstance = _minigamesDict[minigameName];
    //    int onPlayerSideHackyFucky=int.Parse(minigameName.Split("_")[1]);
    //    //Debug.Log($"Checking can open minigame for playerattempting {playerAttempting} and minigame name {minigameName}, with minigame on player side {minigameInstance.OnPlayerSide}, hacky fucky playerside {onPlayerSideHackyFucky} and isCompleted {minigameInstance.IsCompleted()}");
    //    if (minigameInstance.UsedByPlayer != 0)
    //        return false;
    //    //only allow for sabotage if its completed
    //    if (minigameInstance.IsCompleted())
    //        return playerAttempting != onPlayerSideHackyFucky;
        
    //    //only allow for completion if its not already completed
    //    return playerAttempting == onPlayerSideHackyFucky;
        
    //    //return true;
    //}

    public void LaunchPVPMinigame()
    {
        PlayerController.Player1.transform.position = m_PVPMinigameInstance.Player1Pos.position;
        PlayerController.Player2.transform.position = m_PVPMinigameInstance.Player2Pos.position;
        PlayerController.Player1.OnPVPMinigameEntered(m_PVPMinigameInstance);
        PlayerController.Player2.OnPVPMinigameEntered(m_PVPMinigameInstance);
        m_PVPMinigameInstance.Launch();
    }
}