using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerBoxCollider : MonoBehaviour
{
    public Color color = Color.red;
    private void OnDrawGizmos()
    {
        Gizmos.color = color;
        Gizmos.DrawWireCube(transform.position, transform.lossyScale);
    }
}
