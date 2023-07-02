using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public Animator animator;
    
    private Rigidbody2D rb;
    private SpringJoint2D m_SpringJoint;
    
    [SerializeField]
    private GameObject m_HookPrefab;
    private GameObject m_CurrentHook;
    private float dirX = 0.2f;
    [SerializeField] private float moveSpeed = 7f;
    private float airSpeed = 13f;
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
    public float launchPowerIncrement = 0.3f;
    public float launchPower = 0f;
    
    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        m_SpringJoint = GetComponent<SpringJoint2D>();
    }

    private void Update()
    {
        if (Input.GetMouseButtonUp(0))
        {
            if (m_CurrentHook == null)
            {
                LaunchGrapplingHook(Mathf.Max(launchPower, launchPowerMin));
            }
            else
            {
                DestroyGrapplingHook();
            }
            launchPower = launchPowerMin;
        }
        
        // Bullet time
        Time.timeScale = Input.GetButton("Jump") ? 0.5f : 1f;
    }

    private void FixedUpdate()
    {
        rb.velocity = Vector2.ClampMagnitude(rb.velocity, m_MaxSpeed);

        if (m_MovingToHook)
        {
            if (Vector2.Distance((Vector2) m_CurrentHook.transform.position, (Vector2) transform.position) < m_retrieveHookDistance)
            {
                m_SpringJoint.connectedBody = rb;

                m_MovingToHook = false;

                Destroy(m_CurrentHook);
                m_CurrentHook = null;
            }
        }
        
        bool mouseDown = Input.GetMouseButton(0);
        animator.SetBool("Charging", mouseDown);
        if (mouseDown)
        {
            
            if (launchPower < launchPowerMax)
            {
                launchPower += launchPowerIncrement;
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
    }

    private void DestroyGrapplingHook()
    {
        m_SpringJoint.connectedBody = rb;
        m_MovingToHook = false;

        Destroy(m_CurrentHook);
        m_CurrentHook = null;
    }

    public void MoveToHook(Vector2 targetPosition)
    {
        m_SpringJoint.connectedBody = m_CurrentHook.GetComponent<Rigidbody2D>();
        m_MovingToHook = true;
    }
}
