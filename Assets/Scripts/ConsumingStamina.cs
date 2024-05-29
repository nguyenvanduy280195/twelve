using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Bar))]
public class ConsumingStamina : MonoBehaviour
{
    [NonSerialized]
    private Bar _staminaBar;

    private void Awake()
    {
        _staminaBar = GetComponent<Bar>();
    }

    // Update is called once per frame
    void Update()
    {
        //_staminaBar.Value -= Time.deltaTime;
    }
}
