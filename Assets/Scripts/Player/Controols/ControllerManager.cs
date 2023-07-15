using UnityEngine;
using UnityEngine.InputSystem;

public class ControllerManager : MonoBehaviour
{
    public Vector2 move;
    public PlayerControls playerControls;
    public Vector2 input;
    public bool pause;


    public Vector2 currentInputVector;
    public Vector2 smoothInputVelocity;
    [SerializeField] private float smoothInputSpeed = .2f;

    public PauseMenu pauseMenu;

    private void Awake()
    {
        playerControls = new PlayerControls();
    }

    private void Update()
    {
        playerControls.Player.Move.performed += ctx => move = ctx.ReadValue<Vector2>();
        playerControls.Player.Move.canceled += ctx => move = Vector2.zero;

        playerControls.Player.Pause.performed += ctx => pauseMenu.PauseInput();

        input = move;
        currentInputVector = Vector2.SmoothDamp(currentInputVector, input, ref smoothInputVelocity, smoothInputSpeed);

        // Controller will never return to 0 unless we set it
        if (Mathf.Abs(currentInputVector.x) < 0.01f)
        {
            currentInputVector.x = 0f;
        }
        if (Mathf.Abs(currentInputVector.y) < 0.01f)
        {
            currentInputVector.y = 0f;
        }
    }

    public void Pause()
    {
        
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
