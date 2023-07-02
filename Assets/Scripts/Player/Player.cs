using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public Animator animator;
    
    private Rigidbody2D m_Rigidbody;
    private SpringJoint2D m_SpringJoint;
    
    [SerializeField]
    private GameObject m_HookPrefab;
    private GameObject m_CurrentHook;
    private float dirX = 0.2f;
    [SerializeField] private Vector2 moveForce;
    [SerializeField] private float moveSpeed = 7f;

    [SerializeField]
    private float m_MaxSpeed = 1f;
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
    
    private void Awake()
    {
        m_Rigidbody = GetComponent<Rigidbody2D>();
        m_SpringJoint = GetComponent<SpringJoint2D>();
    }

    private void Update()
    {
        if (Input.GetMouseButtonUp(0))
        {
            if (m_CurrentHook == null)
            {
                LaunchGrapplingHook(Mathf.Max(launchPower, launchPowerMin));
                launchPower = launchPowerMin;
            }
            else
            {
                DestroyGrapplingHook();
            }
        }
        
        // Bullet time
        Time.timeScale = Input.GetButton("Jump") ? 0.5f : 1f;
        
        // Walking movement
        dirX = Input.GetAxis("Horizontal");
        moveForce = new Vector2(dirX, 0f);
        m_Rigidbody.AddForce(moveForce,ForceMode2D.Impulse);
    }

    private void FixedUpdate()
    {
        m_Rigidbody.velocity = Vector2.ClampMagnitude(m_Rigidbody.velocity, m_MaxSpeed);

        if (m_MovingToHook)
        {
            if (Vector2.Distance((Vector2) m_CurrentHook.transform.position, (Vector2) transform.position) < m_retrieveHookDistance)
            {
                m_SpringJoint.connectedBody = m_Rigidbody;

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
        m_SpringJoint.connectedBody = m_Rigidbody;
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
