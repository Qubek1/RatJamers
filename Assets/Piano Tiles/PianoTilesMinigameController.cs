using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PianoTilesMinigameController : MonoBehaviour
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

    // Start is called before the first frame update
    void Awake()
    {
        inputManager = new InputActions();
        lanesButtons.Add(new LaneButton(inputManager.Player.ButtonWest, LaneButtonTap, 0));
        lanesButtons.Add(new LaneButton(inputManager.Player.ButtonNorth, LaneButtonTap, 1));
        lanesButtons.Add(new LaneButton(inputManager.Player.ButtonSouth, LaneButtonTap, 2));
        lanesButtons.Add(new LaneButton(inputManager.Player.ButtonEast, LaneButtonTap, 3));
        Restart();
        tilesController.missedTimeStampOnLane += MissedTimeStampOnLane;
    }

    // Update is called once per frame
    void Update()
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

    private void Performed(InputAction.CallbackContext callbackContext)
    {
        action.Invoke(argument);
    }

    public void Disable()
    {
        inputAction.Disable();
    }
}