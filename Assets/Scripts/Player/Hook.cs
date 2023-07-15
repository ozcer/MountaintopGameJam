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
    private Color lineColor;
    private float h_MaxSpeed = 40;
    private bool tethered;

    void Update()
    {
        if (Player != null)
        {
            m_LineRenderer.SetPosition(0, transform.position);
            m_LineRenderer.SetPosition(1, Player.transform.position);

            
            m_LineRenderer.startWidth = .2f;
            m_LineRenderer.endWidth = .2f;
            lineColor = new Color(0.7f, 0.4f, 0.2f);

            m_LineRenderer.startColor = lineColor;
            m_LineRenderer.endColor = lineColor;

        }

    }

    private void FixedUpdate(){
        if(!tethered){
            m_Rigidbody.velocity = Vector2.ClampMagnitude(m_Rigidbody.velocity, h_MaxSpeed);
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
            SoundManager.Instance.PlaySound(Sound.StickyHit);
            Stick();
        }

        if (collision.gameObject.layer == LayerMask.NameToLayer("NonTetherableWall"))
        {
            SoundManager.Instance.PlaySound(Sound.NonStickyHit);
        }

        if (collision.gameObject.layer == LayerMask.NameToLayer("BouncyWall"))
        {
            SoundManager.Instance.PlaySound(Sound.BouncyHit);
        }
    }

    private void Stick()
    {
        tethered = true;
        m_Rigidbody.bodyType = RigidbodyType2D.Static;
        if (Player != null)
        {
            Player.MoveToHook();
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
