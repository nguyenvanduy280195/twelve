using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIPlayerStat : MonoBehaviour
{
    //============= Events =============
    public static event Action<int> OnNotifyChanged;


    [SerializeField] private UIStat _name;
    [SerializeField] private UIStat _class;
    [SerializeField] private UIStat _level;
    [SerializeField] private UIStat _hp;
    [SerializeField] private UIStat _regenHP;
    [SerializeField] private UIStat _attack;
    [SerializeField] private UIStat _mana;
    [SerializeField] private UIStat _regenMana;
    [SerializeField] private UIStat _stamina;
    [SerializeField] private UIStat _regenStamina;
    [SerializeField] private UIStat _nPoints;
    [SerializeField] private Button _applyButton;


    private void _OnUIStatChanged(int delta) => nPoints -= delta;

    private int _OnNumberOfPointsGot() => nPoints;

    private void Awake()
    {
        UIChangableStat.OnNumberOfPointsGot += _OnNumberOfPointsGot;
        UIChangableStat.OnValueChanged += _OnUIStatChanged;
    }

    private void OnDestroy()
    {
        UIChangableStat.OnNumberOfPointsGot -= _OnNumberOfPointsGot;
        UIChangableStat.OnValueChanged -= _OnUIStatChanged;
    }

    private void Start()
    {
        var playerStat = BattleUnitManager.Instance.Player.Stat;
        _name.Content = playerStat.Name;
        _class.Content = playerStat.Class;
        _level.Content = playerStat.Level.ToString();
        _hp.Content = playerStat.HP.ToString();
        _regenHP.Content = playerStat.RegenHP.ToString();
        _attack.Content = playerStat.Attack.ToString();
        _mana.Content = playerStat.Mana.ToString();
        _regenMana.Content = playerStat.RegenMana.ToString();
        _stamina.Content = playerStat.Stamina.ToString();
        _regenStamina.Content = playerStat.RegenStamina.ToString();
        _nPoints.Content = "10";
        ApplyButtonEnabled = true;
    }

    public bool AllUpButtonsEnabled
    {
        set
        {
            (_hp as UIChangableStat).UpButtonEnabled = value;
            (_regenHP as UIChangableStat).UpButtonEnabled = value;
            (_attack as UIChangableStat).UpButtonEnabled = value;
            (_mana as UIChangableStat).UpButtonEnabled = value;
            (_regenMana as UIChangableStat).UpButtonEnabled = value;
            (_stamina as UIChangableStat).UpButtonEnabled = value;
            (_regenStamina as UIChangableStat).UpButtonEnabled = value;
        }
    }

    public bool AllDownButtonsEnabled
    {
        set
        {
            (_hp as UIChangableStat).DownButtonEnabled = value;
            (_regenHP as UIChangableStat).DownButtonEnabled = value;
            (_attack as UIChangableStat).DownButtonEnabled = value;
            (_mana as UIChangableStat).DownButtonEnabled = value;
            (_regenMana as UIChangableStat).DownButtonEnabled = value;
            (_stamina as UIChangableStat).DownButtonEnabled = value;
            (_regenStamina as UIChangableStat).DownButtonEnabled = value;
        }
    }

    public int nPoints { set => _nPoints.Content = value.ToString(); get => int.Parse(_nPoints.Content); }

    public bool ApplyButtonEnabled { set => _applyButton.interactable = value; }

    public void UpLevel()
    {

    }

    public void OnApplyClicked()
    {
        var player = BattleUnitManager.Instance.Player;
        player.Stat.Name = _name.Content;
        player.Stat.Class = _class.Content;
        player.Stat.Level = int.Parse(_level.Content);
        player.Stat.HP = float.Parse(_hp.Content);
        player.Stat.RegenHP = float.Parse(_regenHP.Content);
        player.Stat.Attack = float.Parse(_attack.Content);
        player.Stat.Mana = float.Parse(_mana.Content);
        player.Stat.RegenMana = float.Parse(_regenMana.Content);
        player.Stat.Stamina = float.Parse(_stamina.Content);
        player.Stat.RegenStamina = float.Parse(_regenStamina.Content);
    }

    public void OnCloseClicked()
    {
        gameObject.SetActive(false);
    }
}
