using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleManager : MonoBehaviour
{

    public ParticleSystem groundParticles;
    public GroundCheck ground;

    [Header("Landing Particle Colors")]
    public Color woodColor;
    public Color whiteColor;
    public Color bounceColor;

    public void InstantiateGroundParticles()
    {
        Vector3 particlePosition = transform.position;
        particlePosition.y -= 1f;

        ParticleSystem particleSystem = Instantiate(groundParticles, particlePosition, Quaternion.identity);
        ParticleSystem.MainModule mainModule = particleSystem.main;

        Color particleColor = GetParticleColor();
        mainModule.startColor = particleColor;
    }

    private Color GetParticleColor()
    {
        if (ground.isWoodLayer)
        {
            return woodColor;
        }
        else if (ground.isWhiteLayer)
        {
            return whiteColor;
        }
        else if (ground.isBounceLayer)
        {
            return bounceColor;
        }

        return Color.white; // default
    }
}
