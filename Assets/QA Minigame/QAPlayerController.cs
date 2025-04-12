using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using System;

public class QAPlayerController : MonoBehaviour, InputActions.IQAPlatformerActions
{
    [SerializeField]
    private float jumpForce = 5;
    [SerializeField]
    private float maxSpeed = 10;
    [SerializeField]
    private float acceleration = 20f;
    [SerializeField]
    private Transform groundCheckTransform;
    [SerializeField]
    private LayerMask groundLayerMask;
    [SerializeField]
    private float groundCheckDistance = 0.1f;
    [SerializeField]
    private float minJumpWaitTime = 0.2f;
    [SerializeField]
    private string finishTag;
    [SerializeField]
    private string hazardTag;

    public Action onFinishEnter;
    public Action onHazardEnter;

    private float lastJumpTime = -1;
    private new Rigidbody2D rigidbody;
    private float horizontalMovement;

    private void Awake()
    {
        //InputManager.inputActions.QAPlatformer.Enable();
        //InputManager.inputActions.QAPlatformer.SetCallbacks(this);
        rigidbody = GetComponent<Rigidbody2D>();
    }

    private void FixedUpdate()
    {
        float velocityDamping = maxSpeed / (maxSpeed + acceleration * Time.fixedDeltaTime);
        rigidbody.velocity += Vector2.right * horizontalMovement * acceleration * Time.fixedDeltaTime;
        rigidbody.velocity = new Vector2(rigidbody.velocity.x * velocityDamping, rigidbody.velocity.y);
    }

    public void ResetVelocity()
    {
        rigidbody.velocity = Vector2.zero;
    }

    private void Jump()
    {
        rigidbody.velocity = new Vector2(rigidbody.velocity.x, jumpForce);
        lastJumpTime = Time.time;
    }

    private bool GroundCheck()
    {
        return Physics2D.OverlapCircle(groundCheckTransform.position, groundCheckDistance, groundLayerMask);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(groundCheckTransform.position, groundCheckDistance);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == finishTag)
        {
            onFinishEnter?.Invoke();
        }
        if (collision.tag == hazardTag)
        {
            onHazardEnter?.Invoke();
        }
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        horizontalMovement = context.ReadValue<Vector2>().x;
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        if (GroundCheck() && rigidbody.velocity.y < 0.1f && lastJumpTime + minJumpWaitTime < Time.time)
        {
            Jump();
        }
    }
}
