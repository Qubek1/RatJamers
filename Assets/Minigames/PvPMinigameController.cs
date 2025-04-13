using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class PvPMinigameController : MonoBehaviour
{
    public Transform Player1Pos;
    public Transform Player2Pos;

    public MinigameCameraConfig CameraConfig;
    public MinigameCameraConfig Player2CameraConfig;

    public abstract void Launch();
    public abstract bool IsCompleted();
    public abstract void Hide();
}
