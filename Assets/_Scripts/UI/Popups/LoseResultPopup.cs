using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class LoseResultPopup : PopupTemplate
{
    [SerializeField] private TextMeshProUGUI _goldText;
    [SerializeField] private TextMeshProUGUI _expText;

    private int _nGolds;
    private int _nExps;

    public int Gold
    {
        set
        {
            _goldText.text = value.ToString();
            _nGolds = value;
        }

        get => _nGolds;
    }

    public int Exp
    {
        set
        {
            _expText.text = value.ToString();
            _nExps = value;
        }

        get => _nExps;
    }

    private void OnDestroy()
    {
        var playerStat = ChoosingLevelUnitManager.Instance.PlayerData;
        if (playerStat != null)
        {
            playerStat.Gold += _nGolds;
            playerStat.Exp += _nExps;
        }
    }

    public void OnNextButtonClicked()
    {
        _ReturnPreviousScene();
        MatchingBattleManager.Instance?.ReviveAtCheckpoint();
        AudioManager.Instance?.PlayButton();
    }
}
