using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    public Slider slider;
    public Gradient gradient;
    public Image fill;

    public float HP
    {
        get => slider.value;
        set
        {
            slider.value = value;

            fill.color = gradient.Evaluate(slider.normalizedValue);
        }
    }

    public float MaxHP
    {
        get => slider.maxValue;
        set
        {
            slider.maxValue = value;
            slider.value = value;

            fill.color = gradient.Evaluate(1f);
        }
    }



}
