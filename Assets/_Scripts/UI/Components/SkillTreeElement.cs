using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class SkillTreeElement : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _level;
    [SerializeField] private TextMeshProUGUI _name;
    [SerializeField] private Image _icon;
    [SerializeField] private TextMeshProUGUI _describe;
    [SerializeField] private TextMeshProUGUI _damage;
    [SerializeField] private TextMeshProUGUI _manaConsumption;
    [SerializeField] private TextMeshProUGUI _nextLevel;

    private SkillData _skillData;

    private int _goldToLevelup;

    public SkillData SkillData
    {
        set
        {
            _skillData = value;
            _level.text = _skillData.Level.ToString();
            _name.text = _skillData.Name.ToString();
            _icon.sprite = Resources.Load<Sprite>(_skillData.IconPath);
            _describe.text = _skillData.Describe;

            _UpdateDamage();
            _UpdateManaConsumption();
            _UpdateGoldToLevelup();
        }
        get => _skillData;
    }

    public int Level
    {
        set
        {
            _skillData.Level = value;
            _level.text = _skillData.Level.ToString();

            _UpdateDamage();
            _UpdateManaConsumption();
            _UpdateGoldToLevelup();
        }
        get => _skillData.Level;
    }

    private void _UpdateDamage()
    {
        var playerData = ChoosingLevelUnitManager.Instance.PlayerData;
        var damage = SkillDamageCalculator.Instance?.GetDamage(_skillData.Name, playerData.Attack, _skillData.Level) ?? -2f;
        _damage.text = damage.ToString();
    }

    private void _UpdateManaConsumption()
    {
        var manaConsumption = SkillManaConsumptionCalculator.Instance?.GetManaConsumption(_skillData.Name, _skillData.Level) ?? -2f;
        _manaConsumption.text = manaConsumption.ToString();
    }

    private void _UpdateGoldToLevelup()
    {
        _goldToLevelup = GoldToLevelUpCalculator.Instance?.GetNextLevel(_skillData.Name, _skillData.Level) ?? -2;
        _nextLevel.text = _goldToLevelup.ToString();
    }

    public void OnLevelUpButtonClicked()
    {
        var playerData = ChoosingLevelUnitManager.Instance.PlayerData;
        if (playerData.Gold >= _goldToLevelup)
        {
            playerData.Gold -= _goldToLevelup;

            Level++;

            SaveSystem.SavePlayerStat(playerData);
        }
        else
        {
            AlertSnackbar.Instance
                        ?.SetText("Not enough gold to level up")
                         .Show();
        }
        AudioManager.Instance?.PlayButton();
    }
}
