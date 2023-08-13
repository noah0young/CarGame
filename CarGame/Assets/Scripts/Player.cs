using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Player : MonoBehaviour
{
    [SerializeField] private AudioSource mainMusic;
    [SerializeField] private float musicVolume = .3f;
    [SerializeField] private float deadVolume = .1f;
    [SerializeField] private AudioSource deathSource;
    [SerializeField] private AudioSource boingSource;
    private Animator myAnim;
    [SerializeField] private Transform model;
    private Collider2D myCollider;
    [SerializeField] private Sprite deadSprite;
    [SerializeField] private Vector2 explosionSpeed = new Vector2(5, 10);
    [SerializeField] private float explosionNoMoveTime = .3f;
    [Header("Basic Movement")]
    private bool canMove = true;
    private Rigidbody2D myRigidbody;
    private Vector2 velocity = new Vector2(0, 0);
    protected float curAcceleration;
    [SerializeField] protected float acceleration = 1f;
    protected float curDeceleration;
    [SerializeField] protected float deceleration = .5f;
    protected float curMaxSpeed;
    [SerializeField] protected float maxSpeed = 10f;

    [Header("Basic Jumping")]
    [SerializeField] private float trampoleenSpeedBoost = 10f;
    [SerializeField] private float gravityAcc = 9.81f;
    [SerializeField] protected float jumpSpeed = 10f;
    [SerializeField] protected float minJumpSpeed = 2f;
    [SerializeField] private List<string> jumpKeys;
    //[SerializeField] private float jumpRaycastHeight = 1.1f;
    [SerializeField] private JumpCollider belowCollider;
    [SerializeField] private JumpCollider overlapCollider;
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
    private float platformRaycastHeight = 1.1f;

    [Header("Game Over")]
    [SerializeField] private GameObject gameOverUI;

    protected void Start()
    {
        mainMusic.volume = musicVolume;
        canMove = true;
        Time.timeScale = 1;
        myCollider = GetComponent<Collider2D>();
        myAnim = GetComponentInChildren<Animator>();
        myRigidbody = GetComponent<Rigidbody2D>();
        curAcceleration = acceleration;
        curDeceleration = deceleration;
        curMaxSpeed = maxSpeed;
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
        if (!IsGameOver() && canMove)
        {
            Move();
        }
    }

    protected virtual void Move()
    {
        float direction = Input.GetAxis("Horizontal");
        model.localScale = new Vector3(Mathf.Abs(model.localScale.x), model.localScale.y, model.localScale.z);
        if (direction != 0)
        {
            if (direction < 0)
            {
                model.localScale = new Vector3(-Mathf.Abs(model.localScale.x), model.localScale.y, model.localScale.z);
            }
            myAnim.SetBool("moving", true);
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
            myAnim.SetBool("moving", false);
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
            myAnim.SetBool("falling", false);
            jumped = true;
            velocity.y = jumpSpeed;
            myRigidbody.velocity = velocity;
            StartCoroutine(EndJump());
        }
        if (velocity.y < 0)
        {
            myAnim.SetBool("falling", true);
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
        foreach (string key in jumpKeys)
        {
            if (Input.GetKeyDown(key))
                return true;
        }
        return false;
    }

    protected bool ShouldJump()
    {
        return PressedJump() || quickJumpTimeRemaining > 0;
    }

    protected bool ReleasedJump()
    {
        foreach (string key in jumpKeys)
        {
            if (Input.GetKey(key))
                return false;
        }
        return true;
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
        /*if (overlapCollider.NumColliding() <= 0)
        {
            onGround = (Physics2D.Raycast(transform.position, Vector2.down, jumpRaycastHeight, floorMask)
                || Physics2D.Raycast(transform.position + new Vector3(myCollider.bounds.min.x - myCollider.bounds.center.x, 0), Vector2.down, jumpRaycastHeight, floorMask)
                || Physics2D.Raycast(transform.position + new Vector3(myCollider.bounds.max.x - myCollider.bounds.center.x, 0), Vector2.down, jumpRaycastHeight, floorMask))
                && velocity.y <= 0;
        }
        else
        {
            onGround = false;
        }*/
        onGround = belowCollider.NumColliding() - overlapCollider.NumColliding() > 0;
        SetPlatformRigidbody();
        myAnim.SetBool("inAir", !onGround);
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

    private void SetPlatformRigidbody()
    {
        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down, platformRaycastHeight, floorMask);
        RaycastHit2D hit2 = Physics2D.Raycast(transform.position + new Vector3(myCollider.bounds.min.x - myCollider.bounds.center.x, 0), Vector2.down, platformRaycastHeight, floorMask);
        RaycastHit2D hit3 = Physics2D.Raycast(transform.position + new Vector3(myCollider.bounds.max.x - myCollider.bounds.center.x, 0), Vector2.down, platformRaycastHeight, floorMask);
        try
        {
            platformRigidbody = hit.collider.GetComponent<Rigidbody2D>();
        }
        catch
        {
            platformRigidbody = null;
        }
        if (platformRigidbody == null)
        {
            try
            {
                platformRigidbody = hit2.collider.GetComponent<Rigidbody2D>();
            }
            catch
            {
                platformRigidbody = null;
            }
            if (platformRigidbody == null)
            {
                try
                {
                    platformRigidbody = hit3.collider.GetComponent<Rigidbody2D>();
                }
                catch
                {
                    platformRigidbody = null;
                }
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Game Over"))
        {
            OnGameOver();
        }
        else if (collision.CompareTag("Explosion"))
        {
            Vector2 explosionCenter = collision.bounds.center;
            //float explosionRadius = collision.bounds.max.x - explosionCenter.x;
            Vector2 closestPoint = myCollider.ClosestPoint(explosionCenter);
            //float distance = Vector2.Distance(closestPoint, explosionCenter);
            Vector2 direction = -closestPoint.normalized;
            StartCoroutine(ApplyExplosion(1, direction));
        }
        else if (collision.CompareTag("Trampoleen"))
        {
            boingSource.Play();
            velocity.y = trampoleenSpeedBoost;
            myRigidbody.velocity = new Vector2(myRigidbody.velocity.x, trampoleenSpeedBoost);
        }
    }

    private IEnumerator ApplyExplosion(float percent, Vector2 direction)
    {
        Vector3 trueExplosionApplied = explosionSpeed;
        if (direction.x > -.3f)
        {
            trueExplosionApplied.x = -trueExplosionApplied.x;
        }
        velocity = trueExplosionApplied;
        myRigidbody.velocity = trueExplosionApplied;
        canMove = false;
        yield return new WaitForSeconds(explosionNoMoveTime);
        canMove = true;
    }

    private void OnGameOver()
    {
        myAnim.SetBool("dead", true);
        velocity = Vector2.zero;
        myRigidbody.velocity = Vector2.zero;
        mainMusic.volume = deadVolume;
        deathSource.Play();
        Time.timeScale = 0;
        gameOverUI.SetActive(true);
        isGameOver = true;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.transform.CompareTag("MovingPlatform"))
        {
            platformRigidbody = collision.gameObject.GetComponent<Rigidbody2D>();
        }
        else if (collision.transform.CompareTag("Game Over"))
        {
            OnGameOver();
        }
    }

    /*private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.transform.CompareTag("MovingPlatform") && platformRigidbody == collision.gameObject.GetComponent<Rigidbody2D>())
        {
            platformRigidbody = null;
        }
    }*/
}
