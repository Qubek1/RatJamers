using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.UI;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Splines;

public class PianoTilesMinigameController : MinigameController
{
    public float currentSpeed = 1;
    public float speedGain = 0.05f;
    public float speedLossLoss = 0.2f;
    public float maxError = 0.1f;

    public MusicController musicController;
    public TilesController tilesController;
    public List<XboxButton> xboxButtons;

    private List<LaneButton> lanesButtons = new List<LaneButton>(4);
    private InputActions inputManager;

    private void Awake()
    {
        tilesController.missedTimeStampOnLane += MissedTimeStampOnLane;
    }

    public override void Launch(int launchingPlayer,int onPlayerSide)
    {
        base.Launch(launchingPlayer,onPlayerSide);
        gameObject.SetActive(true);
        InputActionAsset actions = PlayerController.GetPlayer(launchingPlayer).PlayerInput.actions;
        lanesButtons = new List<LaneButton>(4)
        {
            new LaneButton(actions.FindActionMap("ButtonInOrder").FindAction("WestButton"), LaneButtonTap, 0),
            new LaneButton(actions.FindActionMap("ButtonInOrder").FindAction("NorthButton"), LaneButtonTap, 1),
            new LaneButton(actions.FindActionMap("ButtonInOrder").FindAction("SouthButton"), LaneButtonTap, 2),
            new LaneButton(actions.FindActionMap("ButtonInOrder").FindAction("EastButton"), LaneButtonTap, 3)
        };
        Restart();
    }

    private void Update()
    {
        for (int i = 0; i < lanesButtons.Count; i++)
        {
            if (lanesButtons[i].inputAction.IsPressed())
            {
                xboxButtons[i].Press();
            }
            else
            {
                xboxButtons[i].Release();
            }
        }
        if (musicController.IsCompleted())
        {
            MinigameLeft();
        }
    }

    public void Restart()
    {
        musicController.progressInSeconds = 0;
        musicController.audioSource.UnPause();
        currentSpeed = 1;
        tilesController.maxError = maxError;
        tilesController.Restart();
    }

    private void LaneButtonTap(int lane)
    {
        if (tilesController.ActionOnLane(lane))
        {
            currentSpeed += speedGain;
        }
        else
        {
            currentSpeed -= speedLossLoss;
        }
        currentSpeed = Mathf.Max(1, currentSpeed);
        musicController.ChangeSpeed(currentSpeed);
    }

    private void MissedTimeStampOnLane(int lane)
    {
        //Debug.Log("Missed on lane " + (lane + 1).ToString() + "!");
        currentSpeed -= speedLossLoss;
        currentSpeed = Mathf.Max(1, currentSpeed);
        musicController.ChangeSpeed(currentSpeed);
    }

    public override void Hide()
    {
        gameObject.SetActive(false);
        foreach (var laneButton in lanesButtons)
            laneButton.DisconnectFromInputAction();
    }

    public override bool IsCompleted()
    {
        return musicController.IsCompleted();
    }
}

class LaneButton
{
    public InputAction inputAction;
    private Action<int> action;
    private int argument;

    public LaneButton(InputAction inputAction, Action<int> action, int argument)
    {
        this.inputAction = inputAction;
        this.action = action;
        this.argument = argument;
        inputAction.Enable();
        inputAction.performed += Performed;
    }

    public void DisconnectFromInputAction()
    {
        inputAction.Disable();
        inputAction.performed -= Performed;
    }

    private void Performed(InputAction.CallbackContext callbackContext)
    {
        action.Invoke(argument);
    }

    public void Disable()
    {
        inputAction.Disable();
    }
}