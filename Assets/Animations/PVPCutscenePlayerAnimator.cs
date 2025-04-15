using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PVPCutscenePlayerAnimator : MonoBehaviour
{
    [SerializeField]
    private Animator animator;

    public bool isWalking;
    public bool movingLeft;
    public bool movingRight;

    private void Update()
    {
        animator.SetBool("IsWalking", isWalking);
        animator.SetBool("Moving_Left", movingLeft);
        animator.SetBool("Moving_Right", movingRight);
    }
}
