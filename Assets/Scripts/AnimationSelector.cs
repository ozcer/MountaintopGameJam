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
    
    // Update is called once per frame
    void Update()
    {
        bool isOnGround = Physics2D.Raycast(
            collider.bounds.center + Vector3.down * collider.bounds.extents.y, 
            Vector3.down, 
            raycastDistance, 
            groundLayer);

        animator.SetBool("Airborne", !isOnGround);
        animator.SetFloat("YSpeed", rb.velocity.y);
        if(rb.velocity.x > 0) {
        transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
        } else if(rb.velocity.x < 0) {
            transform.localScale = new Vector3(-Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
        }

    }

    void OnDrawGizmos()
    {
        Debug.DrawRay(collider.bounds.center + Vector3.down * collider.bounds.extents.y, Vector3.down * raycastDistance, Color.red);
    }
}
