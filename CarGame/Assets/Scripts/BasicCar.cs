using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicCar : MonoBehaviour
{
    protected Rigidbody2D myRigidbody;
    [SerializeField] private float startSpeed = 3;

    // Start is called before the first frame update
    private void Start()
    {
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
    }
}
