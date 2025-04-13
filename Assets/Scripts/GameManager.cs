using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoSingleton<GameManager>
{
    [Tooltip("In seconds since game start")]
    [SerializeField] private float m_StartPVPMinigameTime;

    private float _timeLeftUntilPVP;

    private void Start()
    {
        _timeLeftUntilPVP = m_StartPVPMinigameTime;
    }

    private void Update()
    {
        if (_timeLeftUntilPVP > 0)
        {
            _timeLeftUntilPVP -= Time.deltaTime;
        }
        else
        {
            // Trigger PVP minigame
            if (PVPMinigameCoroutineRef == null)
                PVPMinigameCoroutineRef = StartCoroutine(TriggerPVPMinigameCoroutine());
            _timeLeftUntilPVP = m_StartPVPMinigameTime;
        }
    }

    private Coroutine PVPMinigameCoroutineRef;
    private IEnumerator TriggerPVPMinigameCoroutine()
    {
        yield return new WaitUntil(() => !PlayerController.IsAnyInMinigame());
        
        // Start the PVP minigame here
        MinigamesManager.Instance.LaunchPVPMinigame();
    }

    public void PVPMinigameFinished()
    {
        PVPMinigameCoroutineRef = null;
        PlayerController.Player1.OnPVPMinigameEnd();
        PlayerController.Player2.OnPVPMinigameEnd();
    }
}
