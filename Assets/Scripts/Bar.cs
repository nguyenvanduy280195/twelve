using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;

public class Bar : MonoBehaviour
{
    public TextMeshProUGUI text;

    [NonSerialized]
    private Slider _slider;

    private void Awake()
    {
        Assert.IsNotNull(text, "Please assign 'text'");

        _slider = GetComponent<Slider>();
    }

    public float Value
    {
        get => _slider.value;
        set
        {
            _slider.value = value;

            text.text = $"{(int)_slider.value}/{_slider.maxValue}";
        }
    }

    public float MaxValue
    {
        get => _slider.maxValue;
        set
        {
            _slider.maxValue = value;
            _slider.value = value;

            text.text = $"{(int)_slider.value}/{_slider.maxValue}";
        }
    }
}
