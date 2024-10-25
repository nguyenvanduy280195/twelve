using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerStatPopup : PopupTemplate
{
    #region Fields

    [SerializeField] private UIStatString _name;
    [SerializeField] private UIStatString _class;
    [SerializeField] private UIStatInt _level;
    [SerializeField] private UIStatInt _gold;

    [SerializeField] private Bar _hp;
    [SerializeField] private Bar _mana;
    [SerializeField] private Bar _stamina;

    [SerializeField] private UIStatFloat _attack;
    [SerializeField] private UIStatFloat _hpMax;
    [SerializeField] private UIStatFloat _hpRegen;
    [SerializeField] private UIStatFloat _manaMax;
    [SerializeField] private UIStatFloat _manaRegen;
    [SerializeField] private UIStatFloat _staminaMax;
    [SerializeField] private UIStatFloat _staminaRegen;
    [SerializeField] private UIStatFloat _staminaConsume;


    [SerializeField] private UIChangaleStatInt _strength;
    [SerializeField] private UIChangaleStatInt _vitality;
    [SerializeField] private UIChangaleStatInt _endurance;
    [SerializeField] private UIChangaleStatInt _intelligent;
    [SerializeField] private UIChangaleStatInt _luck;

    [SerializeField] private UIStatInt _nPoints;
    [SerializeField] private UIStatInt _exp;


    [SerializeField] private Button _applyButton;
    [SerializeField] private Button _levelupButton;

    [SerializeField] private int LevelUpExpThresh = 100;
    [SerializeField] private int LevelupPoints = 5;

    [Header("For observing")]
    [SerializeField] private bool _allButtonsEnabled = true;

    private PlayerData _playerStat;

    #endregion

    private void _OnUIStatChanged(int delta)
    {
        _nPoints.Content -= delta;
        if (_applyButton != null)
        {
            _applyButton.interactable = true;
        }
    }

    private int _OnNumberOfPointsGot() => _nPoints.Content;

    private void Awake()
    {
        UIChangaleStatInt.OnNumberOfPointsGot += _OnNumberOfPointsGot;
        UIChangaleStatInt.OnNumberOfPointsChanged += _OnUIStatChanged;
    }

    private void OnDestroy()
    {
        UIChangaleStatInt.OnNumberOfPointsGot -= _OnNumberOfPointsGot;
        UIChangaleStatInt.OnNumberOfPointsChanged -= _OnUIStatChanged;
    }

    private void Start()
    {
        _playerStat = UnitManager.Instance?.PlayerData ?? BattleUnitManager.Instance?.PlayerAsBattlePlayerUnit.PlayerStat;

        _InitializeContents();

        if (_allButtonsEnabled)
        {
            _InitializeAllButtons();
        }
    }

    private void _InitializeContents()
    {
        _name.Content = _playerStat.Name;
        _name.OnContentChanged = newValue => _playerStat.Name = newValue;

        _class.Content = _playerStat.Class.ToString();
        //_class.OnContentChanged = newValue => _playerStat.Class = newValue;

        _level.Content = _playerStat.Level;
        _level.OnContentChanged = newValue =>
        {
            _playerStat.Level = newValue;
            _staminaConsume.Content = 0.1f * newValue;
        };

        _gold.Content = _playerStat.Gold;
        _gold.OnContentChanged = newValue => _playerStat.Gold = newValue;

        _hp.MaxValue = _playerStat.HPMax;
        _hp.Value = _playerStat.HP;
        _mana.MaxValue = _playerStat.ManaMax;
        _mana.Value = _playerStat.Mana;
        _stamina.MaxValue = _playerStat.StaminaMax;
        _stamina.Value = _playerStat.Stamina;

        _attack.Content = _playerStat.Attack;
        _attack.OnContentChanged = newValue => _playerStat.Attack = newValue;

        _hpMax.Content = _playerStat.HPMax;
        _hpMax.OnContentChanged = newValue => _playerStat.HPMax = newValue;

        _hpRegen.Content = _playerStat.HPRegen;
        _hpRegen.OnContentChanged = newValue => _playerStat.HPRegen = newValue;

        _manaMax.Content = _playerStat.ManaMax;
        _manaMax.OnContentChanged = newValue => _playerStat.ManaMax = newValue;

        _manaRegen.Content = _playerStat.ManaRegen;
        _hpRegen.OnContentChanged = newValue => _playerStat.ManaRegen = newValue;

        _staminaMax.Content = _playerStat.StaminaMax;
        _staminaMax.OnContentChanged = newValue => _playerStat.StaminaMax = newValue;

        _staminaRegen.Content = _playerStat.StaminaRegen;
        _staminaRegen.OnContentChanged = newValue => _playerStat.StaminaRegen = newValue;

        _staminaConsume.Content = _playerStat.StaminaConsumeWeight;
        _staminaConsume.OnContentChanged = newValue => _playerStat.StaminaConsumeWeight = newValue;

        _strength.Content = _playerStat.Strength;
        _strength.OnContentChanged = value =>
        {
            _playerStat.Strength = value;
            _playerStat.Attack = 0.5f * value;
            _attack.Content = 0.5f * value;
        };

        _vitality.Content = _playerStat.Vitality;
        _vitality.OnContentChanged = value =>
        {
            _playerStat.Vitality = value;
            _playerStat.HPMax = 10f * value;
            _playerStat.HPRegen = 0.1f * value;
            _hpMax.Content = 10f * value;
            _hpRegen.Content = 0.1f * value;
        };

        _endurance.Content = _playerStat.Endurance;
        _endurance.OnContentChanged = value =>
        {
            _playerStat.Endurance = value;
            _playerStat.StaminaMax = 10f * value;
            _playerStat.StaminaRegen = 0.1f * value;
            _staminaMax.Content = 10f * value;
            _staminaRegen.Content = 0.1f * value;
        };

        _intelligent.Content = _playerStat.Intelligent;
        _intelligent.OnContentChanged = value =>
        {
            _playerStat.Intelligent = value;
            _playerStat.ManaMax = 10f * value;
            _playerStat.ManaRegen = 0.1f * value;
            _manaMax.Content = 10f * value;
            _manaRegen.Content = 0.1f * value;
        };

        _luck.Content = _playerStat.Luck;
        _luck.OnContentChanged = value => _playerStat.Luck = value;

        _nPoints.Content = _playerStat.nPoints;
        _nPoints.OnContentChanged = value => _playerStat.nPoints = value;

        _exp.Content = _playerStat.Exp;
        _exp.OnContentChanged = value => _playerStat.Exp = value;
    }

    private void _InitializeAllButtons()
    {
        _AllUpButtonsEnabled = _nPoints.Content > 0;
        _AllDownButtonsEnabled = _nPoints.Content > 0;

        if (_applyButton)
        {
            _applyButton.interactable = false;
        }

        if (_levelupButton != null)
        {
            _RecheckLevelUpButtonEnabled();
        }
    }

    private void _RecheckLevelUpButtonEnabled() => _levelupButton.interactable = _playerStat.Exp > LevelUpExpThresh;

    private bool _AllUpButtonsEnabled
    {
        set
        {
            _strength.UpButtonEnabled = value;
            _vitality.UpButtonEnabled = value;
            _endurance.UpButtonEnabled = value;
            _intelligent.UpButtonEnabled = value;
            _luck.UpButtonEnabled = value;
        }
    }

    private bool _AllDownButtonsEnabled
    {
        set
        {
            _strength.DownButtonEnabled = value;
            _vitality.DownButtonEnabled = value;
            _endurance.DownButtonEnabled = value;
            _intelligent.DownButtonEnabled = value;
            _luck.DownButtonEnabled = value;
        }
    }
    #region Button callbacks

    public void OnLevelupButtonClicked()
    {
        _exp.Content -= LevelUpExpThresh;

        _level.Content++;

        _nPoints.Content += LevelupPoints;
        _playerStat.nPoints = _nPoints.Content;

        _AllUpButtonsEnabled = true;
        _AllDownButtonsEnabled = true;

        if (_levelupButton != null)
        {
            _RecheckLevelUpButtonEnabled();
        }

        SaveSystem.SavePlayerStat(_playerStat);
    }

    public void OnApplyClicked()
    {
        if (_applyButton != null)
        {
            _applyButton.interactable = false;
        }
        _AllUpButtonsEnabled = false;
        _AllDownButtonsEnabled = false;

        SaveSystem.SavePlayerStat(_playerStat);
        AudioManager.Instance?.PlayButton();
    }

    public void OnCloseClicked()
    {
        HidePopup();
        AudioManager.Instance?.PlayButton();
    }



    #endregion
}
