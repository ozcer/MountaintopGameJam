using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    private Rigidbody2D m_Rigidbody;

    [SerializeField]
    private GameObject m_HookPrefab;
    private GameObject m_CurrentHook;

    [SerializeField]
    private float m_PullPower = 10f;
    private Vector2 m_PullVelocity;

    private bool m_MovingToHook = false;

    [SerializeField]
    private float m_LaunchPower = 30f;

    private void Awake()
    {
        m_Rigidbody = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            LaunchGrapplingHook();
        }
    }

    private void FixedUpdate()
    {
        if (m_MovingToHook)
        {
            m_Rigidbody.velocity = m_PullVelocity;
            if (Vector2.Distance((Vector2) m_CurrentHook.transform.position, (Vector2) transform.position) < 1f)
            {
                m_MovingToHook = false;
                Destroy(m_CurrentHook);
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

    public void MoveToHook(Vector2 targetPosition)
    {
        Vector2 difference = targetPosition - (Vector2) transform.position;
        float angle = Vector2.Angle(Vector2.up, difference);
        angle = (targetPosition.x > transform.position.x) ? -angle : angle;

        Vector2 unitVector = Vector2.up;
        Quaternion rotation = Quaternion.Euler(0f, 0f, angle);
        Vector2 direction = rotation * unitVector;

        m_PullVelocity = direction * m_PullPower;

        m_MovingToHook = true;
    }
}
