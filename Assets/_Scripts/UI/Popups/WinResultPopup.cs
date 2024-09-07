using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class WinResultPopup : PopupTemplate
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

    private int _xpEnemy;
    private int _xpBattle;
    private int _gEnemy;
    private int _gBattle;


    #region =============== Override methods ===============

    private void Start()
    {
        var playerStat = ChoosingLevelUnitManager.Instance?.PlayerStat;
        if (playerStat is not null)
        {
            _expCurrent.text = playerStat.Exp.ToString();
            _expTotal.text = playerStat.Exp.ToString();
            _goldCurrent.text = playerStat.Gold.ToString();
            _goldTotal.text = playerStat.Gold.ToString();
        }
    }

    private void OnDestroy()
    {
        var playerStat = ChoosingLevelUnitManager.Instance?.PlayerStat;
        if (playerStat is not null)
        {
            playerStat.Exp += _xpEnemy + _xpBattle;
            playerStat.Gold += _gEnemy + _gBattle;
            SaveSystem.SavePlayerStat(playerStat);
        }
    }

    protected override void _ExitBattleInCasualMode() => MatchingBattleManager.Instance?.EndBattle();



    #endregion

    #region =============== Callbacks ===============

    public void OnNextButtonClicked()
    {
        HidePopup();
        _ReturnPreviousScene();
    }

    #endregion

    #region =============== Properties ===============

    public float ExpFromEnemy
    {
        set
        {
            _xpEnemy = (int)value;
            _expFromEnemy.text = _xpEnemy.ToString();
            _UpdateExpTotal();
        }
    }

    public float ExpInBattle
    {
        set
        {
            _xpBattle = (int)value;
            _expInBattle.text = _xpBattle.ToString();
            _UpdateExpTotal();
        }
    }

    public float GoldFromEnemy
    {
        set
        {
            _gEnemy = (int)value;
            _goldBonus.text = _gEnemy.ToString();
            _UpdateGoldTotal();
        }
    }

    public float GoldInBattle
    {
        set
        {
            _gBattle = (int)value;
            _goldInBattle.text = _gBattle.ToString();
            _UpdateGoldTotal();
        }
    }

    #endregion

    #region =============== Supporting methods ===============

    private void _UpdateExpTotal()
    {
        var playerStat = ChoosingLevelUnitManager.Instance?.PlayerStat;
        if (playerStat is not null)
        {
            var total = playerStat.Exp + _xpEnemy + _xpBattle;
            _expTotal.GetComponent<IncreasingNumberEffect>().Thresh = total;
        }
    }

    private void _UpdateGoldTotal()
    {
        var playerStat = ChoosingLevelUnitManager.Instance?.PlayerStat;
        if (playerStat is not null)
        {
            var total = playerStat.Gold + _gEnemy + _gBattle;
            _goldTotal.GetComponent<IncreasingNumberEffect>().Thresh = total;
        }
    }

    #endregion

}
