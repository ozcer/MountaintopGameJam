using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Player : MonoBehaviour
{
    private Animator m_Animator;
    private Rigidbody2D rb;
    public SpringJoint2D springJoint;
    private SpriteRenderer m_SpriteRenderer;

    public PlayerMovement playerMovement;
    public PlayerGrappling playerGrappling;
    public AnimationSelector animationSelector;
    
    public GameObject currentHook;

    public float maxSpeed = 20f;
    public float moveHorizontal;
    public bool movingToHook = false;
    
    public float retriveHookDistance = 1f;
    
    [Header("Charging")]
    public bool mouseDown;
    public bool mouseUp;

    public bool invertDirection = false;    // Shoots opposite to mouse, Change in options


    [Header("UI")]
    public float chargePercent;
    public bool displayGlideUI, displayChargeUI;

    [Header("Recall Logic")]
    public bool softlocked = false;
    public bool softlockCheckCoroutineRunning = false;
    public bool touchingHook = false;

    private Vector3 originalPosition;

    public GroundCheck groundScript;

    public bool stickAim;
    private Vector2 stickSave;

    public PauseMenu pauseMenu;
    private ControllerManager controllerManager;

    public bool setGameFPS = false;
    public int gameFPS = 60;

    public CinemachineVirtualCamera cam;
    public float camZoomTime = .3f;


    private void Awake()
    {
        m_Animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        springJoint = GetComponent<SpringJoint2D>();
        m_SpriteRenderer = GetComponent<SpriteRenderer>();
        controllerManager = FindObjectOfType<ControllerManager>();

        playerMovement = GetComponent<PlayerMovement>();
        playerGrappling = GetComponent<PlayerGrappling>();

        originalPosition = transform.position;

        if (setGameFPS)
        {
            QualitySettings.vSyncCount = 0;  // VSync must be disabled
            Application.targetFrameRate = gameFPS;
        }

    }

    private void Update()
    {
        CheatCodes();
        FaceMouse();

        playerMovement.MovementUpdate();
        playerGrappling.GrapplingUpdate();
    }

    private void FixedUpdate()
    {
        moveHorizontal = controllerManager.moveInputVector.x;

        playerMovement.MovementFixedUpdate();
        playerGrappling.GrapplingFixedUpdate();
    }

    private void CheatCodes()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            transform.position = originalPosition;
            playerGrappling.DestroyGrapplingHook();
            rb.velocity = Vector2.zero;
        }

        if (Input.GetKeyDown(KeyCode.U))
        {
            Vector3 newPosition = transform.position + Vector3.up * 10f;
            transform.position = newPosition;
        }

        if (Input.GetKeyDown(KeyCode.I))
        {
            if (cam.m_Lens.OrthographicSize < 20)
            {
                StartCoroutine(ChangeOrthographicSize(30f, camZoomTime));
            }
            else
            {
                StartCoroutine(ChangeOrthographicSize(16f, camZoomTime));
            }
        }
    }

    private IEnumerator ChangeOrthographicSize(float targetSize, float duration)
    {
        float initialSize = cam.m_Lens.OrthographicSize;
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            float newSize = Mathf.Lerp(initialSize, targetSize, elapsedTime / duration);
            cam.m_Lens.OrthographicSize = newSize;

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        cam.m_Lens.OrthographicSize = targetSize;
    }


    private void FaceMouse()
    {
        if (pauseMenu.gamePaused) return;

        Vector2 mousePositionInWorld = (Vector2) Camera.main.ScreenToWorldPoint(Input.mousePosition);

        if (rb.velocity.x != 0 && moveHorizontal != 0)
        {
            m_SpriteRenderer.flipX = (rb.velocity.x < 0.01);
        }

        if (mouseDown){
            m_SpriteRenderer.flipX = (mousePositionInWorld.x < transform.position.x);
        }
    }

    public void MoveToHook()
    {
        GameManager.Instance.ChangeCameraTarget(transform);
        springJoint.connectedBody = currentHook.GetComponent<Rigidbody2D>();

        movingToHook = true;

        softlockCheckCoroutineRunning = true;
        StartCoroutine(SoftlockCheckCoroutine());
    }

    private IEnumerator SoftlockCheckCoroutine()
    {
        while (!softlocked && softlockCheckCoroutineRunning)
        {
            Vector2 initialPosition = transform.position;
            yield return new WaitForSeconds(1);
            if (softlockCheckCoroutineRunning)
            {
                Vector2 finalPosition = transform.position;

                if (Vector2.Distance(initialPosition, finalPosition) < 0.5f)
                {
                    softlocked = true;
                    softlockCheckCoroutineRunning = false;
                }
            }
        }
    }
}
