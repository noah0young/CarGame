using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Coin : MonoBehaviour
{
    public int score;
    public GameObject coinSound;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            GameManager.AddToScore(score);
            Instantiate(coinSound).transform.position = transform.position;
            Destroy(gameObject);
        }
    }
}
