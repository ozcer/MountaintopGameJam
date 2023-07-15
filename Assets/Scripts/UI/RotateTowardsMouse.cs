using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateTowardsMouse : MonoBehaviour
{
    public GameObject arrow;
    public Player player;
    ControllerManager controllerManager;
    private Vector2 dir;

    private void Awake()
    {
        controllerManager = FindObjectOfType<ControllerManager>();
    }

    void FixedUpdate()
    {
        if (player.stickAim)
        {
            dir = controllerManager.aimInputVector;
        }
        else
        {
            dir = Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.position;
        }

        if (player.invertDirection)
        {
            dir *= -1f; 
        }

        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        arrow.transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
    }
}

