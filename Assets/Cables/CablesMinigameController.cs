using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class CablesMinigameController : MinigameController
{
    public List<Cable> cables;
    public int currentlyControlledCableIndex;
    public CablesOverlapController overlapController;

    //[SerializeField] private TextMeshProUGUI m_TimeLimitText;
    
    //[SerializeField] private float SabotageTimeLimit = 5f;
    // Start is called before the first frame update

    private InputAction _axisInputAction;

    public override void Launch(int launchingPlayer,int onPlayerSide,WorkstationController caller)
    {
        base.Launch(launchingPlayer,onPlayerSide, caller);
        gameObject.SetActive(true);
        
        overlapController.interactedCable = cables[currentlyControlledCableIndex];
        
        for (int cableIndex = 0; cableIndex < cables.Count; cableIndex++)
        {
            cables[cableIndex].InitSplineCable();
            if (cableIndex == currentlyControlledCableIndex)
            {
                cables[cableIndex].Select();
            }
            else
            {
                cables[cableIndex].Deselect();
            }
        }
        
        PlayerController playerInstance= PlayerController.GetPlayer(UsedByPlayer);
        Debug.Log("IsPlayedNull: " + playerInstance == null);
        _axisInputAction =
            playerInstance.PlayerInput.actions.FindActionMap("UI").FindAction("Move");
        _axisInputAction.Enable();

        playerInstance.PlayerInput.actions.FindActionMap("UI").FindAction("SouthButton").performed +=
            HandleCableChanged1;
        playerInstance.PlayerInput.actions.FindActionMap("UI").FindAction("EastButton").performed +=
            HandleCableChanged2;
        playerInstance.PlayerInput.actions.FindActionMap("UI").FindAction("WestButton").performed +=
            HandleCableChanged3;
        
        //_axisInputAction.Enable();
        //playerInstance.PlayerInput.actions.FindActionMap("UI").FindAction("Exit").performed
        //    +=ExitMinigameFromInput;

        //setup time limit if its a sabotage
        //Debug.Log($"IsSabotage: {IsSabotage()}");

    }

    protected override void Start()
    {
       // base.Start();
       gameObject.SetActive(false);
    }

    public override void Hide()
    {
        //NOT SURE IF NEEDED - MAKS
        //PlayerController.GetPlayer(UsedByPlayer).PlayerInput.actions.FindActionMap("UI").FindAction("Move").Disable();
        //PlayerController.GetPlayer(UsedByPlayer).PlayerInput.actions.FindActionMap("UI").FindAction("Exit").performed 
        //    -= ExitMinigameFromInput;
        gameObject.SetActive(false);
        PlayerController playerInstance= PlayerController.GetPlayer(UsedByPlayer);
        
        playerInstance.PlayerInput.actions.FindActionMap("UI").FindAction("SouthButton").performed -=
            HandleCableChanged1;
        playerInstance.PlayerInput.actions.FindActionMap("UI").FindAction("EastButton").performed -=
            HandleCableChanged2;
        playerInstance.PlayerInput.actions.FindActionMap("UI").FindAction("WestButton").performed -=
            HandleCableChanged3;
        StopAllCoroutines();
    }

    //private void ExitMinigameFromInput(InputAction.CallbackContext context)=>MinigameLeft();

    public override bool IsCompleted()
    {
        foreach (var cable in cables)
        {
            if (!cable.connected) return false;
        }

        return true;
    }

    private void HandleCableChanged1(InputAction.CallbackContext context)
    {
        currentlyControlledCableIndex = 0;
        OnCableChanged();
    }
    private void HandleCableChanged2(InputAction.CallbackContext context)
    {
        currentlyControlledCableIndex = 1;
        OnCableChanged();
    }
    private void HandleCableChanged3(InputAction.CallbackContext context)
    {
        currentlyControlledCableIndex = 2;
        OnCableChanged();
    }

    private void OnCableChanged()
    {
        overlapController.interactedCable = cables[currentlyControlledCableIndex];
        for (int cableIndex = 0; cableIndex < cables.Count; cableIndex++)
        {
            if (cableIndex == currentlyControlledCableIndex)
            {
                cables[cableIndex].Select();
            }
            else
            {
                cables[cableIndex].Deselect();
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(IsCompleted())
            MinigameLeft();
        Vector2 inputVector = _axisInputAction.ReadValue<Vector2>();
        cables[currentlyControlledCableIndex].SetMovementVector(inputVector.x * Vector2.right + inputVector.y * Vector2.up);
    }
}
