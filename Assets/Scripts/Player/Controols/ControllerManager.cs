using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

public class ControllerManager : MonoBehaviour
{
    public PlayerControls playerControls;
    private Vector2 input;
    private Vector2 input2;
    public Vector2 move;
    public Vector2 aim;

    public bool mouseDown;
    public bool mouseUp;

    public Vector2 moveInputVector;
    private Vector2 smoothInputVelocity;

    public Vector2 aimInputVector;
    [SerializeField] private float smoothInputSpeed = .2f;

    public PauseMenu pauseMenu;
    public Player player;

    public Vector2 navigate;

    private bool pauseBuffer;

    private void Awake()
    {
        playerControls = new PlayerControls();
    }

    private void Update()
    {

        // ~~~ Gameplay ~~~
        var pMove = playerControls.Player.Move;
        var pAim = playerControls.Player.Aim;
        var fire = playerControls.Player.Fire;

        if (!pauseMenu.gamePaused)
        {

            // Left Stick / Keyboard
            pMove.performed += ctx => move = ctx.ReadValue<Vector2>();
            pMove.canceled += ctx => move = Vector2.zero;
            // Right Stick
            pAim.performed += ctx => aim = ctx.ReadValue<Vector2>();
            pAim.canceled += ctx => aim = Vector2.zero;
            SmoothInput();


            // Click
            fire.performed += ctx => Confirm();

            if (fire.WasReleasedThisFrame() && mouseDown && !pauseBuffer)
            {
                player.mouseUp = true;
                player.mouseDown = false;
                mouseDown = false;
            }
            else
            {
                mouseUp = false;
                player.mouseUp = false;
                pauseBuffer = false;
            }

            // Pause
            playerControls.Player.Pause.performed += ctx => pauseMenu.PauseInput();
        }
        else
        {
            pauseBuffer = true;
        }

        // ~~~ UI ~~~

        // Navigate
        playerControls.UI.Navigate.performed += ctx => navigate = ctx.ReadValue<Vector2>();

        // Confirm
        playerControls.UI.Submit.performed += ctx => Confirm();
        // Cancel
        playerControls.UI.Cancel.performed += ctx => Cancel();

    }

    private void Confirm()
    {
        if (!pauseBuffer) 
        { 
            player.mouseDown = true;
            mouseDown = true;
        }
    }

    private void Cancel()
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

    private void SmoothInput()
    {
        input = move;
        aimInputVector = aim;

        moveInputVector = Vector2.SmoothDamp(moveInputVector, input, ref smoothInputVelocity, smoothInputSpeed);
        // Controller will never return to 0 unless we set it
        if (Mathf.Abs(moveInputVector.x) < 0.01f)
        {
            moveInputVector.x = 0f;
        }
        if (Mathf.Abs(moveInputVector.y) < 0.01f)
        {
            moveInputVector.y = 0f;
        }

        // Controller will never return to 0 unless we set it
        if (Mathf.Abs(aimInputVector.x) < 0.01f)
        {
            aimInputVector.x = 0f;
        }
        if (Mathf.Abs(aimInputVector.y) < 0.01f)
        {
            aimInputVector.y = 0f;
        }
    }
}
