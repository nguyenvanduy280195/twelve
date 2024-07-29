using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ResultInfo : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _goldText;
    [SerializeField] private TextMeshProUGUI _expText;

    public string Gold
    {
        set => _goldText.text = value;
        get => _goldText.text;
    }

    public string Exp
    {
        set => _expText.text = value;
        get => _expText.text;
    }

}
