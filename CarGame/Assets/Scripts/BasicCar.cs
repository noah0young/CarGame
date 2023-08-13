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
    [Header("Car Sway")]
    [SerializeField] private float speedSway = 0;
    [SerializeField] private float timeOffset = 0;
    private float timePassed = 0;

    // Start is called before the first frame update
    private void Start()
    {
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
    }

    public void SetScreenScrollSpeed(float speed)
    {
        Init();
        myRigidbody.velocity = new Vector2(-speed, 0);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("CarDespawner"))
        {
            Destroy(gameObject);
        }
        else if (collision.CompareTag("MovingPlatform") && canExplode)
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
