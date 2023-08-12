using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class CarPlayer : MonoBehaviour
{
    [Header("Basic Movement")]
    private Rigidbody2D myRigidbody;
    protected float curAcceleration;
    [SerializeField] protected float acceleration = 1f;
    protected float curDeceleration;
    [SerializeField] protected float deceleration = .5f;
    protected float curMaxSpeed;
    [SerializeField] protected float maxSpeed = 10f;
    
    [Header("Basic Jumping")]
    [SerializeField] protected float jumpSpeed = 10f;
    [SerializeField] protected float minJumpSpeed = 2f;
    [SerializeField] private string jumpKey = "j";
    [SerializeField] private float jumpRaycastHeight = .6f;
    [SerializeField] private LayerMask floorMask;
    private bool prevOnGround;
    protected bool onGround { get; private set; }
    private bool jumped = false;
    [SerializeField] private float coyoteTime = .1f;
    private float remainingCoyoteTime = 0;
    [SerializeField] private float quickJumpTime = .1f;
    private float quickJumpTimeRemaining = 0; // This will automatically jump when you next reach the ground

    [Header("Tripping")]
    [SerializeField] private float tripDecrease = .3f;
    [SerializeField] private float tripTime = .5f;
    private IEnumerator tripCoroutine;

    protected void Start()
    {
        myRigidbody = GetComponent<Rigidbody2D>();
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
        Move();
    }

    protected virtual void Move()
    {
        float direction = Input.GetAxis("Horizontal");
        Vector2 velocity = myRigidbody.velocity;
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
        myRigidbody.velocity = velocity;
    }

    protected void TryJump()
    {
        if (CanJump() && ShouldJump())
        {
            jumped = true;
            Vector2 velocity = myRigidbody.velocity;
            velocity.y = jumpSpeed;
            myRigidbody.velocity = velocity;
            StartCoroutine(EndJump());
        }
    }

    private IEnumerator EndJump()
    {
        yield return new WaitUntil(() => ReleasedJump());
        // This lets the player end their jump early and have a shorter jump
        if (myRigidbody.velocity.y > minJumpSpeed)
        {
            myRigidbody.velocity = new Vector2(myRigidbody.velocity.x, minJumpSpeed);
        }
    }

    protected virtual bool CanJump()
    {
        return onGround || remainingCoyoteTime > 0;
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

    protected void ResetOnGround()
    {
        prevOnGround = onGround;
        onGround = Physics2D.Raycast(transform.position, Vector2.down, jumpRaycastHeight, floorMask);
        if (onGround)
        {
            jumped = false;
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
    }
}
