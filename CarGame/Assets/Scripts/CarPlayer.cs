using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class CarPlayer : MonoBehaviour
{
    
    [Header("Basic Movement")]
    private Rigidbody2D myRigidbody;
    [SerializeField] protected float acceleration = 1f;
    [SerializeField] protected float deceleration = .5f;
    [SerializeField] protected float maxSpeed = 10f;
    [SerializeField] protected float jumpSpeed = 10f;
    [Header("Basic Jumping")]
    [SerializeField] private string jumpKey = "j";
    [SerializeField] private float jumpRaycastHeight = .6f;
    [SerializeField] private LayerMask floorMask;

    // Start is called before the first frame update
    protected void Start()
    {
        myRigidbody = GetComponent<Rigidbody2D>();
    }

    protected void FixedUpdate()
    {
        Move();
        TryJump();
    }

    protected void Move()
    {
        float direction = Input.GetAxis("Horizontal");
        Vector2 velocity = myRigidbody.velocity;
        if (direction != 0)
        {
            // Acceleration
            velocity.x += direction * acceleration * Time.fixedDeltaTime;
            if (velocity.x > maxSpeed)
            {
                velocity.x = maxSpeed;
            }
            else if (velocity.x < -maxSpeed)
            {
                velocity.x = -maxSpeed;
            }
        }
        else
        {
            // Deceleration
            if (velocity.x > 0)
            {
                velocity.x -= deceleration * Time.fixedDeltaTime;
                if (velocity.x < 0)
                {
                    velocity.x = 0;
                }
            }
            if (velocity.x < 0)
            {
                velocity.x += deceleration * Time.fixedDeltaTime;
                if (velocity.x > 0)
                {
                    velocity.x = 0;
                }
            }
        }
        myRigidbody.velocity = velocity;
    }

    protected void TryJump()
    {
        if (CanJump() && Input.GetKeyDown(jumpKey))
        {
            Vector2 velocity = myRigidbody.velocity;
            velocity.y = jumpSpeed;
            myRigidbody.velocity = velocity;
        }
    }

    protected bool CanJump()
    {
        return Physics2D.Raycast(transform.position, Vector2.down, jumpRaycastHeight, floorMask);
    }
}
