using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmokeParticles : MonoBehaviour
{
    private ParticleSystem m_ParticleSystem;

    private void Awake()
    {
        m_ParticleSystem = GetComponent<ParticleSystem>();
        m_ParticleSystem.Play();
    }

    private void Update()
    {
        if (!m_ParticleSystem.isPlaying)
        {
            Destroy(gameObject);
        }
    }
}
