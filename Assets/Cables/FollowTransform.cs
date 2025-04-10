using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowTransform : MonoBehaviour
{
    public Transform targetToFollow;

    private new Transform transform;

    private void Awake()
    {
        transform = GetComponent<Transform>();
    }

    private void LateUpdate()
    {
        transform.position = targetToFollow.position;
    }
}
