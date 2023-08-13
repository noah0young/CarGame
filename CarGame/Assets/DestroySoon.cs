using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroySoon : MonoBehaviour
{
    [SerializeField] private float destroyAfterSeconds = .1f;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(destroyAfter());
    }

    private IEnumerator destroyAfter()
    {
        yield return new WaitForSeconds(destroyAfterSeconds);
        Destroy(gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
