using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;


public class PlayerController : MonoBehaviour
{
    [SerializeField] private float playerSpeed = 20;
    private Rigidbody2D rb;
    private Vector2 movementInput = Vector2.zero;


    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    public void SetInput(Vector2 input)
    {
        movementInput = input;
    }


    public void OnMove(InputAction.CallbackContext context)
    {
        this.movementInput = context.ReadValue<Vector2>();
    }

    public void OnInteraction(InputAction.CallbackContext context)
    {
    }

    private void FixedUpdate()
    {
        rb.velocity = movementInput * playerSpeed;
    }
}