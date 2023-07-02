using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hook : MonoBehaviour
{
    public Player Player;

    private Rigidbody2D m_Rigidbody;
    public LineRenderer m_LineRenderer;

    private bool m_CanRecall = false;

    void Update()
    {
        if (Player != null)
        {
            m_LineRenderer.SetPosition(0, transform.position);
            m_LineRenderer.SetPosition(1, Player.transform.position);
        }
    }

    private void Awake()
    {
        m_Rigidbody = GetComponent<Rigidbody2D>();
    }

    public void Launch(Vector2 direction, float power)
    {
        m_Rigidbody.AddForce(direction * power, ForceMode2D.Impulse);
        StartCoroutine(RecallCheckCoroutine());
    }

    public bool CanRecall()
    {
        return m_CanRecall;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("TetherableWall"))
        {
            Stick();
        }
    }

    private void Stick()
    {
        m_Rigidbody.bodyType = RigidbodyType2D.Static;
        if (Player != null)
        {
            Player.AttachHook();
        }
    }

    private IEnumerator RecallCheckCoroutine()
    {
        while (!m_CanRecall)
        {
            float initialYValue = transform.position.y;
            yield return new WaitForSeconds(1);
            float finalYValue = transform.position.y;

            if (Mathf.Abs(finalYValue - initialYValue) < 0.5f)
            {
                m_CanRecall = true;
            }
        }
    }
}
