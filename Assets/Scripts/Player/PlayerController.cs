using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;


public class PlayerController : MonoBehaviour, InputActions.IPlayerActions
{
    public static PlayerController Player1;
    public static PlayerController Player2;

    public int GetPlayerNumber()
    {
        if (this == Player1) return 1;
        else return 2;
    }

    public PlayerController GetPlayer(int number)
    {
        if(number == 1) return Player1;
        else return Player2;
    }
    
    [SerializeField] private float playerSpeed = 20;
    
    
    [Header("Refs")]
    [SerializeField] private PlayerInteractionComponent m_InteractionComponent;
    [SerializeField] private PlayerInput m_PlayerInput;
    //[SerializeField] private PlayerCameraController m_CameraPrefab;
    [SerializeField] private PlayerCameraController m_CameraController;
    
    private Rigidbody2D _rb;
    private Vector2 _movementInput = Vector2.zero;


    private void Awake()
    {
        //PlayerInputManager.instance.JoinPlayer()
        if (Player1 == null)
        {
            Player1=this;
            InputManager.Player1InputActions.Enable();
            //InputManager.Player2InputActions.Player.RemoveCallbacks(Player2);
            //InputManager.Player1InputActions.Player.SetCallbacks(this);
        }
            
        else if (Player2 == null)
        {
            Player2=this;
            InputManager.Player2InputActions.Enable();
            //InputManager.Player2InputActions.Player.RemoveCallbacks(Player1);
            //InputManager.Player2InputActions.Player.SetCallbacks(this);
        }
        else
            Debug.LogError("More than 2 players in scene");
        
        _rb = GetComponent<Rigidbody2D>();
        
        MinigamesManager.MinigameEnteredAction += HandleMinigameEntered;
        MinigamesManager.MinigameLeftAction += HandleMinigameLeft;
        
        
    }

    private void Start()
    {
        //m_CameraController=Instantiate(m_CameraPrefab, transform.position, Quaternion.identity);
        //m_PlayerInput.camera=m_CameraController.GetComponent<Camera>();
        m_CameraController.SetTarget(transform);
    }
    

    private void OnDestroy()
    {
        MinigamesManager.MinigameEnteredAction -= HandleMinigameEntered;
        MinigamesManager.MinigameLeftAction -= HandleMinigameLeft;
    }

    private void HandleMinigameEntered(MinigameController entered, int player)
    {
        if(player!=GetPlayerNumber()) return;
        m_PlayerInput.DeactivateInput();
        m_CameraController.SetTarget(entered.CameraTargetPosition);
    }

    private void HandleMinigameLeft(int player)
    {
        
        if(player!=GetPlayerNumber()) return;
        m_PlayerInput.ActivateInput();
        m_CameraController.SetTarget(transform);
    }

    public void SetInput(Vector2 input)
    {
        _movementInput = input;
    }


    public void OnMove(InputAction.CallbackContext context)
    {
        this._movementInput = context.ReadValue<Vector2>();
    }

    public void OnLook(InputAction.CallbackContext context)
    {
        //throw new NotImplementedException();
    }

    public void OnInteract(InputAction.CallbackContext context)
    {
        if(context.performed)
            m_InteractionComponent.ReceiveInteractionInput();
    }

    public void OnAlternateInteract(InputAction.CallbackContext context)
    {
        //throw new NotImplementedException();
    }

    private void FixedUpdate()
    {
        _rb.velocity = _movementInput * playerSpeed;
    }
}