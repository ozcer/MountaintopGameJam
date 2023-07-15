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
    public GameObject tetherPosition;

    private float nonStickyHitCooldown = 0.5f;
    private float bouncyHitCooldown = 0.5f;

    private float lastNonStickyHitTime = 0f;
    private float lastBouncyHitTime = 0f;

    void Update()
    {
        if (Player != null)
        {
            m_LineRenderer.SetPosition(0, tetherPosition.transform.position);
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
        if (m_Rigidbody.velocity.magnitude > 0.01f)
        {
            float angle = Mathf.Atan2(m_Rigidbody.velocity.y, m_Rigidbody.velocity.x) * Mathf.Rad2Deg;
            angle -= 90f; 
            transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
        }
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
            if (Time.time - lastNonStickyHitTime >= nonStickyHitCooldown)
            {
                SoundManager.Instance.PlaySound(Sound.NonStickyHit);
                lastNonStickyHitTime = Time.time;
            }
        }

        if (collision.gameObject.layer == LayerMask.NameToLayer("BouncyWall"))
        {
            if (Time.time - lastBouncyHitTime >= bouncyHitCooldown)
            {
                SoundManager.Instance.PlaySound(Sound.BouncyHit);
                lastBouncyHitTime = Time.time;
            }
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
