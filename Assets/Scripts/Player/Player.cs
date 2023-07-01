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

    [SerializeField]
    private float m_MaxSpeed = 1f;
    private bool m_MovingToHook = false;

    [SerializeField]
    private float m_LaunchPower = 30f;

    private void Awake()
    {
        m_Rigidbody = GetComponent<Rigidbody2D>();
        m_SpringJoint = GetComponent<SpringJoint2D>();
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
            if (Vector2.Distance((Vector2) m_CurrentHook.transform.position, (Vector2) transform.position) < 1f)
            {
                m_SpringJoint.connectedBody = m_Rigidbody;

                m_MovingToHook = false;

                Destroy(m_CurrentHook);
                m_CurrentHook = null;
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
