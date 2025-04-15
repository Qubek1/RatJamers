using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class ClickingOrderPvP : PVPMinigameController
{
    public ClickingOrderController clickingOrderController1;
    public ClickingOrderController clickingOrderController2;

    public float movement = 0.2f;
    private Vector3 StartPos1;
    private Vector3 startPos2;
    public Transform Rat1;
    public Transform Rat2;

    private List<InputButton> inputsList1;
    private List<InputButton> inputsList2;

    private void Awake()
    {
        StartPos1 = Rat1.position;
        startPos2 = Rat2.position;
    }

    void Update()
    {
        Rat1.transform.position = StartPos1 + Vector3.right * movement * clickingOrderController1.progress;
        Rat2.transform.position = startPos2 + Vector3.left * movement * clickingOrderController2.progress;
        if (clickingOrderController1.IsComplete())
        {
            PVPMinigameFinish(playerLeftController, playerRightController);
        }
        else if (clickingOrderController2.IsComplete())
        {
            PVPMinigameFinish(playerRightController, playerLeftController);
        }
    }

    protected override void Launch(Camera pvpMinigamesCamera, PlayerController playerLeft, PlayerController playerRight)
    {
        gameObject.SetActive(true);
        pvpMinigamesCamera.transform.position = transform.position;

        List<int> inputsOrder = new List<int>();
        for (int i=0; i<clickingOrderController1.lenght; i++)
        {
            inputsOrder.Add(Random.Range(0, 4));
        }
        clickingOrderController1.buttonsToClickIndexes = inputsOrder;
        clickingOrderController2.buttonsToClickIndexes = inputsOrder;

        gameObject.SetActive(true);
        
        inputsList1 = new List<InputButton>()
        {
            new InputButton(playerLeft.minigameButtonsAction[0], clickingOrderController1.Click, 0),
            new InputButton(playerLeft.minigameButtonsAction[1], clickingOrderController1.Click, 1),
            new InputButton(playerLeft.minigameButtonsAction[2], clickingOrderController1.Click, 2),
            new InputButton(playerLeft.minigameButtonsAction[3], clickingOrderController1.Click, 3)
        };
        inputsList2 = new List<InputButton>()
        {
            new InputButton(playerRight.minigameButtonsAction[0], clickingOrderController2.Click, 0),
            new InputButton(playerRight.minigameButtonsAction[1], clickingOrderController2.Click, 1),
            new InputButton(playerRight.minigameButtonsAction[2], clickingOrderController2.Click, 2),
            new InputButton(playerRight.minigameButtonsAction[3], clickingOrderController2.Click, 3)
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
