using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyAfterDelay : MonoBehaviour
{
    public float delay = 1;

    private float destroyTime;

    private void Awake()
    {
        destroyTime = Time.time + delay;
    }

    private void Update()
    {
        if (destroyTime < Time.time)
        {
            Destroy(gameObject);
        }
    }
}
