using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChangableStatUI : StatUI
{

    //============= Events =============
    public static event Action<int> OnNumberOfPointsChanged;
    public static event Func<int> OnNumberOfPointsGot;


    //============= Fields =============
    [SerializeField] private GameObject _downButton;
    [SerializeField] private GameObject _upButton;

    //============= Properties =============

    public bool UpButtonEnabled { set => _upButton.SetActive(value); }
    public bool DownButtonEnabled { set => _downButton.SetActive(value); }

    public float Weight = 1;


    public void OnIncrease()
    {
        var value = int.Parse(Content);

        if (OnNumberOfPointsGot?.Invoke() > 0)
        {
            OnNumberOfPointsChanged?.Invoke(1);
            Content = $"{value + Weight}";
        }
    }

    public void OnDecrease()
    {
        var value = int.Parse(Content);
        if (value > _threshDown)
        {
            Content = $"{value - Weight}";
            OnNumberOfPointsChanged?.Invoke(-1);
        }
    }

}
