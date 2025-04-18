using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PVPMinigamesManager : MonoSingleton<PVPMinigamesManager>
{
    [SerializeField]
    private int pointForWin = 25;
    [SerializeField]
    private List<PVPMinigameController> pvpMinigames;
    [SerializeField]
    private List<Camera> playersCameras;
    [SerializeField]
    private PlayerController playerLeft;
    [SerializeField]
    private PlayerController playerRight;
    [SerializeField]
    private GameObject splitScreenUI;
    [SerializeField]
    private Camera pvpMinigameCamera;

    [Tooltip("In seconds since game start")]
    [SerializeField] private float m_StartPVPMinigameTime;

    [Tooltip("After waiting for both players to finish a minigame. Dont make it too long")]
    [SerializeField] private float m_PVPStartDelay=1f;

    [SerializeField]
    private Animator cutscenesAnimator;

    [SerializeField]
    private float _timeLeftUntilPVP;

    private PVPMinigameController currentMinigame;
    private float pvpCameraSize;

    private void Start()
    {
        _timeLeftUntilPVP = m_StartPVPMinigameTime;
        pvpCameraSize = pvpMinigameCamera.orthographicSize;
    }

    private void Update()
    {
        if (_timeLeftUntilPVP > 0)
        {
            _timeLeftUntilPVP -= Time.deltaTime;
        }
        else
        {
            // Trigger PVP minigame
            Debug.Log(PVPMinigameCoroutineRef);
            if (PVPMinigameCoroutineRef == null && pvpMinigames.Count > 0)
                PVPMinigameCoroutineRef = StartCoroutine(TriggerPVPMinigameCoroutine());
            _timeLeftUntilPVP = m_StartPVPMinigameTime;
        }
    }

    private Coroutine PVPMinigameCoroutineRef;
    private IEnumerator TriggerPVPMinigameCoroutine()
    {
        currentMinigame = pvpMinigames.First();
        pvpMinigames.RemoveAt(0);
        yield return new WaitUntil(() => !PlayerController.IsAnyInMinigame());
        yield return new WaitForSeconds(m_PVPStartDelay);

        cutscenesAnimator.enabled = true;
        yield return null;
        cutscenesAnimator.Play(currentMinigame.cutsceneAnimation.name);
        playerLeft.OnPVPMinigameEntered(currentMinigame);
        playerRight.OnPVPMinigameEntered(currentMinigame);
        foreach (Camera playerCamera in playersCameras)
        {
            playerCamera.gameObject.SetActive(false);
        }
        splitScreenUI.SetActive(false);
        pvpMinigameCamera.gameObject.SetActive(true);
        yield return new WaitForSeconds(currentMinigame.cutsceneAnimation.length + 0.1f);

        cutscenesAnimator.enabled = false;
        pvpMinigameCamera.transform.position = currentMinigame.transform.position;
        pvpMinigameCamera.orthographicSize = pvpCameraSize;
        currentMinigame.Launch(playerLeft, playerRight);
    }

    public void PVPMinigameFinished(PlayerController winner, PlayerController losser)
    {
        PVPMinigameCoroutineRef = null;
        winner.OnPVPMinigameEnd();
        losser.OnPVPMinigameEnd();
        winner.progressBar.AddPoints(pointForWin);
        splitScreenUI.SetActive(true);
        foreach (Camera playerCamera in playersCameras)
        {
            playerCamera.gameObject.SetActive(true);
        }
        pvpMinigameCamera.gameObject.SetActive(false);
    }
}
