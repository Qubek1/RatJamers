using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class ClickingOrderPvP : PvPMinigameController
{
    public ClickingOrderController clickingOrderController1;
    public ClickingOrderController clickingOrderController2;

    private List<InputButton> inputsList1;
    private List<InputButton> inputsList2;

    void Update()
    {
        if (!IsCompleted()) return;

        GameManager.Instance.PVPMinigameFinished(clickingOrderController1.progress > clickingOrderController2.progress);
        Hide();
    }

    public override void Launch()
    {
        gameObject.SetActive(true);

        List<int> inputsOrder = new List<int>();
        for (int i=0; i<clickingOrderController1.lenght; i++)
        {
            inputsOrder.Add(Random.Range(0, 4));
        }
        clickingOrderController1.buttonsToClickIndexes = inputsOrder;
        clickingOrderController2.buttonsToClickIndexes = inputsOrder;

        gameObject.SetActive(true);
        InputActionAsset actions1 = PlayerController.Player1.PlayerInput.actions;
        InputActionAsset actions2 = PlayerController.Player2.PlayerInput.actions;
        inputsList1 = new List<InputButton>()
        {
            new InputButton(actions1.FindActionMap("UI").FindAction("WestButton"), clickingOrderController1.Click, 0),
            new InputButton(actions1.FindActionMap("UI").FindAction("NorthButton"), clickingOrderController1.Click, 1),
            new InputButton(actions1.FindActionMap("UI").FindAction("SouthButton"), clickingOrderController1.Click, 2),
            new InputButton(actions1.FindActionMap("UI").FindAction("EastButton"), clickingOrderController1.Click, 3)
        };
        inputsList2 = new List<InputButton>()
        {
            new InputButton(actions2.FindActionMap("UI").FindAction("WestButton"), clickingOrderController2.Click, 0),
            new InputButton(actions2.FindActionMap("UI").FindAction("NorthButton"), clickingOrderController2.Click, 1),
            new InputButton(actions2.FindActionMap("UI").FindAction("SouthButton"), clickingOrderController2.Click, 2),
            new InputButton(actions2.FindActionMap("UI").FindAction("EastButton"), clickingOrderController2.Click, 3)
        };
        clickingOrderController1.Generate();
        clickingOrderController2.Generate();
    }

    public override void Hide()
    {
        foreach (InputButton inputButton in inputsList1)
        {
            inputButton.UnSubsribe();
        }
        foreach (InputButton inputButton in inputsList2)
        {
            inputButton.UnSubsribe();
        }
        clickingOrderController1.DestroyButtons();
        clickingOrderController2.DestroyButtons();
        gameObject.SetActive(false);
    }

    public override bool IsCompleted()
    {
        return clickingOrderController1.IsComplete() || clickingOrderController2.IsComplete();
    }
}
