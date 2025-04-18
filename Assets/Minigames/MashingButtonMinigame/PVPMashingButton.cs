using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PVPMashingButton : PVPMinigameController
{
    [SerializeField]
    private XboxButtonSprite xboxButtonSprite;
    [SerializeField]
    private Animator laserAnimator;
    [SerializeField]
    private int pointsDifferenceToWin = 10;
    [SerializeField]
    private float minChangeTime = 1f;
    [SerializeField]
    private float maxChangeTime = 3f;
    [SerializeField]
    private float buttonFillChangeDelay = 0.2f;
    [SerializeField]
    private int currentPoints = 0;
    [SerializeField]
    private int currentButtonToMash;

    private float nextChangeTime;
    private float nextButtonFillChangeTime;

    private PlayerController leftPlayerController;
    private PlayerController rightPlayerController;

    private float currentTime = 0.5f;

    public override void Launch(PlayerController playerLeft, PlayerController playerRight)
    {
        gameObject.SetActive(true);
        currentPoints = 0;
        playerLeft.onMinigameButtonClicked += leftPlayerButtonClicked;
        playerRight.onMinigameButtonClicked += rightPlayerButtonClicked;
        leftPlayerController = playerLeft;
        rightPlayerController = playerRight;
        currentTime = 0.5f;
        ChangeButtonToMash();
        FlipButtonFill();
    }

    private void Update()
    {
        if (Time.time >= nextChangeTime)
        {
            ChangeButtonToMash();
        }
        if (Time.time >= nextButtonFillChangeTime)
        {
            FlipButtonFill();
        }
        UpdateLaserAnimator();

        if (currentPoints >= pointsDifferenceToWin)
        {
            PVPMinigameFinish(leftPlayerController, rightPlayerController);
        }
        if (currentPoints <= -pointsDifferenceToWin)
        {
            PVPMinigameFinish(rightPlayerController, leftPlayerController);
        }
    }

    private void UpdateLaserAnimator()
    {
        currentTime = Mathf.Lerp(currentTime, ((float)currentPoints + pointsDifferenceToWin) / (pointsDifferenceToWin * 2), 0.1f);
        laserAnimator.SetFloat("Progress", currentTime);
    }

    private void ChangeButtonToMash()
    {
        nextChangeTime = Time.time + Random.Range(minChangeTime, maxChangeTime);
        currentButtonToMash = Random.Range(0, 4);
        xboxButtonSprite.SetButton(currentButtonToMash);
    }

    private void FlipButtonFill()
    {
        nextButtonFillChangeTime = Time.time + buttonFillChangeDelay;
        xboxButtonSprite.FlipFillState();
    }

    public override bool IsCompleted() => Mathf.Abs(currentPoints) >= pointsDifferenceToWin;

    public override void Hide()
    {
        leftPlayerController.onMinigameButtonClicked -= leftPlayerButtonClicked;
        rightPlayerController.onMinigameButtonClicked -= rightPlayerButtonClicked;
        gameObject.SetActive(false);
    }

    public void leftPlayerButtonClicked(int buttonIndex, InputAction.CallbackContext callbackContext)
    {
        Debug.Log(buttonIndex);
        Debug.Log(currentButtonToMash);
        if (buttonIndex == currentButtonToMash)
        {
            currentPoints++;
        }
        else
        {
            currentPoints--;
        }
    }

    public void rightPlayerButtonClicked(int buttonIndex, InputAction.CallbackContext callbackContext)
    {
        Debug.Log(buttonIndex);
        Debug.Log(currentButtonToMash);
        if (buttonIndex == currentButtonToMash)
        {
            currentPoints--;
        }
        else
        {
            currentPoints++;
        }
    }
}
