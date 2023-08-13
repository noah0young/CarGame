using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicCar : MonoBehaviour
{
    protected Rigidbody2D myRigidbody;
    [SerializeField] private float startSpeed = 3;
    [Header("Explosion")]
    [SerializeField] private Sprite destroyedSprite;
    [SerializeField] private GameObject explosionPreFab;
    [SerializeField] private float timeUntilExplosion = .1f;
    [SerializeField] private bool canExplode = true;
    [SerializeField] private bool isBombCar = false;
    [SerializeField] private bool isWall = false;
    [Header("Car Sway")]
    [SerializeField] private float speedSway = 0;
    [SerializeField] private float timeOffset = 0;
    private float bumpSpeed = .3f;
    private bool bumped = false;
    private float timePassed = 0;

    // Start is called before the first frame update
    private void Start()
    {
        bumped = false;
        timePassed += timeOffset;
        Init();
    }

    private void Init()
    {
        if (myRigidbody == null)
        {
            myRigidbody = GetComponent<Rigidbody2D>();
            myRigidbody.velocity = new Vector2(-startSpeed, 0);
        }
    }

    private void Update()
    {
        timePassed += Time.deltaTime;
        float curSway = Mathf.Sin(timePassed);
        myRigidbody.velocity += new Vector2(curSway * speedSway * Time.deltaTime, 0);
        if (bumped)
        {
            myRigidbody.velocity += new Vector2(0, bumpSpeed * speedSway * Time.deltaTime);
        }
    }

    public void SetScreenScrollSpeed(float speed)
    {
        Init();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.transform.CompareTag("Bump"))
        {
            bumped = true;
        }
        else if (collision.CompareTag("CarDespawner"))
        {
            Destroy(gameObject);
        }
        else if (collision.CompareTag("MovingPlatform") && isWall)
        {
            collision.GetComponent<BasicCar>().WallExplode();
        }
        else if (collision.CompareTag("MovingPlatform") && canExplode)
        {
            StartCoroutine(Explode());
        }
        else if (collision.transform.CompareTag("Player") && isBombCar)
        {
            StartCoroutine(Explode());
        }
    }

    private void WallExplode()
    {
        StartCoroutine(Explode());
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.transform.CompareTag("Player") && isBombCar)
        {
            StartCoroutine(Explode());
        }
    }

    private IEnumerator Explode()
    {
        GetComponentInChildren<Animator>().StopPlayback();
        GetComponentInChildren<SpriteRenderer>().sprite = destroyedSprite;
        yield return new WaitForSeconds(timeUntilExplosion);
        GameObject explosion = Instantiate(explosionPreFab);
        explosion.transform.position = transform.position;
        explosion.GetComponent<Rigidbody2D>().velocity = myRigidbody.velocity;
        Destroy(gameObject);
    }
}
