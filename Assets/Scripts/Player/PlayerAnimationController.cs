using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimationController : MonoBehaviour
{
    private static readonly int IsWalking = Animator.StringToHash(IS_WALKING);
    private static readonly int MovingUp = Animator.StringToHash("Moving_Up");
    private static readonly int MovingLeft = Animator.StringToHash("Moving_Left");
    private static readonly int MovingRight = Animator.StringToHash("Moving_Right");
    private static readonly int MovingDown = Animator.StringToHash("Moving_Down");
    [SerializeField] private PlayerController playerController;
    [SerializeField] private Animator animator;
    private const string IS_WALKING = "IsWalking";
    
    public enum Direction
    {
        None,
        Up,
        Down,
        Left,
        Right
    }
    
    private Direction lastDirection = Direction.None;


    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    void Start()
    {
    }


    private Direction GetDirectionFromVector(Vector2 input)
    {
        if (input == Vector2.zero)
            return Direction.None;

        if (Mathf.Abs(input.x) > Mathf.Abs(input.y))
            return input.x > 0 ? Direction.Right : Direction.Left;
        else
            return input.y > 0 ? Direction.Up : Direction.Down;
    }
    
    
    private string GetAnimatorBoolName(Direction direction)
    {
        return direction switch
        {
            Direction.Up => "Moving_Up",
            Direction.Down => "Moving_Down",
            Direction.Left => "Moving_Left",
            Direction.Right => "Moving_Right",
            _ => null
        };
    }

    // Update is called once per frame
    void Update()
    {
        bool isWalking = playerController.GetIsWalking();
        animator.SetBool(IsWalking, isWalking);

        if (!isWalking)
        {
            // Resetujemy tylko jeśli przestaliśmy chodzić
            ResetDirectionBooleans();
            lastDirection = Direction.None;
            return;
        }

        Direction currentDirection = GetDirectionFromVector(playerController.GetMovementInput());

        // Jeśli kierunek się nie zmienił — nic nie robimy
        if (currentDirection == lastDirection)
            return;

        // Resetujemy tylko, jeśli kierunek się zmienił
        ResetDirectionBooleans();

        string boolName = GetAnimatorBoolName(currentDirection);
        if (!string.IsNullOrEmpty(boolName))
            animator.SetBool(boolName, true);

        lastDirection = currentDirection;
    }

    
    private void ResetDirectionBooleans()
    {
        animator.SetBool("Moving_Up", false);
        animator.SetBool("Moving_Down", false);
        animator.SetBool("Moving_Left", false);
        animator.SetBool("Moving_Right", false);
    }


}