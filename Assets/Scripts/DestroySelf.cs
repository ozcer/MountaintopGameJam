using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroySelf : MonoBehaviour
{
    public float delay = 5f;

    // Start is called before the first frame update
    private void Awake()
    {
        Destroy(gameObject, delay);
    }

}
