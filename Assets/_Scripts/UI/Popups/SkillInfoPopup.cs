using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

// Builder pattern is too difficult to debug
// For details:
// In case the program occurs an exception, it only alerts generally
// For example:
// ----------------------------------------
// SkillInfoPopup.Instance? 
// .SetSkillName(skillData.Name.ToString())
// .SetSkillImage(Resources.Load<Sprite>(skillData.IconPath))
// .SetSkillDescribe(skillData.Describe)
// .SetManaConsumed(SkillManaConsumptionCalculator.Instance.GetManaConsumption(skillData.Name, skillData.Level))
// .SetOnUsed(() =>
// {
//     if (BattleGameManager.Instance?.IsPlayerTurn ?? false)
//     {
//         var enemy = BattleUnitManager.Instance.EnemyAsBattleUnitBase;
//         BattleGameManager.Instance.WaitingForSkill = true;
//         Action onSkillSucceeded = () => _button.interactable = false;
//         Action onSkillFailed = () => BattleGameManager.Instance.WaitingForSkill = false;
//         Action onSkillExecuted = () => BattleGameManager.Instance.WaitingForSkill = false;
//         Action onSkillDone = () => _button.interactable = true;
//         player.ExecuteSkill(_iSkill, enemy, onSkillSucceeded, onSkillFailed, onSkillExecuted, onSkillDone);
//     }
//     else
//     {
//         AlertSnackbar.Instance?
//                     .SetText("This is not player's turn")
//                     .Show();
//     }
// })
// .Show();
// ----------------------------------------------------
// if SkillManaConsumptionCalculator.Instance is null, it just alerts at the first dot
public class SkillInfoPopup : Singleton<SkillInfoPopup>
{
    [SerializeField] private TextMeshProUGUI _skillName;
    [SerializeField] private Image _skillImage;
    [SerializeField] private TextMeshProUGUI _describe;
    [SerializeField] private TextMeshProUGUI _manaConsumed;
    [SerializeField] private MoveTo _content;
    [SerializeField] private GameObject _background;
    [SerializeField] private Vector3 _contentStartPosition;
    private Action _onUsed;
    private bool _showing = false;

    public bool Showing => _showing;

    private void Start()
    {
        _contentStartPosition = _content.transform.localPosition;
    }

    public void SetSkillName(string value)
    {
        _skillName.text = value;
    }

    public void SetSkillImage(Sprite value)
    {
        _skillImage.sprite = value;
    }

    public void SetSkillDescribe(string value)
    {
        _describe.text = value;
    }

    public void SetManaConsumed(float value)
    {
        _manaConsumed.text = value.ToString();
    }

    public void SetOnUsed(Action value)
    {
        _onUsed = value;
    }

    public void Show()
    {
        GameManager.Instance?.SetPausing(true);
        _background?.SetActive(true);
        _content.To = Vector3.zero;
        _showing = true;
    }

    public void Hide()
    {
        GameManager.Instance?.SetPausing(false);
        _background?.SetActive(false);
        _content.To = _contentStartPosition;
        _showing = false;
    }

    public void OnUseButtonClick()
    {
        _onUsed?.Invoke();

        AudioManager.Instance?.PlayButton();
        Hide();
    }

    public void OnCloseButtonClick() => Hide();

}
