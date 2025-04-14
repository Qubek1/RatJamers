using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using System;

public class ClickingOrderMinigameController : MinigameController
{
    [SerializeField]
    private float progress;
    [SerializeField]
    private ClickingOrderController clickingOrderController;

    private List<InputButton> inputsList;

    private void Update()
    {
        if (IsCompleted())
        {
            MinigameFinish(progress);
        }
    }

    public override void Launch(PlayerController interactingPlayer)
    {
        base.Launch(interactingPlayer);
        gameObject.SetActive(true);
        inputsList = new List<InputButton>()
        {
            new InputButton(interactingPlayer.minigameButtonsAction[0], clickingOrderController.Click, 0),
            new InputButton(interactingPlayer.minigameButtonsAction[1], clickingOrderController.Click, 1),
            new InputButton(interactingPlayer.minigameButtonsAction[2], clickingOrderController.Click, 2),
            new InputButton(interactingPlayer.minigameButtonsAction[3], clickingOrderController.Click, 3)
        };
        List<int> inputsOrder = new List<int>();
        for (int i = 0; i < clickingOrderController.lenght; i++)
        {
            inputsOrder.Add(UnityEngine.Random.Range(0, 4));
        }
        clickingOrderController.buttonsToClickIndexes = inputsOrder;
        clickingOrderController.Generate();
    }

    public override void Hide()
    {
        foreach (InputButton inputButton in inputsList)
        {
            inputButton.UnSubsribe();
        }
        clickingOrderController.DestroyButtons();
        gameObject.SetActive(false);
    }

    public override bool IsCompleted()
    {
        return clickingOrderController.IsComplete();
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

class InputButton
{
    private int arg;
    private Action<int> action;
    private InputAction inputAction;

    public InputButton(InputAction inputAction, Action<int> action, int arg)
    {
        inputAction.performed += Click;
        this.action = action;
        this.inputAction = inputAction;
        this.arg = arg;
    }

    private void Click(InputAction.CallbackContext context)
    {
        action.Invoke(arg);
    }

    public void UnSubsribe()
    {
        inputAction.performed -= Click;
    }
}