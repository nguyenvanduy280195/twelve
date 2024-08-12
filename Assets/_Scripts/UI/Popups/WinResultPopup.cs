using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using TMPro;
using UnityEngine;

public class WinResultPopup : MonoBehaviour
{
    #region =============== For Inspector ===============

    [SerializeField] private TextMeshProUGUI _expCurrent;
    [SerializeField] private TextMeshProUGUI _expInBattle;
    [SerializeField] private TextMeshProUGUI _expFromEnemy;
    [SerializeField] private TextMeshProUGUI _expTotal;
    [SerializeField] private TextMeshProUGUI _goldCurrent;
    [SerializeField] private TextMeshProUGUI _goldInBattle;
    [SerializeField] private TextMeshProUGUI _goldBonus;
    [SerializeField] private TextMeshProUGUI _goldTotal;

    #endregion

    #region =============== Inherited via Unity ===============

    private void Start()
    {
        if (BattleUnitManager.Instance.PlayerAsBattleUnitBase.Stat is PlayerStat playerStat)
        {
            _expCurrent.text = playerStat.Exp.ToString();
            _expTotal.text = playerStat.Exp.ToString();
            _goldCurrent.text = playerStat.Gold.ToString();
            _goldTotal.text = playerStat.Gold.ToString();
        }
    }

    private void Update()
    {
    }

    #endregion

    public void OnNextButtonClicked()
    {
        MatchingBattleManager.Instance.EndBattle();
    }

    private void _UpdateExpTotal()
    {
        if (BattleUnitManager.Instance.PlayerAsBattleUnitBase.Stat is PlayerStat playerStat)
        {
            var total = playerStat.Exp + _xpEnemy + _xpBattle;
            _expTotal.GetComponent<IncreasingNumberEffect>().Thresh = total;
        }
    }

    private void _UpdateGoldTotal()
    {
        if (BattleUnitManager.Instance.PlayerAsBattleUnitBase.Stat is PlayerStat playerStat)
        {
            var total = playerStat.Gold + _gEnemy + _gBattle;
            var effect = _goldTotal.GetComponent<IncreasingNumberEffect>();
            effect.Thresh = total;

        }
    }

    private int _xpEnemy;
    private int _xpBattle;
    private int _gEnemy;
    private int _gBattle;

    #region Properties
    public int ExpFromEnemy
    {
        set
        {
            _xpEnemy = value;
            _expFromEnemy.text = value.ToString();
            _UpdateExpTotal();
        }
    }
    public int ExpInBattle
    {
        set
        {
            _xpBattle = value;
            _expInBattle.text = value.ToString();
            _UpdateExpTotal();
        }
    }
    public int GoldFromEnemy
    {
        set
        {
            _gEnemy = value;
            _goldBonus.text = value.ToString();
            _UpdateGoldTotal();
        }
    }
    public int GoldInBattle
    {
        set
        {
            _gBattle = value;
            _goldInBattle.text = value.ToString();
            _UpdateGoldTotal();
        }
    }
    #endregion
}
