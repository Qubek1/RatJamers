using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using System;

public class ClickingOrderMinigameController : MinigameController
{
    [SerializeField]
    private ClickingOrderController clickingOrderController;

    private List<InputButton> inputsList;

    private void Update()
    {
        if (IsCompleted())
        {
            MinigameLeft();
        }
    }

    public override void Launch(int launchingPlayer, int onPlayerSide, WorkstationController caller)
    {
        base.Launch(launchingPlayer, onPlayerSide,caller);
        gameObject.SetActive(true);
        InputActionAsset actions = PlayerController.GetPlayer(launchingPlayer).PlayerInput.actions;
        inputsList = new List<InputButton>()
        {
            new InputButton(actions.FindActionMap("ButtonInOrder").FindAction("WestButton"), clickingOrderController.Click, 0),
            new InputButton(actions.FindActionMap("ButtonInOrder").FindAction("NorthButton"), clickingOrderController.Click, 1),
            new InputButton(actions.FindActionMap("ButtonInOrder").FindAction("SouthButton"), clickingOrderController.Click, 2),
            new InputButton(actions.FindActionMap("ButtonInOrder").FindAction("EastButton"), clickingOrderController.Click, 3)
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