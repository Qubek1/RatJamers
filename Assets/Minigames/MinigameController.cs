using System;
using UnityEngine;

public abstract class MinigameController : MonoBehaviour
{
    public static event Action ResetAction;
    [SerializeField] private Transform m_CameraTarget;

    private int _usedByPlayer;
    public Vector2 CameraTargetPosition => m_CameraTarget.position;
    protected virtual void Start()
    {
        Hide();
    }

    public virtual void Launch(int player)
    {
        _usedByPlayer = player;
    }

    public abstract void Hide();

    public void Reset()
    {
        ResetAction?.Invoke();
    }

    public abstract bool IsCompleted();
    
    public void MinigameLeft()
    {
        Debug.Log($"Minigame {gameObject.name} Correctly Finished!");
        MinigamesManager.MinigameLeftAction?.Invoke(_usedByPlayer);
        _usedByPlayer = 0;
    }
}
