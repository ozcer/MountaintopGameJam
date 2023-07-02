using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InstructionsCanvas : MonoBehaviour
{
    [SerializeField]
    private GameObject m_CanvasObject;

    void Update()
    {
        if (m_CanvasObject == null)
        {
            return;
        }

        if (Input.GetKey(KeyCode.Tab))
        {
            bool enabled = m_CanvasObject.activeSelf;
            if (!enabled)
            {
                m_CanvasObject.SetActive(true);
            }
        }
        else if (Input.GetKeyUp(KeyCode.Tab))
        {
            m_CanvasObject.SetActive(false);
        }
    }
}
