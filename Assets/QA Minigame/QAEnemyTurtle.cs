using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QAEnemyTurtle : MonoBehaviour
{
    [SerializeField]
    float distance = 1;
    [SerializeField]
    float gravityScale;
    [SerializeField]
    float velocity;

    private Vector2 startPos;
    private new Collider2D collider;
    private new Rigidbody2D rigidbody;
    private bool dead = false;

    private void Awake()
    {
        collider = GetComponent<Collider2D>();
        startPos = transform.position;
        rigidbody = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        if (startPos.x + distance < transform.position.x)
        {
            rigidbody.velocity = new Vector2(-velocity, 0);
        }
        else if (startPos.x > transform.position.x)
        {
            rigidbody.velocity = new Vector2(velocity, 0);
        }
        if (transform.position.y < -10)
        {
            rigidbody.gravityScale = 0;
            rigidbody.velocity = Vector2.zero;
        }
    }

    private void OnEnable()
    {
        Restart();
    }

    public void Restart()
    {
        dead = false;
        collider.enabled = true;
        transform.position = startPos;
        rigidbody.velocity = new Vector2(velocity, 0);
        rigidbody.gravityScale = 0;
    }

    public void Death()
    {
        dead = true;
        collider.enabled = false;
        rigidbody.gravityScale = gravityScale;
    }
}
