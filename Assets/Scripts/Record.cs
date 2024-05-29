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

    [SerializeField]
    private TextMeshProUGUI _hpScore;

    [SerializeField]
    private TextMeshProUGUI _mpScore;

    [SerializeField]
    private TextMeshProUGUI _staminaScore;

    private void Awake()
    {
        Assert.IsNotNull(_nTurns, "Please assign '_nTurns'");
        Assert.IsNotNull(_attackScore, "Please assign '_attackScore'");
        Assert.IsNotNull(_expScore, "Please assign '_expScore'");
        Assert.IsNotNull(_goldScore, "Please assign '_goldScore'");
        Assert.IsNotNull(_hpScore, "Please assign '_hpScore'");
        Assert.IsNotNull(_mpScore, "Please assign '_mpScore'");
        Assert.IsNotNull(_staminaScore, "Please assign '_staminaScore'");
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

    public int HPScore { set => _hpScore.text = $"{value}"; get => GetValue(_hpScore) ?? -1; }

    public int MPScore { set => _mpScore.text = $"{value}"; get => GetValue(_mpScore) ?? -1; }

    public int StaminaScore { set => _staminaScore.text = $"{value}"; get => GetValue(_staminaScore) ?? -1; }
}
