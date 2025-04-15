using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class PVPMinigameController : MonoBehaviour
{
    [SerializeField]
    private Animator pvpWalkingToBossAnimation;
    [SerializeField]
    private string animationStateName;
    [SerializeField]
    private float animationsTime;
    [SerializeField]
    private float cameraSize;

    public PlayerController playerLeftController;
    public PlayerController playerRightController;

    public Transform Player1Pos;
    public Transform Player2Pos;

    public void StartPvPEvent(Camera pvpMinigameCamera)
    {
        pvpWalkingToBossAnimation.enabled = true;
        pvpWalkingToBossAnimation.Play(animationStateName);
        StartCoroutine(WaitForCutscene(pvpMinigameCamera));
    }

    private IEnumerator WaitForCutscene(Camera pvpMinigameCamera)
    {
        yield return new WaitForSeconds(animationsTime);
        pvpWalkingToBossAnimation.enabled = false;
        pvpMinigameCamera.orthographicSize = cameraSize;
        Launch(pvpMinigameCamera, playerLeftController, playerRightController);
    }

    protected abstract void Launch(Camera pvpMinigameCamera, PlayerController playerLeft, PlayerController playerRight);

    protected void PVPMinigameFinish(PlayerController winner, PlayerController losser)
    {
        GameManager.Instance.PVPMinigameFinished(winner, losser);
        Hide();
    }

    public abstract bool IsCompleted();
    public abstract void Hide();
}
