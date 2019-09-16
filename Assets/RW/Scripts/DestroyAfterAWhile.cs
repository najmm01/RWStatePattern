using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyAfterAWhile : MonoBehaviour
{
    public float lifeTime;

    private void Start()
    {
        StartCoroutine(SelfDestruct());
    }

    IEnumerator SelfDestruct()
    {
        yield return new WaitForSeconds(lifeTime);
        Destroy(gameObject);
    }
}
