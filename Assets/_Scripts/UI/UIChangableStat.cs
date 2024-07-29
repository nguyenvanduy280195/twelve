using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIChangableStat : UIStat
{

    //============= Events =============
    public static event Action<int> OnValueChanged;

    public static event Func<int> OnNumberOfPointsGot;


    //============= Fields =============
    [SerializeField] private GameObject _downButton;
    [SerializeField] private GameObject _upButton;

    //============= Properties =============

    public bool UpButtonEnabled { set => _upButton.SetActive(value); }
    public bool DownButtonEnabled { set => _downButton.SetActive(value); }

    public void OnIncrease()
    {
        var value = int.Parse(Content);

        if (OnNumberOfPointsGot?.Invoke() > 0)
        {
            OnValueChanged?.Invoke(1);
            Content = $"{value + 1}";
        }
    }

    public void OnDecrease()
    {
        var value = int.Parse(Content);
        if (value > _threshDown)
        {
            Content = $"{value - 1}";
            OnValueChanged?.Invoke(-1);
        }
    }

}
