using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.UI;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Splines;
using UnityEngine.UIElements;

public class PianoTilesMinigameController : MinigameController
{
    public float maxProgress;

    public float currentSpeed = 1;
    public float speedGain = 0.05f;
    public float speedLossLoss = 0.2f;
    public float maxError = 0.1f;

    public MusicController musicController;
    public TilesController tilesController;
    public List<XboxButton> xboxButtons;

    [SerializeField]
    private int correct = 0;
    [SerializeField]
    private int incorrect = 0;

    private List<LaneButton> lanesButtons = new List<LaneButton>(4);
    private InputActions inputManager;

    private void Awake()
    {
        tilesController.missedTimeStampOnLane += MissedTimeStampOnLane;
    }

    public override void Launch(PlayerController interactingPlayer)
    {
        base.Launch(interactingPlayer);
        gameObject.SetActive(true);
        lanesButtons = new List<LaneButton>(4)
        {
            new LaneButton(interactingPlayer.minigameButtonsActions[0], LaneButtonTap, 0),
            new LaneButton(interactingPlayer.minigameButtonsActions[1], LaneButtonTap, 1),
            new LaneButton(interactingPlayer.minigameButtonsActions[2], LaneButtonTap, 2),
            new LaneButton(interactingPlayer.minigameButtonsActions[3], LaneButtonTap, 3)
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
            MinigameFinish(maxProgress * ((float)correct / (correct + incorrect)));
        }
    }

    public void Restart()
    {
        musicController.progressInSeconds = 0;
        musicController.audioSource.UnPause();
        currentSpeed = 1;
        tilesController.maxError = maxError;
        tilesController.Restart();
        correct = 0;
        incorrect = 0;
    }

    private void LaneButtonTap(int lane)
    {
        if (tilesController.ActionOnLane(lane))
        {
            currentSpeed += speedGain;
            correct++;
        }
        else
        {
            currentSpeed -= speedLossLoss;
            incorrect++;
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
        incorrect++;
    }

    public override void Hide()
    {
        foreach (var laneButton in lanesButtons)
            laneButton.DisconnectFromInputAction();
        gameObject.SetActive(false);
    }

    public override bool IsCompleted()
    {
        return musicController.IsCompleted();
    }

    public override bool CanStartNegative()
    {
        return false;
    }

    public override bool CanStartPositive()
    {
        return true;
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