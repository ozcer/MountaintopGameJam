using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ReturnToTitle : MonoBehaviour
{
    ControllerManager controllerManager;

    private void Awake()
    {
        controllerManager = FindObjectOfType<ControllerManager>();
    }

    // Update is called once per frame
    void Update()
    {
        if (controllerManager.mouseUp)
        {
            SceneManager.LoadScene(0);
        }
    }
}
