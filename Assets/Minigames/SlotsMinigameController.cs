using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class SlotsMinigameController : MonoBehaviour, InputActions.IUIActions
{
    private List<SlotComponent> _allSlots = new();
    private List<PuttableComponent> _allPuttables = new();
    
    private SlotComponent _activeSlot;
    private PuttableComponent _activePuttable;
    
    void Start()
    {
        _allSlots.Clear();
        foreach (SlotComponent slot in FindObjectsOfType<SlotComponent>())
            _allSlots.Add(slot);
        
        _allPuttables.Clear();
        foreach (PuttableComponent puttable in FindObjectsOfType<PuttableComponent>())
            _allPuttables.Add(puttable);
        
        
        InputManager.inputActions.UI.Enable();
        InputManager.inputActions.UI.SetCallbacks(this);
        if (EventSystem.current == null || EventSystem.current.firstSelectedGameObject == null)
        {
            EventSystem.current.SetSelectedGameObject(_allPuttables[0].gameObject);
            _activePuttable = _allPuttables[0];
        }
        else
        {
            _activePuttable = EventSystem.current.firstSelectedGameObject.GetComponent<PuttableComponent>();
        }

        if (_activePuttable == null)
        {
            Debug.LogWarning("initial activePuttable is still null after Start!");
        }
        
        _activePuttable.OnSelected();
        _activeSlot = _allSlots[0];
        _activeSlot.OnSelected();
    }

    public void OnNavigate(InputAction.CallbackContext context)
    {
        if(!context.performed) return;
        Vector2 navigationDirection = context.ReadValue<Vector2>().normalized;
        if(navigationDirection==Vector2.zero) return;
        
    }

    public void OnReset(InputAction.CallbackContext context)
    {
        
    }

    public void OnMove(InputAction.CallbackContext context) { }
    public void OnSubmit(InputAction.CallbackContext context) { }
    public void OnCancel(InputAction.CallbackContext context) { }
    public void OnPoint(InputAction.CallbackContext context) { }
    public void OnClick(InputAction.CallbackContext context) { }
    public void OnScrollWheel(InputAction.CallbackContext context) { }
    public void OnMiddleClick(InputAction.CallbackContext context) { }
    public void OnRightClick(InputAction.CallbackContext context) { }
    public void OnTrackedDevicePosition(InputAction.CallbackContext context) { }
    public void OnTrackedDeviceOrientation(InputAction.CallbackContext context) { }
}
