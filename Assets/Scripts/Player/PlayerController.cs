using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;


public class PlayerController : MonoBehaviour//, InputActions.IPlayerActions
{
    public bool leftPlayer;
    public int playerIndex;
    public static PlayerController Player1;
    public static PlayerController Player2;
    public static bool IsAnyInMinigame()=>
        Player1._isInMinigame || Player2._isInMinigame;

    public bool _isInMinigame
    {
        get;
        private set;
    }
    
    [SerializeField] private float playerSpeed;
    [SerializeField] private string inputControllScheme;
    [SerializeField] private bool keyboard;

    [Header("Refs")] 
    [SerializeField] private Transform m_InitialPos;
    public Transform InitialPos => m_InitialPos;
    public MainProgressBarSnap progressBar;

    private static Vector2 GetStartingPos(PlayerController player)
    {
        //yes this intentional. Do. Not. Fucking. Ask.
        if (player == Player1)
        {
            return Player2.InitialPos.position;
        }
        else
        {
            return Player1.InitialPos.position;
        }
    }
    [SerializeField] private PlayerInteractionComponent m_InteractionComponent;
    [SerializeField] private PlayerInput m_PlayerInput;
    public PlayerInput PlayerInput => m_PlayerInput;
    //[SerializeField] private PlayerCameraController m_CameraPrefab;
    [SerializeField] private PlayerCameraController m_CameraController;
    public PlayerCameraController GetCameraController() => m_CameraController;
    
    private Rigidbody2D _rb;
    private Vector2 _movementInput = Vector2.zero;

    private bool _isWalking = false;

    public InputAction minigameMoveAction { get; private set; }
    public List<InputAction> minigameButtonsActions { get; private set; }

    public InputAction minigameNavigationAction { get; private set; }

    public delegate void minigameButtonClickedAction(int buttonIndex, InputAction.CallbackContext callbackContext);
    public minigameButtonClickedAction onMinigameButtonClicked;

    private void Awake()
    {
        if (keyboard)
        {
            m_PlayerInput.SwitchCurrentControlScheme(inputControllScheme, Keyboard.current);
        }
        InputActionMap miniGameActionMap = PlayerInput.actions.FindActionMap("MiniGame");
        minigameMoveAction = miniGameActionMap.FindAction("Move");
        minigameNavigationAction = miniGameActionMap.FindAction("Navigate");
        Debug.Log(minigameNavigationAction);
        minigameButtonsActions = new List<InputAction>()
        { 
            miniGameActionMap.FindAction("WestButton"),
            miniGameActionMap.FindAction("NorthButton"),
            miniGameActionMap.FindAction("SouthButton"),
            miniGameActionMap.FindAction("EastButton")
        };
        //for (int buttonIndex = 0; buttonIndex < 4; buttonIndex++)
        //{
        //    minigameButtonsActions[buttonIndex].performed += ((callbackContext) => onMinigameButtonClicked?.Invoke(buttonIndex, callbackContext));
        //}
        minigameButtonsActions[0].performed += ((callbackContext) => onMinigameButtonClicked?.Invoke(0, callbackContext));
        minigameButtonsActions[1].performed += ((callbackContext) => onMinigameButtonClicked?.Invoke(1, callbackContext));
        minigameButtonsActions[2].performed += ((callbackContext) => onMinigameButtonClicked?.Invoke(2, callbackContext));
        minigameButtonsActions[3].performed += ((callbackContext) => onMinigameButtonClicked?.Invoke(3, callbackContext));

        this.playerSpeed = 4;
        //PlayerInputManager.instance.JoinPlayer()
        if (Player1 == null)
        {
            Player1=this;
            //InputManager.Player1InputActions.Enable();
            //InputManager.Player2InputActions.Player.RemoveCallbacks(Player2);
            //InputManager.Player1InputActions.Player.SetCallbacks(this);
        }
        else if (Player2 == null)
        {
            Player2=this;
            //InputManager.Player2InputActions.Enable();
            //InputManager.Player2InputActions.Player.RemoveCallbacks(Player1);
            //InputManager.Player2InputActions.Player.SetCallbacks(this);
        }
        else
            Debug.LogError("More than 2 players in scene");
        
        _rb = GetComponent<Rigidbody2D>();
        
        //MinigamesManager.minigameEnterEvent += HandleMinigameEntered;
        //MinigamesManager.minigameLeftEvent += HandleMinigameLeft;
    }

    private void Start()
    {
        if (leftPlayer)
        {
            m_CameraController.SetCameraPosition(0.5f);
        }
        else
        {
            m_CameraController.SetCameraPosition(0);
        }

        //m_CameraController=Instantiate(m_CameraPrefab, transform.position, Quaternion.identity);
        //m_PlayerInput.camera=m_CameraController.GetComponent<Camera>();
        m_CameraController.SetTarget(transform);
        m_PlayerInput.actions.FindActionMap("Player").FindAction("Move").performed += OnMove;
        
        m_PlayerInput.actions.FindActionMap("Player").FindAction("Move").canceled += OnMove;
        
        m_PlayerInput.actions.FindActionMap("Player").FindAction("Move").started += OnMove;
        
        m_PlayerInput.actions.FindActionMap("Player").FindAction("Interact").performed += OnInteract;

        //transform.position = GetStartingPos(this);
    }
    

    private void OnDestroy()
    {
        //MinigamesManager.minigameEnterEvent -= HandleMinigameEntered;
        //MinigamesManager.minigameLeftEvent -= HandleMinigameLeft;
    }

    public void OnPVPMinigameEntered(PVPMinigameController minigame)
    {
        m_PlayerInput.SwitchCurrentActionMap("MiniGame");
        _isInMinigame = true;
    }

    public void OnPVPMinigameEnd()
    {
        _isInMinigame = false;
        m_PlayerInput.SwitchCurrentActionMap("Player");
        m_CameraController.ResetCamera();
        m_CameraController.SetTarget(transform);
    }

    public void HandleMinigameEntered(MinigameController entered)
    {
        _isInMinigame = true;
        //Debug.Log($"Game entered event receiving on {GetPlayerNumber()} player with {byPlayer} byPlayerid");
        m_PlayerInput.SwitchCurrentActionMap("MiniGame");
        //m_PlayerInput.DeactivateInput();
        //m_CameraController.SetTarget(entered.CameraConfig.CameraTarget);
        m_CameraController.SetConfig(entered.CameraConfig);
    }

    public void HandleMinigameLeft()
    {
        _isInMinigame = false;
        //Debug.Log($"Game left event receiving on {GetPlayerNumber()} player with {player} id");
        m_PlayerInput.SwitchCurrentActionMap("Player");
        //m_PlayerInput.ActivateInput();
        m_CameraController.ResetCamera();
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

    private void Update()
    {
        this._isWalking = this._movementInput != Vector2.zero;
    }

    private void FixedUpdate()
    {
        _rb.velocity = _movementInput * playerSpeed;
    }

    public bool GetIsWalking()
    {
        return _isWalking;
    }

    public Vector2 GetMovementInput()
    {
        return this._movementInput;
    }
}