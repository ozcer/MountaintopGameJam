using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class HoldpointSpawner : MonoBehaviour
{
    
    [SerializeField] GameObject holdpointPrefab;
    float nodeSpawnTimer = 0f;
    [SerializeField] float nodeSpawnDelay = 5f;
    [SerializeField] float nodeSpawnDistanceMinimum = 5f;
    [SerializeField] float nodeSpawnDistanceMaximum = 10f;
    

    void FixedUpdate()
    {
            
            nodeSpawnTimer += Time.fixedDeltaTime;
            if (nodeSpawnTimer >= nodeSpawnDelay)
            {
                nodeSpawnTimer = 0f;
                SpawnHoldpoint();
            }
    }

    void SpawnHoldpoint()
    {
        Vector2 randomPoint = GetRandomPointInRadius(nodeSpawnDistanceMinimum, nodeSpawnDistanceMaximum);
        GameObject player = GameObject.FindWithTag("Player");
        randomPoint += (Vector2) player.transform.position;
        Instantiate(holdpointPrefab, randomPoint, Quaternion.identity);
    }
    
    private Vector2 GetRandomPointInRadius(float minRadius, float maxRadius)
    {
        float angle = Random.Range(0f, Mathf.PI * 2f);
        float radius = Random.Range(minRadius, maxRadius);

        float x = radius * Mathf.Cos(angle);
        float y = radius * Mathf.Sin(angle);

        Vector2 randomPoint = new Vector2(x, y);
        return randomPoint;
    }
}
