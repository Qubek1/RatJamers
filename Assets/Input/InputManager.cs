using System;
using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// good ole InputManager to avoid common input handling and callback issues
/// </summary>
public class InputManager : MonoBehaviour {
    //public static readonly InputActions Player1InputActions=new();
    //public static readonly InputActions Player2InputActions=new();

    //public static InputActionAsset Player1InputActions;
    //public static InputActionAsset Player2InputActions;

    public event EventHandler InteractEvent;
    public event EventHandler AlternateInteractEvent;


    private void Awake() {
        //inputActions = new InputActions();

        //Player1InputActions.Player.Interact.performed += OnInteractPerformed;
        //Player1InputActions.Player.AlternateInteract.performed += OnAlternateInteractPerformed;
    }

    /*
    private void OnAlternateInteractPerformed(InputAction.CallbackContext obj) {
        AlternateInteractEvent?.Invoke(this, EventArgs.Empty);
    }

    private void OnInteractPerformed(InputAction.CallbackContext obj) {
        InteractEvent?.Invoke(this, EventArgs.Empty);
    }

    public Vector2 GetInputVector2Normalized() {
        //return Player1InputActions.Player.Move.ReadValue<Vector2>();
        Player1In
        return Player1InputActions.Player.Move.ReadValue<Vector2>();
    }
    */
}