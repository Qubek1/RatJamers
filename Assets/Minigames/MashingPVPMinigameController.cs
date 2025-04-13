using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class MashingPVPMinigameController : MinigameController
{

    public Transform Player1Pos;
    public Transform Player2Pos;
    
    [SerializeField] private int m_MashDifferenceToWin = 10;
    
    [SerializeField] private MinigameCameraConfig m_Player2CameraConfig;
    public MinigameCameraConfig Player2CameraConfig => m_Player2CameraConfig;
    [Header("Refs")]
    [SerializeField] private TextMeshProUGUI m_Player1MashCountText;
    [SerializeField] private TextMeshProUGUI m_Player2MashCountText;


    private int _player1MashCount;
    private int _player2MashCount;
    // Start is called before the first frame update
    protected override void Start()
    {
        //connect to events etc here
        
        
        base.Start();
    }

    public override void Launch(int launchingPlayer, int onPlayerSide)
    {
        base.Launch(launchingPlayer, onPlayerSide);
    }
    
    void Update()
    {
        m_Player1MashCountText.text = _player1MashCount.ToString();
        m_Player2MashCountText.text = _player2MashCount.ToString();
        if(!IsCompleted()) return;
        
        GameManager.Instance.PVPMinigameFinished();
        Hide();
    }
    

    public void LaunchGame()
    {
        _player1MashCount = 0;
        _player2MashCount = 0;
        gameObject.SetActive(true);
        
        PlayerController.Player1.PlayerInput.actions.FindActionMap("UI").FindAction("Submit").performed += HandlePlayer1Mash;
        PlayerController.Player2.PlayerInput.actions.FindActionMap("UI").FindAction("Submit").performed += HandlePlayer2Mash;
    }

    private void HandlePlayer1Mash(InputAction.CallbackContext context)
    {
        _player1MashCount++;
    }

    private void HandlePlayer2Mash(InputAction.CallbackContext context)
    {
        _player2MashCount++;
    }

    public override void Hide()
    {
        PlayerController.Player1.PlayerInput.actions.FindActionMap("UI").FindAction("Submit").performed -= HandlePlayer1Mash;
        PlayerController.Player2.PlayerInput.actions.FindActionMap("UI").FindAction("Submit").performed -= HandlePlayer2Mash;
        gameObject.SetActive(false);
    }

    public override bool IsCompleted()
    {
        return Mathf.Abs(_player1MashCount- _player2MashCount) >= m_MashDifferenceToWin;
    }
}
