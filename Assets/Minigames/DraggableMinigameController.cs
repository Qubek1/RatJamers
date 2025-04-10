using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class DraggableMinigameController : MonoBehaviour, InputActions.IUIActions
{
    private List<DraggableComponent> m_AllDraggables = new();
    
    private DraggableComponent currentlyDragged;
    // Start is called before the first frame update
    void Start()
    {
        m_AllDraggables.Clear();
        foreach (DraggableComponent draggable in FindObjectsOfType<DraggableComponent>())
            m_AllDraggables.Add(draggable);
        
        InputManager.inputActions.UI.Enable();
        InputManager.inputActions.UI.SetCallbacks(this);
        if (EventSystem.current == null || EventSystem.current.firstSelectedGameObject == null)
        {
            EventSystem.current.SetSelectedGameObject(m_AllDraggables[0].gameObject);
            currentlyDragged = m_AllDraggables[0];
        }
        else
        {
            currentlyDragged = EventSystem.current.firstSelectedGameObject.GetComponent<DraggableComponent>();
        }
    }
        

    private void Update()
    {
        Vector2 navInput = InputManager.inputActions.UI.Move.ReadValue<Vector2>();
        if (navInput != Vector2.zero)
            currentlyDragged?.Move(navInput);
    }

    public void OnNavigate(InputAction.CallbackContext context)
    {
        Vector2 navigationDirection = context.ReadValue<Vector2>();
    }

    public void OnMove(InputAction.CallbackContext context)
    {
    }

    public void OnSubmit(InputAction.CallbackContext context)
    {
        //throw new System.NotImplementedException();
    }

    public void OnCancel(InputAction.CallbackContext context)
    {
        //throw new System.NotImplementedException();
    }

    public void OnPoint(InputAction.CallbackContext context)
    {
        //throw new System.NotImplementedException();
    }

    public void OnClick(InputAction.CallbackContext context)
    {
        //throw new System.NotImplementedException();
    }

    public void OnScrollWheel(InputAction.CallbackContext context)
    {
        //throw new System.NotImplementedException();
    }

    public void OnMiddleClick(InputAction.CallbackContext context)
    {
        //throw new System.NotImplementedException();
    }

    public void OnRightClick(InputAction.CallbackContext context)
    {
        //throw new System.NotImplementedException();
    }

    public void OnTrackedDevicePosition(InputAction.CallbackContext context)
    {
        //throw new System.NotImplementedException();
    }

    public void OnTrackedDeviceOrientation(InputAction.CallbackContext context)
    {
        //throw new System.NotImplementedException();
    }
}
