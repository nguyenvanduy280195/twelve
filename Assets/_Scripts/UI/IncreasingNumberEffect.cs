using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class IncreasingNumberEffect : MonoBehaviour
{
    private TextMeshProUGUI _numberAsText;

    private int _number;

    public int Thresh;

    private void Start()
    {
        _numberAsText = GetComponent<TextMeshProUGUI>();

        if (!int.TryParse(_numberAsText.text, out _number))
        {
            Debug.LogWarning($"_numberAsText.text is a number");
        }
    }

    private void Update()
    {
        var delta = Thresh - _number;
        if (delta != 0)
        {
            _number += delta / Mathf.Abs(delta);
            _numberAsText.text = _number.ToString();
        }
    }
}
