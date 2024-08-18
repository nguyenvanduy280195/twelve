using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerStatPopup : MonoBehaviour
{
    #region Fields

    [SerializeField] private StatUI _name;
    [SerializeField] private StatUI _class;
    [SerializeField] private StatUI _level;
    [SerializeField] private StatUI _gold;

    [SerializeField] private Bar _hp;
    [SerializeField] private Bar _mana;
    [SerializeField] private Bar _stamina;

    [SerializeField] private StatUI _attack;
    [SerializeField] private StatUI _maxHP;
    [SerializeField] private StatUI _regenHP;
    [SerializeField] private StatUI _maxMana;
    [SerializeField] private StatUI _regenMana;
    [SerializeField] private StatUI _maxStamina;
    [SerializeField] private StatUI _regenStamina;
    [SerializeField] private StatUI _nPoints;
    [SerializeField] private StatUI _exp;

    [SerializeField] private bool _allButtonsEnabled = true;

    [SerializeField] private Button _applyButton;
    [SerializeField] private Button _levelupButton;

    [SerializeField] private int LevelUpExpThresh = 100;
    [SerializeField] private int LevelupPoints = 5;

    private PlayerStat _playerStat;

    #endregion

    private void _OnUIStatChanged(int delta)
    {
        nPoints -= delta;
        ApplyButtonEnabled = true;
    }

    private int _OnNumberOfPointsGot() => nPoints;

    private void Awake()
    {
        ChangableStatUI.OnNumberOfPointsGot += _OnNumberOfPointsGot;
        ChangableStatUI.OnNumberOfPointsChanged += _OnUIStatChanged;
    }

    private void OnDestroy()
    {
        ChangableStatUI.OnNumberOfPointsGot -= _OnNumberOfPointsGot;
        ChangableStatUI.OnNumberOfPointsChanged -= _OnUIStatChanged;
    }

    private void Start()
    {
        _playerStat = ChoosingLevelUnitManager.Instance?.PlayerStat ?? BattleUnitManager.Instance.PlayerAsBattlePlayerUnit.PlayerStat;

        _InitializeContents();

        if (_allButtonsEnabled)
        {
            _InitializeAllButtons();
        }
    }

    private void _InitializeContents()
    {
        _name.Content = _playerStat.Name;
        _class.Content = _playerStat.Class;
        _level.Content = _playerStat.Level.ToString();
        _gold.Content = _playerStat.Gold.ToString();

        _hp.MaxValue = _playerStat.MaxHP;
        _hp.Value = _playerStat.HP;
        _mana.MaxValue = _playerStat.MaxMana;
        _mana.Value = _playerStat.Mana;
        _stamina.MaxValue = _playerStat.MaxStamina;
        _stamina.Value = _playerStat.Stamina;

        _attack.Content = _playerStat.Attack.ToString();
        _maxHP.Content = _playerStat.MaxHP.ToString();
        _regenHP.Content = _playerStat.RegenHP.ToString();
        _maxMana.Content = _playerStat.MaxMana.ToString();
        _regenMana.Content = _playerStat.RegenMana.ToString();
        _maxStamina.Content = _playerStat.MaxStamina.ToString();
        _regenStamina.Content = _playerStat.RegenStamina.ToString();

        _nPoints.Content = _playerStat.nPoints.ToString();
        _exp.Content = _playerStat.Exp.ToString();
    }

    private void _InitializeAllButtons()
    {
        AllUpButtonsEnabled = nPoints > 0;
        AllDownButtonsEnabled = nPoints > 0;

        _GetChangableStatUI(_maxHP).Weight = 10;
        _GetChangableStatUI(_maxMana).Weight = 10;
        _GetChangableStatUI(_maxStamina).Weight = 10;

        ApplyButtonEnabled = false;

        _RecheckLevelUpButtonEnabled();
    }

    private ChangableStatUI _GetChangableStatUI(StatUI stat) => stat as ChangableStatUI;

    public bool AllUpButtonsEnabled
    {
        set
        {
            _GetChangableStatUI(_attack).UpButtonEnabled = value;
            _GetChangableStatUI(_maxHP).UpButtonEnabled = value;
            _GetChangableStatUI(_regenHP).UpButtonEnabled = value;
            _GetChangableStatUI(_maxMana).UpButtonEnabled = value;
            _GetChangableStatUI(_regenMana).UpButtonEnabled = value;
            _GetChangableStatUI(_maxStamina).UpButtonEnabled = value;
            _GetChangableStatUI(_regenStamina).UpButtonEnabled = value;
        }
    }

    public bool AllDownButtonsEnabled
    {
        set
        {
            _GetChangableStatUI(_attack).DownButtonEnabled = value;
            _GetChangableStatUI(_maxHP).DownButtonEnabled = value;
            _GetChangableStatUI(_regenHP).DownButtonEnabled = value;
            _GetChangableStatUI(_maxMana).DownButtonEnabled = value;
            _GetChangableStatUI(_regenMana).DownButtonEnabled = value;
            _GetChangableStatUI(_maxStamina).DownButtonEnabled = value;
            _GetChangableStatUI(_regenStamina).DownButtonEnabled = value;
        }
    }

    public int nPoints { set => _nPoints.Content = value.ToString(); get => int.Parse(_nPoints.Content); }

    public int Exp
    {
        set
        {
            _playerStat.Exp = value;
            _exp.Content = value.ToString();
        }

        get => _playerStat.Exp;
    }

    public int Level
    {
        set
        {
            _playerStat.Level = value;
            _level.Content = value.ToString();
        }
        get => _playerStat.Level;
    }

    public bool ApplyButtonEnabled { set => _applyButton.interactable = value; }

    public void _RecheckLevelUpButtonEnabled() => _levelupButton.interactable = _playerStat.Exp > LevelUpExpThresh;

    #region Button callbacks

    public void OnLevelupButtonClicked()
    {
        Exp -= LevelUpExpThresh;

        Level++;

        nPoints += LevelupPoints;
        _playerStat.nPoints = int.Parse(_nPoints.Content);

        AllUpButtonsEnabled = true;
        AllDownButtonsEnabled = true;

        _RecheckLevelUpButtonEnabled();

        SaveSystem.SavePlayerStat(_playerStat);
    }

    public void OnApplyClicked()
    {
        _playerStat.Attack = float.Parse(_attack.Content);
        _playerStat.MaxHP = float.Parse(_maxHP.Content);
        _playerStat.RegenHP = float.Parse(_regenHP.Content);
        _playerStat.MaxMana = float.Parse(_maxMana.Content);
        _playerStat.RegenMana = float.Parse(_regenMana.Content);
        _playerStat.MaxStamina = float.Parse(_maxStamina.Content);
        _playerStat.RegenStamina = float.Parse(_regenStamina.Content);

        _playerStat.nPoints = int.Parse(_nPoints.Content);

        ApplyButtonEnabled = false;
        AllUpButtonsEnabled = false;
        AllDownButtonsEnabled = false;

        SaveSystem.SavePlayerStat(_playerStat);
    }

    public void OnCloseClicked()
    {
        gameObject.SetActive(false);
        GameManager.Instance.SetPausing(false);
    }

    #endregion
}
