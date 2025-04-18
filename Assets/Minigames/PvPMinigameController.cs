using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public abstract class PVPMinigameController : MonoBehaviour
{
    public AnimationClip cutsceneAnimation;
    public PlayerController playerLeftController;
    public PlayerController playerRightController;

    public abstract void Launch(PlayerController playerLeft, PlayerController playerRight);

    protected void PVPMinigameFinish(PlayerController winner, PlayerController losser)
    {
        PVPMinigamesManager.Instance.PVPMinigameFinished(winner, losser);
        Hide();
    }

    public abstract bool IsCompleted();
    public abstract void Hide();
}
