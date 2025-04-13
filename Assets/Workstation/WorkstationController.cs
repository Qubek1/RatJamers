 using System;
using UnityEngine;

public class WorkstationController : MonoBehaviour, IInteractable
{

    [SerializeField] private string m_MinigameToLaunch;
    [SerializeField] private int m_Player;

    private int _usedByPlayer;
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.TryGetComponent(out PlayerInteractionComponent player))
        {
            player.RegisterInteractable(this);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.TryGetComponent(out PlayerInteractionComponent player))
        {
            player.DeRegisterInteractable(this);
            if(player.GetComponent<PlayerController>().GetPlayerNumber()==_usedByPlayer)
                _usedByPlayer = 0;
        }
    }
    
    public void Interact(int playerInteracting)
    {
        _usedByPlayer = playerInteracting;
        MinigamesManager.Instance.LaunchMinigame(m_MinigameToLaunch, m_Player,playerInteracting);
    }

    public bool IsInteractable()=>_usedByPlayer==0;
}
