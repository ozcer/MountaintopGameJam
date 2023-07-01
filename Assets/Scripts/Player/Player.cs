using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    private Rigidbody2D m_Rigidbody;
    private SpringJoint2D m_SpringJoint;

    [SerializeField]
    private GameObject m_HookPrefab;
    private GameObject m_CurrentHook;
    private float dirX = 0.2f;
    [SerializeField] private float moveSpeed = 7f;
    private float airSpeed = 13f;

    [SerializeField]
    private float m_MaxSpeed = 20f;
    private bool m_MovingToHook = false;

    [SerializeField]
    private float m_LaunchPower = 30f;
    
    [SerializeField]
    private float m_retrieveHookDistance = 1f;

    private Rigidbody2D rb;
    public float speed = 10.0f;


    private void Awake()
    {
        m_Rigidbody = GetComponent<Rigidbody2D>();
        m_SpringJoint = GetComponent<SpringJoint2D>();
    }

    private void Start() {
        rb = this.GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (m_CurrentHook == null)
            {
                LaunchGrapplingHook();
            }
            else
            {
                DestroyGrapplingHook();
            }
        }

        if (Input.GetMouseButton(1))
        {
            Time.timeScale = 0.5f;
        } else if (Input.GetMouseButtonUp(1))
        {
            Time.timeScale = 1f;
        }
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

    private void LaunchGrapplingHook()
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
        hook.Launch(fireVector, m_LaunchPower);

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
