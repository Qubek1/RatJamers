using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class MinigameController : MonoBehaviour
{
    public static event Action ResetAction;
    [SerializeField] private Transform m_CameraTarget;
    public Vector2 CameraTargetPosition => m_CameraTarget.position;
    protected virtual void Start()
    {
        Hide();
    }

    public abstract void Launch();

    public abstract void Hide();

    public void Reset()
    {
        ResetAction?.Invoke();
    }

    public abstract bool IsCompleted();
    
    public void MinigameLeft()
    {
        Debug.Log($"Minigame {gameObject.name} Correctly Finished!");
        MinigamesManager.MinigameLeftAction?.Invoke();
    }
}
