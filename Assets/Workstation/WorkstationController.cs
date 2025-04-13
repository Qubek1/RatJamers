 using System;
using UnityEngine;

public class WorkstationController : MonoBehaviour, IInteractable
{

    [SerializeField] private string m_MinigameToLaunch;
    [SerializeField] private int m_Player;

    private ProductivityBar _productivityBar;

    private int _usedByPlayer;

    private void Start()
    {
        _productivityBar = GetComponentInChildren<ProductivityBar>();
        if(_productivityBar==null)
            Debug.LogError("No productivity bar found in the workstation", this);
    }

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

    public void UpdateProductivity(float value)
    {
        _productivityBar.UpdateCurrentProductivityOnMiniGameEnd(value);
    }
    
    public void Interact(int playerInteracting)
    {
        _usedByPlayer = playerInteracting;
        MinigamesManager.Instance.LaunchMinigame(m_MinigameToLaunch, m_Player,playerInteracting, this);
    }

    public bool IsInteractable(int playerInteracting)
    {
        return MinigamesManager.Instance.CanOpenMinigame(m_MinigameToLaunch, playerInteracting);
    }
}
