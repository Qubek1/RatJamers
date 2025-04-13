using System;
using UnityEngine;

public abstract class MinigameController : MonoBehaviour
{
    public static event Action ResetAction;
    //[SerializeField] public Transform CameraTarget;
    [SerializeField] private MinigameCameraConfig m_CameraConfig;
    public MinigameCameraConfig CameraConfig => m_CameraConfig;
    protected int UsedByPlayer;

    protected int OnPlayerSide;
    //public Vector2 CameraTargetPosition => m_CameraTarget.position;
    protected virtual void Start()
    {
        Hide();
    }

    public virtual void Launch(int launchingPlayer,int onPlayerSide)
    {
        UsedByPlayer = launchingPlayer;
        OnPlayerSide = onPlayerSide;
    }

    public abstract void Hide();

    public void Reset()
    {
        ResetAction?.Invoke();
    }

    protected bool IsSabotage()=>UsedByPlayer!=0&&UsedByPlayer!=OnPlayerSide;

    public abstract bool IsCompleted();
    
    public void MinigameLeft()
    {
        Debug.Log($"Minigame {gameObject.name} Correctly Finished!");
        MinigamesManager.MinigameLeftAction?.Invoke(UsedByPlayer);
        UsedByPlayer = 0;
        gameObject.SetActive(false);
    }
}

[Serializable]
public struct MinigameCameraConfig
{
    public Transform CameraTarget;
    public Vector3 CameraOffset;
    public float CameraSize;
}