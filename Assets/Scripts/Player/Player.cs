using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Player : MonoBehaviour
{
    private Animator m_Animator;
    private Rigidbody2D rb;
    private SpringJoint2D m_SpringJoint;
    private SpriteRenderer m_SpriteRenderer;
    public AnimationSelector animationSelector;
    
    [SerializeField]
    private GameObject m_HookPrefab;
    private GameObject m_CurrentHook;

    [SerializeField]
    public float airSpeed = 26f;
    public float airControl = 10f;
    public bool extraControl = false;
    public float speed = 10.0f;

    [SerializeField]
    private float m_MaxSpeed = 20f;
    private float moveHorizontal;
    private bool m_MovingToHook = false;
    
    [SerializeField]
    private float m_retrieveHookDistance = 1f;

    [SerializeField] private LayerMask platformLayer;
    
    [Header("Charging")]
    public float launchPowerMin = 10f;
    public float launchPowerMax = 50f;
    public float launchPowerIncrement = 1f;
    private float launchPower = 0f;
    public bool mouseDown;
    public bool mouseUp;

    private Vector2 aim;
    public bool invertDirection = false;    // Change in options

    [Header("Gliding")]
    public float maxGlideFrames = 1200f;
    public float glideFramesRemaining;
    public bool glideDepleted = false;
    public bool gliding = false;

    [Header("UI")]
    public float chargePercent;
    public ChargeBar chargeBar;
    public bool displayGlideUI, displayChargeUI;

    [Header("Recall Logic")]
    public bool softlocked = false;
    public bool softlockCheckCoroutineRunning = false;
    public bool touchingHook = false;
    
    private bool wallClimbL = false;
    private bool wallClimbR = false;

    private Vector3 originalPosition;

    [SerializeField]
    private FeatherParticles m_FeatherParticles;

    [SerializeField]
    private GameObject m_SmokeParticlesPrefab;
    private int m_FramesBetweenSmoke = 10;
    private int m_SmokeFramesRemaining = 10;
    
    public bool useGlideOverride = false;
    public bool glideOverride = false;

    public GroundCheck groundScript;

    public bool stickAim;
    private Vector2 stickSave;
    

    [Header("Bounce Timer")]
    [SerializeField]
    private float currentClamp = 20f;
    public float minClamp;
    public float maxClamp;
    public float clampInterval;

    public PauseMenu pauseMenu;

    private ControllerManager controllerManager;
    
    private void Awake()
    {
        m_Animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        m_SpringJoint = GetComponent<SpringJoint2D>();
        m_SpriteRenderer = GetComponent<SpriteRenderer>();
        controllerManager = FindObjectOfType<ControllerManager>();

        launchPower = launchPowerMin;

        glideFramesRemaining = maxGlideFrames;
        originalPosition = transform.position;
    }

    private void Update()
    {
        if (mouseUp || (stickAim && aim == Vector2.zero))
        {
            if (m_CurrentHook == null && m_HookPrefab)
            {
                LaunchGrapplingHook(Mathf.Max(launchPower, launchPowerMin));
            }
            else
            {
                RecallLogic();
            }
            launchPower = launchPowerMin;
            stickAim = false;
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            transform.position = originalPosition;
        }

        
        if (Input.GetKeyDown(KeyCode.U))
        {
            Vector3 newPosition = transform.position + Vector3.up * 10f;
            transform.position = newPosition;
        }
        
        FaceMouse();
        
        GlideLogic();

        if (useGlideOverride)
        {
            m_Animator.SetBool("Gliding", glideOverride);
        }
    }

    private void FixedUpdate()
    {
        moveHorizontal = controllerManager.moveInputVector.x;
        aim = controllerManager.aimInputVector;

        //Reduce maximum clamp after a bounce
        if(currentClamp > minClamp){
            currentClamp -= clampInterval;
        }
        // Clamp speed
        rb.velocity = new Vector2(Mathf.Clamp(rb.velocity.x, -m_MaxSpeed, m_MaxSpeed), Mathf.Clamp(rb.velocity.y, -currentClamp, currentClamp));        

        // If touching hook
        if (m_MovingToHook && Vector2.Distance((Vector2)m_CurrentHook.transform.position, (Vector2)transform.position) < m_retrieveHookDistance)
        {
            touchingHook = true;
        }
        
        if (m_CurrentHook == null)
        {
            m_Animator.SetBool("Charging", mouseDown);

            if (mouseDown || aim != Vector2.zero)
            {
                if (!displayChargeUI) //set mouse down instance to trigger event that can be read by UIHandler
                {
                    SoundManager.Instance.PlaySound(Sound.Charge);
                    displayChargeUI = true;
                }

                if (launchPower < launchPowerMax)
                {
                    launchPower += launchPowerIncrement;

                    chargePercent =  (launchPower - launchPowerMin) / (launchPowerMax - launchPowerMin);

                    chargeBar.SetValue(chargePercent);
                    
                }
                if(aim != Vector2.zero)
                {
                    stickAim = true;
                    if(aim.magnitude > .95)
                    {
                        stickSave = aim;
                    }
                }
            }
        }


        Vector2 movement = new Vector2(moveHorizontal, 0); 
        
        // If hook is below player or wall climbing, disable movement
        if (softlockCheckCoroutineRunning || ((wallClimbL && rb.velocity.x < 4) || (wallClimbR && rb.velocity.x > -4)))
        {
            return;
        }

        // Move the player
        rb.AddForce(movement * airSpeed, ForceMode2D.Force);

        if (groundScript.playerIsGrounded)
        {
            // Overwrite player velocity, IE snap turning
            rb.velocity = new Vector2(movement.x * speed, rb.velocity.y);
            SmokeEffect();
        }

        // Add more air control
        else
        {
            // Add more control while speed is less than 10

            if (moveHorizontal > 0 && rb.velocity.x < 10)
            {
                rb.AddForce(movement * airControl, ForceMode2D.Force);
            }
            else if (moveHorizontal < 0 && rb.velocity.x > -10)
            {
                rb.AddForce(movement * airControl, ForceMode2D.Force);
            }
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("WallL"))
        {
            wallClimbL = true;
        }
        if (collision.gameObject.CompareTag("WallR"))
        {
            wallClimbR = true;
        }
        if (collision.gameObject.layer == LayerMask.NameToLayer("BouncyWall"))
        {
            SoundManager.Instance.PlaySound(Sound.BouncyHit);
            currentClamp = maxClamp;
        }

    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("WallL"))
        {
            wallClimbL = false;
        }
        if (collision.gameObject.CompareTag("WallR"))
        {
            wallClimbR = false;
        }
    }

    private void LaunchGrapplingHook(float power)
    {
        SoundManager.Instance.PlaySound(Sound.Throw);

        m_Animator.SetBool("Charging", false);
        displayChargeUI = false;

        Vector3 worldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 targetPosition = new Vector2(worldPosition.x, worldPosition.y);

        Vector2 difference = targetPosition - (Vector2) transform.position;
        float angle = Vector2.Angle(Vector2.up, difference);


        angle = (worldPosition.x > transform.position.x) ? -angle : angle;

        Vector2 unitVector = Vector2.up;
        Quaternion rotation = Quaternion.Euler(0f, 0f, angle);

        Vector2 fireVector = rotation * unitVector;

        if (stickAim)
        {
            fireVector = stickSave;
        }

        if (invertDirection)
        {
            fireVector *= -1f; // Invert the fireVector in the Y direction if invertDirection is true
        }

        GameObject hookObject = Instantiate(m_HookPrefab, transform.position, Quaternion.identity);

        Hook hook = hookObject.GetComponent<Hook>();
        hook.Player = this;
        hook.Launch(fireVector, power);
        

        m_CurrentHook = hookObject;
        GameManager.Instance.ChangeCameraTarget(hookObject.transform);
    }

    private void DestroyGrapplingHook()
    {
        GameManager.Instance.ChangeCameraTarget(transform);

        m_SpringJoint.connectedBody = rb;
        m_MovingToHook = false;

        Destroy(m_CurrentHook);
        m_CurrentHook = null;

        touchingHook = false;
        softlocked = false;
        softlockCheckCoroutineRunning = false;
        touchingHook = false;
    }

    private void FaceMouse()
    {
        if (pauseMenu.gamePaused) return;

        Vector2 mousePositionInWorld = (Vector2)Camera.main.ScreenToWorldPoint(Input.mousePosition);

        if (rb.velocity.x != 0 && moveHorizontal != 0)
        {
            if (rb.velocity.x > 0.01)
            {
                m_SpriteRenderer.flipX = false;
            }
            if (rb.velocity.x < 0.01)
            {
                m_SpriteRenderer.flipX = true;
            }
        }


        if(mouseDown){
            if (mousePositionInWorld.x < transform.position.x){
                m_SpriteRenderer.flipX = true;
            }
            else if (mousePositionInWorld.x > transform.position.x){
                m_SpriteRenderer.flipX = false;
            }
        }
    }

    private void SmokeEffect()
    {
        if (moveHorizontal != 0)
        {
            m_SmokeFramesRemaining -= 1;

            if (m_SmokeFramesRemaining <= 0 && m_SmokeParticlesPrefab != null)
            {
                Instantiate(
                    m_SmokeParticlesPrefab,
                    new Vector3(transform.position.x, transform.position.y - 0.5f, transform.position.z),
                    Quaternion.identity);

                m_SmokeFramesRemaining = m_FramesBetweenSmoke;
            }
        }
    }

    private void GlideLogic()
    {
        if (m_MovingToHook)
        {
            return;
        }

        if (Input.GetButton("Jump") && !glideDepleted) //set glide instance to trigger event that can be read by UIHandler
        {
            m_FeatherParticles?.StartParticleSystem();

            m_Animator.SetBool("Gliding", true);
            displayGlideUI = true;

            if (rb.velocity.y < -1)
            {
                rb.velocity = new Vector2(rb.velocity.x, -1);
            }

            if (glideFramesRemaining % 200 == 0)
            {
                SoundManager.Instance.PlaySound(Sound.Flap, 2f);
            }

            glideFramesRemaining -= 1;

            if (glideFramesRemaining <= 0)
            {
                glideDepleted = true;
            }    
        }
        else
        {
            m_FeatherParticles?.StopParticleSystem();

            m_Animator.SetBool("Gliding", false);
            displayGlideUI = false;

            if (glideFramesRemaining < maxGlideFrames)
            {
                glideFramesRemaining += 1;
                if (glideFramesRemaining >= maxGlideFrames)
                {
                    glideDepleted = false;
                }
            }
        }
    }

    private void RecallLogic()
    {
        if (!m_MovingToHook)
        {
            Hook hook = m_CurrentHook.GetComponent<Hook>();
            if (hook.CanRecall())
                DestroyGrapplingHook();
        }
        else if (touchingHook || m_CurrentHook.transform.position.y >= transform.position.y || softlocked)
        {
            DestroyGrapplingHook();
        }
    }

    public void MoveToHook()
    {
        GameManager.Instance.ChangeCameraTarget(transform);
        m_SpringJoint.connectedBody = m_CurrentHook.GetComponent<Rigidbody2D>();

        m_MovingToHook = true;

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
