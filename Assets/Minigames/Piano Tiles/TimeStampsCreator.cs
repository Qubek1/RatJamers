using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class TimeStampsCreator : MonoBehaviour
{
    public TilesController tileController;

    private List<LaneButton> lanesButtons = new List<LaneButton>(4);
    private InputActions inputManager;

    // Start is called before the first frame update
    void Awake()
    {
        inputManager = new InputActions();
        lanesButtons.Add(new LaneButton(inputManager.UIqbek.WestButton, LaneButtonTap, 0));
        lanesButtons.Add(new LaneButton(inputManager.UIqbek.NorthButton, LaneButtonTap, 1));
        lanesButtons.Add(new LaneButton(inputManager.UIqbek.SouthButton, LaneButtonTap, 2));
        lanesButtons.Add(new LaneButton(inputManager.UIqbek.EastButton, LaneButtonTap, 3));
    }

    private void OnDisable()
    {
        foreach (LaneButton inputAction in lanesButtons)
        {
            inputAction.Disable();
        }
    }

    private void LaneButtonTap(int lane)
    {
        tileController.AddTimeStamp(lane);
    }

    // Update is called once per frame
    void Update()
    {

    }
}