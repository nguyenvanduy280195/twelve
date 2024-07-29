using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UIStat : MonoBehaviour
{
    [SerializeField] protected TextMeshProUGUI _content;

    protected int _threshDown = -1;

    public string Content
    {
        set
        {
            _content.text = value;
            if (_threshDown <= -1)
            {
                int.TryParse(value, out _threshDown);
            }
        }
        get => _content.text;
    }

}
