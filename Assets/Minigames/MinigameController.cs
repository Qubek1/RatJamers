using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public abstract class MinigameController : MonoBehaviour
{
    public static event Action ResetAction;
    //[SerializeField] public Transform CameraTarget;
    [SerializeField] private bool m_CanBeSabotaged;
    [SerializeField] private float m_SabotageTime = 5f;
    
    [SerializeField] private TextMeshProUGUI m_TimeLimitText;
    [SerializeField] private MinigameCameraConfig m_CameraConfig;
    public MinigameCameraConfig CameraConfig => m_CameraConfig;
    public int UsedByPlayer;

    public int OnPlayerSide;

    //protected bool _isFinishedCorrectly;

    private WorkstationController _callingWorker;
    //public Vector2 CameraTargetPosition => m_CameraTarget.position;
    protected virtual void Start()
    {
        Hide();
    }

    public virtual void Launch(int launchingPlayer,int onPlayerSide, WorkstationController caller)
    {
        gameObject.SetActive(true);
        UsedByPlayer = launchingPlayer;
        OnPlayerSide = onPlayerSide;
        _callingWorker = caller;
        if (IsSabotage())
        {
            m_TimeLimitText?.gameObject.SetActive(true);
            StartCoroutine(SabotageTimeLimitCoroutine());
        }
        else
        {
            m_TimeLimitText?.gameObject.SetActive(false);
        }
    }
    
    private IEnumerator SabotageTimeLimitCoroutine()
    {
        float timeLeft = m_SabotageTime;
        while (timeLeft > 0)
        {
            timeLeft-=Time.deltaTime;
            if(m_TimeLimitText!=null)
                m_TimeLimitText.text = timeLeft.ToString("0.00");
            yield return null;

        }
        //yield return new WaitForSeconds(SabotageTimeLimit);
        MinigameLeft();
    }

    public abstract void Hide();

    public void Reset()
    {
        ResetAction?.Invoke();
    }

    protected bool IsSabotage()
    {
        if (!m_CanBeSabotaged) return false;
        return UsedByPlayer!=0&&UsedByPlayer!=OnPlayerSide;
    }

    public abstract bool IsCompleted();
    
    public void MinigameLeft()
    {
        Debug.Log($"Minigame {gameObject.name} Correctly Finished!");
        MinigamesManager.MinigameLeftAction?.Invoke(UsedByPlayer);
        if (IsSabotage())
        {
            if(!IsCompleted())
                _callingWorker.UpdateProductivity(-100f);
        }
        else
        {
            _callingWorker.UpdateProductivity(50f);
        }
        
            
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