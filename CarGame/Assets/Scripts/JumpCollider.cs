using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpCollider : MonoBehaviour
{
    private ISet<GameObject> colliding = new HashSet<GameObject>();

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("MovingPlatform"))
        {
            colliding.Add(collision.gameObject);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("MovingPlatform") && colliding.Contains(collision.gameObject))
        {
            colliding.Remove(collision.gameObject);
        }
    }

    public int NumColliding()
    {
        return colliding.Count;
    }
}
