using System;
using UnityEngine;

public class WorkstationController : MonoBehaviour, IInteractable
{

    [SerializeField] private string m_MinigameToLaunch;
    [SerializeField] private int m_Player;
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
        }
    }

    public void Interact(int player)
    {
        MinigamesManager.Instance.LaunchMinigame(m_MinigameToLaunch, player);
    }

    public bool IsInteractable()
    {
        return true;
    }
}
