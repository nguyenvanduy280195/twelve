using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;

public class Bar : MonoBehaviour
{
    private Slider _slider;

    private void Awake()
    {
        _slider = GetComponent<Slider>();
    }

    public TextMeshProUGUI text;

    public float Speed = 1;

    [NonSerialized]
    public float Value;

    public float MaxValue
    {
        get => _slider.maxValue;
        set => _slider.maxValue = value;
    }

    private void Update()
    {
        if (Mathf.Abs(_slider.value - Value) > float.Epsilon)
        {
            _slider.value += Speed * Time.deltaTime * (Value - _slider.value);
            text.text = $"{Mathf.Round(_slider.value)}/{_slider.maxValue}";
        }
    }
}
