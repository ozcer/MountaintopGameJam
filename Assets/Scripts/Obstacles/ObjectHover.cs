using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectHover : MonoBehaviour
{
    public float hoverHeight = 1f;        // Maximum height of the hover
    public float hoverSpeed = 1f;         // Speed of the hover movement

    private Vector3 initialPosition;      // Initial position of the object

    private void Start()
    {
        initialPosition = transform.position;   // Store the initial position of the object
    }

    private void Update()
    {
        // Calculate the vertical displacement using the sine wave
        float yOffset = Mathf.Sin(Time.time * hoverSpeed) * hoverHeight;

        // Update the object's position with the vertical displacement
        transform.position = initialPosition + new Vector3(0f, yOffset, 0f);
    }
}

