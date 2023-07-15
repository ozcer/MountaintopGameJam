using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class ControllerManager : MonoBehaviour
{
    public InputAction PlayerControls;
    public Vector2 move;
    public PlayerControls playerControls;
    public Vector2 input;

    public Vector2 currentInputVector;
    private Vector2 smoothInputVelocity;
    [SerializeField] private float smoothInputSpeed = .2f;

    private void Awake()
    {
        playerControls = new PlayerControls();
    }

    private void Update()
    {
        playerControls.Player.Move.performed += ctx => move = ctx.ReadValue<Vector2>();
        playerControls.Player.Move.canceled += ctx => move = Vector2.zero;

        input = move;
        currentInputVector = Vector2.SmoothDamp(currentInputVector, input, ref smoothInputVelocity, smoothInputSpeed);
            
    }

    private void OnEnable()
    {
        playerControls.Player.Enable();
    }

    private void OnDisable()
    {
        playerControls.Player.Disable();
    }
}
