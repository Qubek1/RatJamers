using System;
using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// good ole InputManager to avoid common input handling and callback issues
/// </summary>
public class InputManager : MonoBehaviour {
    public static readonly InputActions inputActions=new();

    public event EventHandler InteractEvent;
    public event EventHandler AlternateInteractEvent;


    private void Awake() {
        //inputActions = new InputActions();
        inputActions.Player.Enable();

        inputActions.Player.Interact.performed += OnInteractPerformed;
        inputActions.Player.AlternateInteract.performed += OnAlternateInteractPerformed;
    }

    private void OnAlternateInteractPerformed(InputAction.CallbackContext obj) {
        AlternateInteractEvent?.Invoke(this, EventArgs.Empty);
    }

    private void OnInteractPerformed(InputAction.CallbackContext obj) {
        InteractEvent?.Invoke(this, EventArgs.Empty);
    }

    public Vector2 GetInputVector2Normalized() {
        return inputActions.Player.Move.ReadValue<Vector2>();
    }
}