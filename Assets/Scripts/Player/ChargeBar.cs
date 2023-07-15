using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChargeBar : MonoBehaviour
{
    private Slider m_Slider;

    private void Awake()
    {
        m_Slider = GetComponent<Slider>();
    }

    public void SetValue(float value)
    {
        if (m_Slider != null)
        {
            m_Slider.value = value;
        }
    }
}
