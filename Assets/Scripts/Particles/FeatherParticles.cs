using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FeatherParticles : MonoBehaviour
{
    private ParticleSystem m_ParticleSystem;

    private void Awake()
    {
        m_ParticleSystem = GetComponent<ParticleSystem>();
        m_ParticleSystem.Stop();
    }

    public void StartParticleSystem()
    {
        if (!m_ParticleSystem.isPlaying)
        {
            m_ParticleSystem.Play();
        }
    }

    public void StopParticleSystem()
    {
        if (m_ParticleSystem.isPlaying)
        {
            m_ParticleSystem.Stop();
        }
    }
}
