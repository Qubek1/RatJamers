using System;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class DraggableMinigameController : MinigameController, InputActions.IUIActions
{
    public const float DRAGGABLE_POSITION_THRESHOLD = 1.5f;
    public const float DRAGGABLE_ROTATION_THRESHOLD = 10f;
    

    [SerializeField] private bool m_Rotatable;
    
    private List<DraggableComponent> _allDraggables = new();
    
    private List<DraggableSlotComponent> _allSlots = new();
    private DraggableComponent _currentlyDragged;
    protected override void Start()
    {
        //if(SceneManager.sceneCount>1)
        base.Start();
        _allDraggables.Clear();
        foreach (DraggableComponent draggable in GetComponentsInChildren<DraggableComponent>())
            _allDraggables.Add(draggable);
        
        _allSlots.Clear();
        foreach (DraggableSlotComponent slot in GetComponentsInChildren<DraggableSlotComponent>())
            _allSlots.Add(slot);
        
        _currentlyDragged = _allDraggables[0];

        if (_currentlyDragged == null)
        {
            Debug.LogWarning("initial currentlydragged is still null after Start!");
        }
        _currentlyDragged.OnSelected();
    }
    
    public override void Launch(int player)
    {
        base.Launch(player);
        gameObject.SetActive(true);
        
        //subscribe to needed input events from player
        PlayerController playerInstance = PlayerController.GetPlayer(player);
        _moveAction =
            playerInstance.PlayerInput.actions.FindActionMap("UI").FindAction("Move");
        _moveAction.Enable();
        playerInstance.PlayerInput.SwitchCurrentActionMap("UI");
        //playerInstance.PlayerInput.actions.FindActionMap("UI").Enable();
        
        playerInstance.PlayerInput.actions.FindActionMap("UI").FindAction("Navigate").performed += OnNavigate;
        playerInstance.PlayerInput.actions.FindActionMap("UI").FindAction("Navigate").started += OnNavigate;
        playerInstance.PlayerInput.actions.FindActionMap("UI").FindAction("Navigate").canceled += OnNavigate;
    }

    public override void Hide()
    {
        gameObject.SetActive(false);
        PlayerController.GetPlayer(UsedByPlayer).PlayerInput.actions.FindActionMap("UI").Disable();
        PlayerController.GetPlayer(UsedByPlayer).PlayerInput.actions.FindActionMap("UI").FindAction("Navigate").performed -= OnNavigate;
        PlayerController.GetPlayer(UsedByPlayer).PlayerInput.actions.FindActionMap("UI").FindAction("Navigate").started -= OnNavigate;
        PlayerController.GetPlayer(UsedByPlayer).PlayerInput.actions.FindActionMap("UI").FindAction("Navigate").canceled -= OnNavigate;

    }

    private InputAction _moveAction;

    private Vector2 GetMoveInput() => _moveAction.ReadValue<Vector2>();


    private void Update()
    {
        Vector2 navInput = GetMoveInput();
        //Debug.Log("navInput: " + navInput);
        if (navInput == Vector2.zero||_currentlyDragged==null)
            return;
        
        _currentlyDragged.Move(navInput);
        DraggableSlotComponent maybeSlot=DraggableSlotComponent.IsWithin(_currentlyDragged.transform.position);
        if (maybeSlot == null || maybeSlot.IsUsed()||maybeSlot==_currentlyDragged.LastSlot) return;
        
        //check if rotation is correct to snap to slot
        /*
        if (m_Rotatable)
        {
            float angleDelta = Vector2.SignedAngle(maybeSlot.transform.position - _currentlyDragged.transform.position, _currentlyDragged.transform.up);
            if (Mathf.Abs(angleDelta) <= DRAGGABLE_ROTATION_THRESHOLD)
            {
                maybeSlot.UseSlot(_currentlyDragged);
                _currentlyDragged.transform.position=maybeSlot.transform.position;
                _currentlyDragged.OnPutIntoSlot(maybeSlot);
            }
        }
        */
        maybeSlot.UseSlot(_currentlyDragged);
        _currentlyDragged.transform.position=maybeSlot.transform.position;
        _currentlyDragged.OnPutIntoSlot(maybeSlot);

        if (IsCompleted())
        {
            MinigameLeft();
        }
    }

    public override bool IsCompleted()
    {
        foreach (var slot in _allSlots)
            if (!slot.IsCorrectAssigned())
                return false;
        return true;
    }
    
    public void OnNavigate(InputAction.CallbackContext context)
    {
        if(!context.performed) return;
        Vector2 navigationDirection = context.ReadValue<Vector2>().normalized;
        if(navigationDirection==Vector2.zero) return;
        //Debug.Log($"received navigate input {navigationDirection}");
        //Debug.Log($"navdir: {navigationDirection}, currentlyDragged {currentlyDragged.gameObject.name}");
        if (_currentlyDragged == null)
        {
            _currentlyDragged = GetClosestInDirection(from: Vector2.zero, dir: navigationDirection);
            _currentlyDragged.OnSelected();
        }
        else
        {
            _currentlyDragged.OnDeselected();
            _currentlyDragged = GetClosestInDirection(from: _currentlyDragged.transform.position, dir: navigationDirection);
            _currentlyDragged.OnSelected();
        } 
            
    }

    public void OnReset(InputAction.CallbackContext context)
    {
        if(!context.performed)
            return;
        base.Reset();
    }

    public void OnRotate(InputAction.CallbackContext context)
    {
        if (!m_Rotatable) return;
        float normalized = context.ReadValue<float>();
        //Debug.Log(normalized);
        if(Mathf.Approximately(normalized,0))
            _currentlyDragged?.StopRotate();
        else
            _currentlyDragged?.StartRotate(normalized);

    }

    private DraggableComponent GetClosestInDirection(Vector2 from, Vector2 dir)
    {
        DraggableComponent currentClosest=null;
        float currentClosestDist=float.MaxValue;
        foreach (var draggable in _allDraggables)
        {
            //do not switch to used slots, for now???
            if(draggable.IsInSlot) continue;
            //if its not to the left of from position, ignore it
            if(!IsInDirOf(from,draggable.transform.position,dir))
                continue;

            float distance = GetDistInDir(from, draggable.transform.position, dir);
            if (distance<currentClosestDist)
            {
                currentClosestDist = distance;
                currentClosest = draggable;
            }
        }
        //Debug.LogWarning("could not find currentClosest, returning currentlyDragged");
        return currentClosest!=null?currentClosest:_currentlyDragged;
    }

    private float GetDistInDir(Vector2 from, Vector2 to, Vector2 dir)
    {
        if (dir == Vector2.left || dir == Vector2.right)
            //return Mathf.Abs(from.x) - Mathf.Abs(to.x);
            return Mathf.Abs(from.x - to.x);
        if (dir == Vector2.up||dir == Vector2.down)
            //return Mathf.Abs(from.y) - Mathf.Abs(to.y);
            return Mathf.Abs(from.y - to.y);
        
        Debug.LogWarning("Non-conventional direction passed to getDistInDir, defaulting to float.MaxValue");
        return float.MaxValue;
    }
    private bool IsInDirOf(Vector2 from, Vector2 to, Vector2 dir)
    {
        if (dir == Vector2.left)
            return from.x > to.x;
        if (dir == Vector2.right)
            return from.x < to.x;
        if (dir == Vector2.up)
            return from.y < to.y;
        if (dir == Vector2.down)
            return from.y > to.y;
        Debug.LogWarning("Non-conventional direction passed to IsInDirOf, defaulting to false");
        return false;
    }

    public void OnMove(InputAction.CallbackContext context)
    {
    }

    public void OnSubmit(InputAction.CallbackContext context) { }

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
}
