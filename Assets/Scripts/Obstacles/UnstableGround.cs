using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnstableGround : MonoBehaviour
{
    private BoxCollider2D m_Collider;
    private SpriteRenderer m_SpriteRenderer;

    [SerializeField]
    private float m_DurationBeforeBreaking = 2f;

    [SerializeField]
    private float m_DurationBeforeRespawning = 3f; 

    private void Awake()
    {
        m_Collider = GetComponent<BoxCollider2D>();
        m_SpriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            OnStep();
        }
    }

    private void OnStep()
    {
        StartCoroutine(BreakCoroutine());
    }

    private void Break()
    {
        m_Collider.enabled = false;
        m_SpriteRenderer.enabled = false;

        StartCoroutine(RespawnCoroutine());
    }

    private IEnumerator BreakCoroutine()
    {
        yield return new WaitForSeconds(m_DurationBeforeBreaking);

        Break();
    }

    private IEnumerator RespawnCoroutine()
    {
        yield return new WaitForSeconds(m_DurationBeforeRespawning);

        m_Collider.enabled = true;
        m_SpriteRenderer.enabled = true;
    }
}
