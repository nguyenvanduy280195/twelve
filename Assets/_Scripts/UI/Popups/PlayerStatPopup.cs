using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerStatPopup : MonoBehaviour
{
    //============= Events =============
    public static event Action<int> OnNotifyChanged;


    [SerializeField] private StatUI _name;
    [SerializeField] private StatUI _class;
    [SerializeField] private StatUI _level;
    [SerializeField] private StatUI _hp;
    [SerializeField] private StatUI _regenHP;
    [SerializeField] private StatUI _attack;
    [SerializeField] private StatUI _mana;
    [SerializeField] private StatUI _regenMana;
    [SerializeField] private StatUI _stamina;
    [SerializeField] private StatUI _regenStamina;
    [SerializeField] private StatUI _nPoints;
    [SerializeField] private Button _applyButton;


    private void _OnUIStatChanged(int delta) => nPoints -= delta;

    private int _OnNumberOfPointsGot() => nPoints;

    private void Awake()
    {
        ChangableStatUI.OnNumberOfPointsGot += _OnNumberOfPointsGot;
        ChangableStatUI.OnValueChanged += _OnUIStatChanged;
    }

    private void OnDestroy()
    {
        ChangableStatUI.OnNumberOfPointsGot -= _OnNumberOfPointsGot;
        ChangableStatUI.OnValueChanged -= _OnUIStatChanged;
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
            (_hp as ChangableStatUI).UpButtonEnabled = value;
            (_regenHP as ChangableStatUI).UpButtonEnabled = value;
            (_attack as ChangableStatUI).UpButtonEnabled = value;
            (_mana as ChangableStatUI).UpButtonEnabled = value;
            (_regenMana as ChangableStatUI).UpButtonEnabled = value;
            (_stamina as ChangableStatUI).UpButtonEnabled = value;
            (_regenStamina as ChangableStatUI).UpButtonEnabled = value;
        }
    }

    public bool AllDownButtonsEnabled
    {
        set
        {
            (_hp as ChangableStatUI).DownButtonEnabled = value;
            (_regenHP as ChangableStatUI).DownButtonEnabled = value;
            (_attack as ChangableStatUI).DownButtonEnabled = value;
            (_mana as ChangableStatUI).DownButtonEnabled = value;
            (_regenMana as ChangableStatUI).DownButtonEnabled = value;
            (_stamina as ChangableStatUI).DownButtonEnabled = value;
            (_regenStamina as ChangableStatUI).DownButtonEnabled = value;
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
