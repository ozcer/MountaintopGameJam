using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public Animator animator;
    private Rigidbody2D rb;
    private SpringJoint2D m_SpringJoint;
    private SpriteRenderer m_SpriteRenderer;
    
    [SerializeField]
    private GameObject m_HookPrefab;
    private GameObject m_CurrentHook;
    private float dirX = 0.2f;
    [SerializeField] private float moveSpeed = 7f;
    private float airSpeed = 13f;
    public float speed = 10.0f;

    [SerializeField]
    private float m_MaxSpeed = 20f;
    private bool m_HookAttachedAndWaiting = false;
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
    public int maxGlideFrames = 1200;
    public int glideFramesRemaining;
    public bool glideDepleted = false;

    [Header("UI")]
    public GameObject canvasObject;
    public ChargeBar chargeBar;

    [Header("Recall")]
    public bool canRecall = false;
    public bool recallCoroutineRunning = false;
    
    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        m_SpringJoint = GetComponent<SpringJoint2D>();
        m_SpriteRenderer = GetComponent<SpriteRenderer>();

        glideFramesRemaining = maxGlideFrames;
    }

    private void Update()
    {
        if (Input.GetMouseButtonUp(0))
        {
            if (m_CurrentHook == null)
            {
                LaunchGrapplingHook(Mathf.Max(launchPower, launchPowerMin));
                launchPower = launchPowerMin;

                canvasObject.SetActive(false);
            }
            else
            {
                if (m_HookAttachedAndWaiting)
                {
                    MoveToHook();
                }
                else
                {
                    RecallLogic();
                }
            }
            launchPower = launchPowerMin;
        }
        
        FaceMouse();
        GlideLogic();
    }

    private void FixedUpdate()
    {
        rb.velocity = Vector2.ClampMagnitude(rb.velocity, m_MaxSpeed);

        if (m_MovingToHook)
        {
            if (Vector2.Distance((Vector2) m_CurrentHook.transform.position, (Vector2) transform.position) < m_retrieveHookDistance)
            {
                DestroyGrapplingHook();
            }
        }
        
        bool mouseDown = Input.GetMouseButton(0);
        if (m_CurrentHook == null)
        {
            animator.SetBool("Charging", mouseDown);
            if (mouseDown)
            {
                if (!canvasObject.activeSelf)
                {
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
        
        // Control while on ground
        if(!m_MovingToHook){

            float moveHorizontal = Input.GetAxis("Horizontal"); // Gets input from 'A' and 'D'

            // Creates a new Vector2 where x is determined by 'A' or 'D' input
            Vector2 movement = new Vector2(moveHorizontal, 0);

            // Applies the movement to the Rigidbody2D
            rb.velocity = new Vector2(movement.x * speed, rb.velocity.y);

        }

        //  Less control while in air
        if(m_MovingToHook){
            float moveHorizontal = Input.GetAxis("Horizontal"); // Gets input from 'A' and 'D'

            // Creates a new Vector2 where x is determined by 'A' or 'D' input
            Vector2 movement = new Vector2(moveHorizontal, 0);

            // Applies the force to the Rigidbody2D
            if(Mathf.Abs(rb.velocity.x) < 10) {
                // Applies the force to the Rigidbody2D
                rb.AddForce(movement * airSpeed, ForceMode2D.Force);
            }
        }
    }

    private void LaunchGrapplingHook(float power)
    {
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
        GameManager.Instance.ResetTargets();
        GameManager.Instance.ChangeCameraTarget(transform);

        m_SpringJoint.connectedBody = rb;
        m_MovingToHook = false;

        Destroy(m_CurrentHook);
        m_CurrentHook = null;
    }

    private void FaceMouse()
    {
        Vector2 mousePositionInWorld = (Vector2) Camera.main.ScreenToWorldPoint(Input.mousePosition);
        m_SpriteRenderer.flipX = (mousePositionInWorld.x < transform.position.x);
    }

    private void GlideLogic()
    {
        if (m_MovingToHook)
        {
            return;
        }

        if (Input.GetButton("Jump") && !glideDepleted)
        {
            if (rb.velocity.y < -1)
            {
                rb.velocity = new Vector2(rb.velocity.x, -1);
            }

            glideFramesRemaining -= 1;
            if (glideFramesRemaining <= 0)
            {
                glideDepleted = true;
            }    
        }
        else
        {
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
            if (canRecall)
            {
                recallCoroutineRunning = false;
                DestroyGrapplingHook();
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

    public void AttachHook()
    {
        GameManager.Instance.ResetAndAddTargets(new Transform[] { transform, m_CurrentHook.transform });

        m_SpringJoint.autoConfigureDistance = true;
        m_SpringJoint.connectedBody = m_CurrentHook.GetComponent<Rigidbody2D>();

        m_HookAttachedAndWaiting = true;
    }

    public void MoveToHook()
    {
        m_SpringJoint.autoConfigureDistance = false;
        m_SpringJoint.distance = 0.005f;

        m_MovingToHook = true;
        m_HookAttachedAndWaiting = false;

        recallCoroutineRunning = true;
        canRecall = false;
        StartCoroutine(RecallCheckCoroutine());
    }

    private IEnumerator RecallCheckCoroutine()
    {
        while (!canRecall && recallCoroutineRunning)
        {
            Vector2 initialPosition = transform.position;
            yield return new WaitForSeconds(1);
            if (recallCoroutineRunning)
            {
                Vector2 finalPosition = transform.position;

                float distance = Vector2.Distance(initialPosition, finalPosition);
                if (distance < 0.5f)
                {
                    canRecall = true;
                }
            }
        }
    }
}
