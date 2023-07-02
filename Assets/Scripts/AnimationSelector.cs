using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationSelector : MonoBehaviour
{
    [SerializeField] Animator animator;
    [SerializeField] Collider2D collider;
    [SerializeField] Rigidbody2D rb;
    public Vector2 jumpForce = new Vector2(0, 10); 
    public float raycastDistance = 0.1f;
    public LayerMask groundLayer;
    public bool left;
    
    public bool useYSpeedBelowZeroOverride = false;
    public bool ySpeedBelowZeroOverride = false;
    
    void Update()
    {
        if (useYSpeedBelowZeroOverride)
        {
            animator.SetBool("YSpeedBelowZero", ySpeedBelowZeroOverride);
        }
        else
        {
            animator.SetBool("YSpeedBelowZero", rb.velocity.y < 0);
        }
        animator.SetBool("XSpeedNonZero", rb.velocity.x != 0);
    }

    void OnDrawGizmos()
    {
        Debug.DrawRay(collider.bounds.center + Vector3.down * collider.bounds.extents.y, Vector3.down * raycastDistance, Color.red);
    }
}
