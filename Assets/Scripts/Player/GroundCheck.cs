using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundCheck : MonoBehaviour
{
    public bool playerIsGrounded = false;
    [SerializeField] private LayerMask woodLayer;
    [SerializeField] private LayerMask whiteLayer;
    [SerializeField] private LayerMask bounceLayer;

    public bool isWoodLayer;
    public bool isWhiteLayer;
    public bool isBounceLayer;
    public ParticleManager particleManager;
    private Rigidbody2D playerRigidbody;
    public Player player;


    private void Awake(){
        playerRigidbody = player.GetComponent<Rigidbody2D>();
    }
        
    private void OnTriggerStay2D(Collider2D other)
    {
        int layer = other.gameObject.layer;
        SetTouchFlags(layer);
        playerIsGrounded = isWoodLayer || isWhiteLayer || isBounceLayer;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        
        if(playerRigidbody.velocity.y < -6){
            int layer = other.gameObject.layer;
            SetTouchFlags(layer);
            particleManager.InstantiateGroundParticles();
        }
    }


    private void SetTouchFlags(int layer)
    {
        // For each layer, return true if touching it
        isWoodLayer = CheckLayer(layer, woodLayer);
        isWhiteLayer = CheckLayer(layer, whiteLayer);
        isBounceLayer = CheckLayer(layer, bounceLayer);
    }
    
    private bool CheckLayer(int layer, LayerMask targetLayer)
    {
        // Checks if layer int matches layer target
        return ((1 << layer) & targetLayer) != 0;
    }


    private void OnTriggerExit2D(Collider2D other)
    {
        playerIsGrounded = false;
    }


}
