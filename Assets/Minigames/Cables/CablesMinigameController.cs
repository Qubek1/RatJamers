using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class CablesMinigameController : MinigameController
{
    public float maxSabotageTime = 10;
    public List<Cable> cables;
    public int currentlyControlledCableIndex;
    public CablesOverlapController overlapController;

    //[SerializeField] private TextMeshProUGUI m_TimeLimitText;
    
    //[SerializeField] private float SabotageTimeLimit = 5f;
    // Start is called before the first frame update

    private InputAction _axisInputAction;
    private float launchTime;

    public override void Launch(PlayerController interactingPlayer)
    {
        base.Launch(interactingPlayer);
        gameObject.SetActive(true);

        interactingPlayer.minigameButtonsActions[0].performed += HandleCableChanged1;
        interactingPlayer.minigameButtonsActions[1].performed += HandleCableChanged2;
        interactingPlayer.minigameButtonsActions[2].performed += HandleCableChanged3;
        _axisInputAction = interactingPlayer.minigameMoveAction;

        launchTime = Time.time;
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
    }

    public override void Hide()
    {
        //NOT SURE IF NEEDED - MAKS
        //PlayerController.GetPlayer(UsedByPlayer).PlayerInput.actions.FindActionMap("UI").FindAction("Move").Disable();
        //PlayerController.GetPlayer(UsedByPlayer).PlayerInput.actions.FindActionMap("UI").FindAction("Exit").performed 
        //    -= ExitMinigameFromInput;
        gameObject.SetActive(false);

        interactingPlayer.minigameButtonsActions[0].performed -= HandleCableChanged1;
        interactingPlayer.minigameButtonsActions[1].performed -= HandleCableChanged2;
        interactingPlayer.minigameButtonsActions[2].performed -= HandleCableChanged3;
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
        Vector2 inputVector = _axisInputAction.ReadValue<Vector2>();
        cables[currentlyControlledCableIndex].SetMovementVector(inputVector.x * Vector2.right + inputVector.y * Vector2.up);
        if (IsCompleted() && !IsSabotage())
        {
            workStation.GetComponent<Animator>().SetTrigger("Fixed");
            MinigameFinish(100);
        }
        else if (IsSabotage() && Time.time > launchTime + maxSabotageTime)
        {
            if (!IsCompleted())
            {
                workStation.GetComponent<Animator>().SetTrigger("Sabotaged");
                MinigameFinish(-100);
            }
            else
                MinigameFinish(0);
        }
        Debug.Log($"is completed {IsCompleted().ToString()}");
        Debug.Log($"is sabotage {IsSabotage().ToString()}");
    }

    public override bool CanStartNegative() => IsCompleted();

    public override bool CanStartPositive() => !IsCompleted();
}
