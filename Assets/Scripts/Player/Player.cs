using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    private float dirX = 0.2f;
    [SerializeField] private float moveSpeed = 7f;
    public float airSpeed = 26f;
    public float speed = 10.0f;

    [SerializeField]
    private float m_MaxSpeed = 20f;
    private bool m_MovingToHook = false;

    [SerializeField]
    private float m_LaunchPower = 30f;
    
    [SerializeField]
    private float m_retrieveHookDistance = 1f;
    
    [Header("Charging")]
    public float launchPowerMin = 10f;
    public float launchPowerMax = 50f;
    public float launchPowerIncrement = 1f;
    public float launchPower = 0f;

    [Header("Gliding")]
    public float maxGlideFrames = 1200f;
    public float glideFramesRemaining;
    public bool glideDepleted = false;
    public bool gliding = false;

    [Header("UI")]
    public GameObject canvasObject;
    public ChargeBar chargeBar;

    [Header("Recall Logic")]
    public bool softlocked = false;
    public bool softlockCheckCoroutineRunning = false;
    public bool touchingHook = false;
    
    private bool wallClimb = false;
    private bool grounded = false;
    private bool left;

    private Vector3 originalPosition;

    [SerializeField]
    private FeatherParticles m_FeatherParticles;

    [SerializeField]
    private GameObject m_SmokeParticlesPrefab;
    private int m_FramesBetweenSmoke = 10;
    private int m_SmokeFramesRemaining = 10;
    
    public bool useGlideOverride = false;
    public bool glideOverride = false;
    
    private void Awake()
    {
        m_Animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        m_SpringJoint = GetComponent<SpringJoint2D>();
        m_SpriteRenderer = GetComponent<SpriteRenderer>();

        glideFramesRemaining = maxGlideFrames;
        originalPosition = transform.position;

        StartCoroutine(QuackCoroutine());
    }

    private void Update()
    {
        if (Input.GetMouseButtonUp(0))
        {
            if (m_CurrentHook == null && m_HookPrefab)
            {
                LaunchGrapplingHook(Mathf.Max(launchPower, launchPowerMin));
                canvasObject.SetActive(false);
            }
            else
            {
                RecallLogic();
            }
            launchPower = launchPowerMin;
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
        rb.velocity = Vector2.ClampMagnitude(rb.velocity, m_MaxSpeed);

        if (m_MovingToHook)
        {
            if (Vector2.Distance((Vector2) m_CurrentHook.transform.position, (Vector2) transform.position) < m_retrieveHookDistance)
            {
                touchingHook = true;
            }
        }
        
        bool mouseDown = Input.GetMouseButton(0);
        if (m_CurrentHook == null)
        {
            m_Animator.SetBool("Charging", mouseDown);
            if (mouseDown)
            {
                if (!canvasObject.activeSelf)
                {
                    SoundManager.Instance.PlaySound(Sound.Charge);
                    canvasObject.SetActive(true);
                }

                if (launchPower < launchPowerMax)
                {
                    launchPower += launchPowerIncrement;

                    float chargePercent =  (launchPower - launchPowerMin) / (launchPowerMax - launchPowerMin);
                    chargeBar.SetValue(chargePercent);
                }
            }
        }
        

        float moveHorizontal = Input.GetAxis("Horizontal"); // Gets input from 'A' and 'D'

        // Creates a new Vector2 where x is determined by 'A' or 'D' input
        Vector2 movement = new Vector2(moveHorizontal, 0);


        
        if(!softlockCheckCoroutineRunning && !wallClimb)
        {

        // Creates a new Vector2 where x is determined by 'A' or 'D' input

            // Applies the force to the Rigidbody2D
            // if(Mathf.Abs(rb.velocity.x) < 10) {
                // Applies the force to the Rigidbody2D
                rb.AddForce(movement * airSpeed, ForceMode2D.Force);
            // }
            
            // Control while on ground
            if(grounded) {
                // Creates a new Vector2 where x is determined by 'A' or 'D' input
                rb.velocity = new Vector2(movement.x * speed, rb.velocity.y);

                if (moveHorizontal != 0)
                {
                    m_SmokeFramesRemaining -= 1;
                    if (m_SmokeFramesRemaining <= 0)
                    {
                        if (m_SmokeParticlesPrefab != null)
                            Instantiate(
                                m_SmokeParticlesPrefab,
                                new Vector3(transform.position.x, transform.position.y - 0.5f, transform.position.z),
                                Quaternion.identity
                            );

                        m_SmokeFramesRemaining = m_FramesBetweenSmoke;
                    }
                }
            }
            
            //  Less control while in air
            if(m_MovingToHook){

                // Applies the force to the Rigidbody2D
                if(Mathf.Abs(rb.velocity.x) < 10) {
                    // Applies the force to the Rigidbody2D
                    rb.AddForce(movement * 24f, ForceMode2D.Force);
                }
            }
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Wall"))
        {
            wallClimb = true;
        }
        
        if (collision.gameObject.CompareTag("Ground"))
        {
            grounded = true;
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Wall"))
        {
            wallClimb = false;
        }
        
        if (collision.gameObject.CompareTag("Ground"))
        {
            grounded = false;
        }
    }



    private void LaunchGrapplingHook(float power)
    {
        SoundManager.Instance.PlaySound(Sound.Throw);

        m_Animator.SetBool("Charging", false);

        Vector3 worldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 targetPosition = new Vector2(worldPosition.x, worldPosition.y);

        Vector2 difference = targetPosition - (Vector2) transform.position;
        float angle = Vector2.Angle(Vector2.up, difference);
        angle = (worldPosition.x > transform.position.x) ? -angle : angle;

        Vector2 unitVector = Vector2.up;
        Quaternion rotation = Quaternion.Euler(0f, 0f, angle);

        Vector2 fireVector = rotation * unitVector;

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
        Vector2 mousePositionInWorld = (Vector2)Camera.main.ScreenToWorldPoint(Input.mousePosition);

        if(rb.velocity.x > 0) {
            m_SpriteRenderer.flipX = false;
        } 
        if(rb.velocity.x < 0) {
            m_SpriteRenderer.flipX = true;
        }


        if(Input.GetMouseButton(0)){
            if (mousePositionInWorld.x < transform.position.x){
                m_SpriteRenderer.flipX = true;
            }
            else if (mousePositionInWorld.x > transform.position.x){
                m_SpriteRenderer.flipX = false;
            }
        }
    }

    private void GlideLogic()
    {
        if (m_MovingToHook)
        {
            return;
        }

        if (Input.GetButton("Jump") && !glideDepleted)
        {
            if (m_FeatherParticles != null)
            {
                m_FeatherParticles.StartParticleSystem();
            }

            m_Animator.SetBool("Gliding", true);
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
            if (m_FeatherParticles != null)
            {
                m_FeatherParticles.StopParticleSystem();
            }

            m_Animator.SetBool("Gliding", false);
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
        if (m_MovingToHook)
        {
            if (touchingHook)
            {
                DestroyGrapplingHook();
            }
            else
            {
                if (m_CurrentHook.transform.position.y >= transform.position.y || softlocked)
                {
                    DestroyGrapplingHook();
                }
            }
        }
        else
        {
            Hook hook = m_CurrentHook.GetComponent<Hook>();
            if (hook.CanRecall())
            {
                DestroyGrapplingHook();
            }
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

    private IEnumerator QuackCoroutine()
    {
        while (true)
        {
            int interval = (int) Random.Range(5, 15);
            yield return new WaitForSeconds(interval);

            int d20 = (int) Random.Range(0, 20);
            if (d20 == 10)
            {
                SoundManager.Instance.PlaySound(Sound.HumanQuack);
            }
            else
            {
                SoundManager.Instance.PlaySound(Sound.Quack);
            }
        }
    }
}
