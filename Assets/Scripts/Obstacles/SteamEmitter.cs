using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SteamEmitter : MonoBehaviour
{
    private ParticleSystem m_ParticleSystem;

    [SerializeField]
    private float m_IntervalBetweenEmission = 5f;

    private void Awake()
    {
        m_ParticleSystem = GetComponent<ParticleSystem>();
        m_ParticleSystem.Stop();

        StartCoroutine(EmitCoroutine());
    }

    private IEnumerator EmitCoroutine()
    {
        while (true)
        {
            m_ParticleSystem.Stop();
            m_ParticleSystem.Play();

            yield return new WaitForSeconds(m_IntervalBetweenEmission);
        }
    }
}
