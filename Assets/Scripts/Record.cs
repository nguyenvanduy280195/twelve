using System;
using TMPro;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.SocialPlatforms.Impl;
using UnityEngine.UI;
using static System.Net.Mime.MediaTypeNames;

public class Record : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI _nTurns;

    [SerializeField]
    private TextMeshProUGUI _attackScore;

    [SerializeField]
    private TextMeshProUGUI _expScore;

    [SerializeField]
    private TextMeshProUGUI _goldScore;

    private void Awake()
    {
        Assert.IsNotNull(_nTurns, "Please assign '_nTurns'");
        Assert.IsNotNull(_attackScore, "Please assign '_attackScore'");
        Assert.IsNotNull(_expScore, "Please assign '_expScore'");
        Assert.IsNotNull(_goldScore, "Please assign '_goldScore'");
    }

    private int? GetValue(TextMeshProUGUI text)
    {
        try
        {
            return int.Parse(text.text);
        }
        catch (Exception e)
        {
            Debug.LogWarning(e.Message);
        }

        return null;
    }
    public int NTurns { set => _nTurns.text = $"{value}"; get => GetValue(_nTurns) ?? -1; }

    public int AttackScore { set => _attackScore.text = $"{value}"; get => GetValue(_attackScore) ?? -1; }

    public int ExpScore { set => _expScore.text = $"{value}"; get => GetValue(_expScore) ?? -1; }

    public int GoldScore{ set => _goldScore.text = $"{value}"; get => GetValue(_goldScore) ?? -1; }
}
