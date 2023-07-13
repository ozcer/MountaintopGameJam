using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundCheck : MonoBehaviour
{
    public bool isGrounded = false;
    [SerializeField] private LayerMask platform1;
    [SerializeField] private LayerMask platform2;
    [SerializeField] private LayerMask platform3;

    private void OnTriggerStay2D(Collider2D other) {
        int layer = other.gameObject.layer;
        if (((1 << layer) & platform1) != 0 || ((1 << layer) & platform2) != 0 || ((1 << layer) & platform3) != 0) {
            isGrounded = true;
        }
    }

    private void OnTriggerExit2D(Collider2D other) {
        isGrounded = false;
    }
}
