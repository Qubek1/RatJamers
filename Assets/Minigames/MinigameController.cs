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

    [SerializeField] private SpriteRenderer m_FrameSprite;
    [SerializeField] private TextMeshProUGUI m_TimeLimitText;
    [SerializeField] private MinigameCameraConfig m_CameraConfig;
    public MinigameCameraConfig CameraConfig => m_CameraConfig;
    public PlayerController interactingPlayer;
    public WorkstationController workStation;

    //protected bool _isFinishedCorrectly;


    //public Vector2 CameraTargetPosition => m_CameraTarget.position;
    protected virtual void Start()
    {
        Hide();
    }

    public virtual void Launch(PlayerController interactingPlayer)
    {
        this.interactingPlayer = interactingPlayer;
        gameObject.SetActive(true);
        //if (launchingPlayer == 1)
        //{
        //    Vector3 newScale = m_FrameSprite.transform.localScale;
        //    newScale.x=-Mathf.Abs(newScale.x);
        //    m_FrameSprite.transform.localScale =newScale;
        //}
        //else
        //{
        //    Vector3 newScale = m_FrameSprite.transform.localScale;
        //    newScale.x=Mathf.Abs(newScale.x);
        //    m_FrameSprite.transform.localScale =newScale;
        //}
        //if (interactingPlayer != workStation.ownerPlayer)
        //{
        //    m_TimeLimitText?.gameObject.SetActive(true);
        //    StartCoroutine(SabotageTimeLimitCoroutine());
        //}
        //else
        //{
        //    m_TimeLimitText?.gameObject.SetActive(false);
        //}
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
        MinigameFinish(0);
    }

    public void Reset()
    {
        ResetAction?.Invoke();
    }

    public abstract void Hide();
    public abstract bool CanStartNegative();
    public abstract bool CanStartPositive();
    public abstract bool IsCompleted();

    public bool IsSabotage() => interactingPlayer != workStation.ownerPlayer;

    public void MinigameFinish(float productivityChange)
    {
        Debug.Log($"Minigame {gameObject.name} Correctly Finished!");
        Hide();
        interactingPlayer.HandleMinigameLeft();
        workStation.UpdateProductivity(productivityChange);
        interactingPlayer = null;
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