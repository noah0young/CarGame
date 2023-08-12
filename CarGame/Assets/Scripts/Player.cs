using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Player : MonoBehaviour
{
    [Header("Basic Movement")]
    private Rigidbody2D myRigidbody;
    private Collider2D myCollider;
    private Vector2 velocity = new Vector2(0, 0);
    protected float curAcceleration;
    [SerializeField] protected float acceleration = 1f;
    protected float curDeceleration;
    [SerializeField] protected float deceleration = .5f;
    protected float curMaxSpeed;
    [SerializeField] protected float maxSpeed = 10f;

    [Header("Basic Jumping")]
    [SerializeField] private float gravityAcc = 9.81f;
    [SerializeField] protected float jumpSpeed = 10f;
    [SerializeField] protected float minJumpSpeed = 2f;
    [SerializeField] private string jumpKey = "j";
    [SerializeField] private float jumpRaycastHeight = .6f;
    [SerializeField] private LayerMask floorMask;
    private bool jumped = false;
    [SerializeField] private float coyoteTime = .1f;
    private float remainingCoyoteTime = 0;
    [SerializeField] private float quickJumpTime = .1f;
    private float quickJumpTimeRemaining = 0; // This will automatically jump when you next reach the ground
    private bool isGameOver = false;

    [Header("On Ground")]
    private bool prevOnGround;
    protected bool onGround { get; private set; }
    private Rigidbody2D platformRigidbody;

    [Header("Tripping")]
    [SerializeField] private float tripDecrease = .3f;
    [SerializeField] private float tripTime = .5f;
    private IEnumerator tripCoroutine;

    protected void Start()
    {
        myRigidbody = GetComponent<Rigidbody2D>();
        myCollider = GetComponent<Collider2D>();
        curAcceleration = acceleration;
        curDeceleration = deceleration;
        curMaxSpeed = maxSpeed;
        tripCoroutine = null;
    }

    protected void Update()
    {
        ResetOnGround();
        TryJump();
        UpdateRemainingJumpTimers();
    }

    private void UpdateRemainingJumpTimers()
    {
        if (remainingCoyoteTime > 0)
        {
            remainingCoyoteTime -= Time.deltaTime;
        }
        if (quickJumpTimeRemaining > 0)
        {
            quickJumpTimeRemaining -= Time.deltaTime;
        }
    }

    protected void FixedUpdate()
    {
        if (!IsGameOver())
        {
            Move();
        }
    }

    protected virtual void Move()
    {
        float direction = Input.GetAxis("Horizontal");
        if (direction != 0)
        {
            // Acceleration
            velocity.x += direction * curAcceleration * Time.fixedDeltaTime;
            if (velocity.x > curMaxSpeed)
            {
                velocity.x = curMaxSpeed;
            }
            else if (velocity.x < -curMaxSpeed)
            {
                velocity.x = -curMaxSpeed;
            }
        }
        else
        {
            // Deceleration
            if (velocity.x > 0)
            {
                velocity.x -= curDeceleration * Time.fixedDeltaTime;
                if (velocity.x < 0)
                {
                    velocity.x = 0;
                }
            }
            if (velocity.x < 0)
            {
                velocity.x += curDeceleration * Time.fixedDeltaTime;
                if (velocity.x > 0)
                {
                    velocity.x = 0;
                }
            }
        }
        velocity.y -= gravityAcc * Time.deltaTime;
        if (platformRigidbody != null)
        {
            myRigidbody.velocity = velocity + platformRigidbody.velocity;
        }
        else
        {
            myRigidbody.velocity = velocity;
        }
    }

    protected void TryJump()
    {
        if (CanJump() && ShouldJump())
        {
            jumped = true;
            velocity.y = jumpSpeed;
            myRigidbody.velocity = velocity;
            StartCoroutine(EndJump());
        }
    }

    private IEnumerator EndJump()
    {
        yield return new WaitUntil(() => ReleasedJump());
        // This lets the player end their jump early and have a shorter jump
        if (velocity.y > minJumpSpeed)
        {
            velocity = new Vector2(velocity.x, minJumpSpeed);
        }
    }

    protected virtual bool CanJump()
    {
        if (!IsGameOver())
        {
            return onGround || remainingCoyoteTime > 0;
        }
        return false;
    }

    protected bool PressedJump()
    {
        return Input.GetKeyDown(jumpKey);
    }

    protected bool ShouldJump()
    {
        return PressedJump() || quickJumpTimeRemaining > 0;
    }

    protected bool ReleasedJump()
    {
        return !Input.GetKey(jumpKey);
    }
    protected bool IsGameOver()
    {
        if (isGameOver)
        {
            return true;
        }
        return false;
    }

    protected void ResetOnGround()
    {
        prevOnGround = onGround;
        onGround = Physics2D.Raycast(transform.position, Vector2.down, jumpRaycastHeight, floorMask)
            || Physics2D.Raycast(transform.position + new Vector3(myCollider.bounds.min.x - myCollider.bounds.center.x, 0), Vector2.down, jumpRaycastHeight, floorMask)
            || Physics2D.Raycast(transform.position + new Vector3(myCollider.bounds.max.x - myCollider.bounds.center.x, 0), Vector2.down, jumpRaycastHeight, floorMask);
        if (onGround)
        {
            jumped = false;
            if (velocity.y < 0)
            {
                velocity.y = 0;
            }
        }
        else if (!jumped && !onGround && prevOnGround)
        {
            remainingCoyoteTime = coyoteTime;
        }
        else if (!onGround && PressedJump())
        {
            quickJumpTimeRemaining = quickJumpTime;
        }
    }

    protected virtual IEnumerator Trip()
    {
        Debug.Log("Tripped");
        curAcceleration = acceleration - tripDecrease;
        curDeceleration = deceleration - tripDecrease;
        curMaxSpeed = maxSpeed - tripDecrease;
        yield return new WaitForSeconds(tripTime);
        curAcceleration = acceleration;
        curDeceleration = deceleration;
        curMaxSpeed = maxSpeed;
        tripCoroutine = null;
        Debug.Log("Tripped End");
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Trip"))
        {
            if (tripCoroutine != null)
            {
                StopCoroutine(tripCoroutine);
                tripCoroutine = null;
            }
            tripCoroutine = Trip();
            StartCoroutine(tripCoroutine);
        }
        if (collision.CompareTag("Game Over"))
        {
            Debug.Log("Game Over");
            isGameOver = true;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        Debug.Log("colliding with " + collision.transform.name + ", with tag = " + collision.transform.tag);
        if (collision.transform.CompareTag("MovingPlatform"))
        {
            platformRigidbody = collision.gameObject.GetComponent<Rigidbody2D>();
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.transform.CompareTag("MovingPlatform") && platformRigidbody == collision.gameObject.GetComponent<Rigidbody2D>())
        {
            platformRigidbody = null;
        }
    }
}
