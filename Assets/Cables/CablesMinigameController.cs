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

    [SerializeField] private TextMeshProUGUI m_TimeLimitText;
    
    [SerializeField] private float SabotageTimeLimit = 5f;
    // Start is called before the first frame update

    private InputAction _axisInputAction;
    
    protected override void Start()
    {
        
        base.Start();
    }

    public override void Launch(int launchingPlayer,int onPlayerSide)
    {
        base.Launch(launchingPlayer,onPlayerSide);
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
        
        PlayerController playerInstance= PlayerController.GetPlayer(launchingPlayer);
        _axisInputAction =
            playerInstance.PlayerInput.actions.FindActionMap("UI").FindAction("Move");
        _axisInputAction.Enable();
        //_axisInputAction.Enable();
        playerInstance.PlayerInput.actions.FindActionMap("UI").FindAction("Exit").performed
            +=ExitMinigameFromInput;
        
        //setup time limit if its a sabotage
        Debug.Log($"IsSabotage: {IsSabotage()}");
        if (IsSabotage())
        {
            m_TimeLimitText.gameObject.SetActive(true);
            StartCoroutine(SabotageTimeLimitCoroutine());
        }
        else
        {
            m_TimeLimitText.gameObject.SetActive(false);
        }
        
    }

    private IEnumerator SabotageTimeLimitCoroutine()
    {
        float timeLeft = SabotageTimeLimit;
        while (timeLeft > 0)
        {
            timeLeft-=Time.deltaTime;
            m_TimeLimitText.text = timeLeft.ToString("0.00");
            yield return null;

        }
        //yield return new WaitForSeconds(SabotageTimeLimit);
        MinigameLeft();
    }

    public override void Hide()
    {
        //NOT SURE IF NEEDED - MAKS
        //PlayerController.GetPlayer(UsedByPlayer).PlayerInput.actions.FindActionMap("UI").FindAction("Move").Disable();
        PlayerController.GetPlayer(UsedByPlayer).PlayerInput.actions.FindActionMap("UI").FindAction("Exit").performed 
            -= ExitMinigameFromInput;
        gameObject.SetActive(false);
        StopAllCoroutines();
    }

    private void ExitMinigameFromInput(InputAction.CallbackContext context)=>MinigameLeft();

    public override bool IsCompleted()
    {
        return false;
    }

    // Update is called once per frame
    void Update()
    {
        bool changed = false;
        if (Input.GetKey(KeyCode.Alpha1))
        {
            changed = true;
            currentlyControlledCableIndex = 0;
        }
        if (Input.GetKey(KeyCode.Alpha2))
        {
            changed = true;
            currentlyControlledCableIndex = 1;
        }
        if (Input.GetKey(KeyCode.Alpha3))
        {
            changed = true;
            currentlyControlledCableIndex = 2;
        }
        if (Input.GetKey(KeyCode.Alpha4))
        {
            changed = true;
            currentlyControlledCableIndex = 3;
        }
        if (Input.GetKey(KeyCode.Alpha5))
        {
            changed = true;
            currentlyControlledCableIndex = 4;
        }
        if (changed)
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

        Vector2 inputVector = _axisInputAction.ReadValue<Vector2>();
        cables[currentlyControlledCableIndex].SetMovementVector(inputVector.x * Vector2.right + inputVector.y * Vector2.up);
    }
}
