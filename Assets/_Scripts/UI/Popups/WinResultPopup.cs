using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

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

    private int _xpEnemy;
    private int _xpBattle;
    private int _gEnemy;
    private int _gBattle;


    #region =============== Inherited via Unity ===============

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

    #endregion

    #region =============== Callbacks ===============

    public void OnNextButtonClicked()
    {
        if (MatchingBattleManager.Instance)
        {
            MatchingBattleManager.Instance.EndBattle();
        }
        else
        {
            SceneManager.LoadScene("InBattle");
        }
    }

    #endregion

    #region =============== Properties ===============

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
