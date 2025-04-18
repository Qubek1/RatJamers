 using System;
using UnityEngine;

public class WorkstationController : MonoBehaviour, IInteractable
{
    [SerializeField] private float pointsPerSeconds = 1;
    [SerializeField] private GameObject plusOnePrefab;
    [SerializeField] private Transform plusOneSpawnPoint;
    [SerializeField] private MinigameController minigamePrefab;
    [SerializeField] private PlayerController _ownerPlayer;
    public PlayerController ownerPlayer { get => _ownerPlayer; }

    public MinigameController minigametInstance;

    private ProductivityBar _productivityBar;
    private PlayerController _usedByPlayer;
    private float currentPointProgress = 0;

    private void Start()
    {
        _productivityBar = GetComponentInChildren<ProductivityBar>();
        if(_productivityBar==null)
            Debug.LogError("No productivity bar found in the workstation", this);
        currentPointProgress = pointsPerSeconds;
    }

    private void Update()
    {
        currentPointProgress -= _productivityBar.currentProductivity / _productivityBar.maxProductivity * Time.deltaTime;
        if (currentPointProgress < 0)
        {
            currentPointProgress += pointsPerSeconds;
            ownerPlayer.progressBar.AddPoints(1);
            //Instantiate(
            //    plusOnePrefab,
            //    plusOneSpawnPoint.position,
            //    Quaternion.identity,
            //    plusOneSpawnPoint
            //    );
        }
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
            if(player == _usedByPlayer)
                _usedByPlayer = null;
        }
    }

    public void CreateMinigameInstance(Vector3 position, Transform parent)
    {
        minigametInstance = Instantiate(minigamePrefab, position, Quaternion.identity, parent);
        minigametInstance.workStation = this;
    }

    public void UpdateProductivity(float value)
    {
        _productivityBar.UpdateCurrentProductivityOnMiniGameEnd(value);
    }
    
    public void Interact(PlayerController interactingPlayer)
    {
        _usedByPlayer = interactingPlayer;
        interactingPlayer.HandleMinigameEntered(minigametInstance);
        minigametInstance.Launch(interactingPlayer);
    }

    public bool IsInteractable(PlayerController interactingPlayer)
    {
        return (interactingPlayer == ownerPlayer && minigametInstance.CanStartPositive()) ||
            (interactingPlayer != ownerPlayer && minigametInstance.CanStartNegative());
    }
}
