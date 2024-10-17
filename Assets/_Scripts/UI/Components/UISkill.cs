using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class UISkill : MonoBehaviour
{
    [SerializeField] private int _iSkill;

    private Button _button;

    private void Start()
    {
        _button = GetComponent<Button>();
    }

    public void OnSkillClicked()
    {
        StartCoroutine(_ExecuteTheSkill());
    }

    private IEnumerator _ExecuteTheSkill()
    {
        yield return new WaitUntil(() => SkillInfoPopup.Instance != null);
        var skillInfoPopup = SkillInfoPopup.Instance;

        yield return new WaitUntil(() => BattleUnitManager.Instance.PlayerAsBattlePlayerUnit != null);
        var player = BattleUnitManager.Instance.PlayerAsBattlePlayerUnit;

        var playerStat = player.PlayerStat;
        var skillData = playerStat.SkillData[_iSkill];

        SkillInfoPopup.Instance?
                    .SetSkillName(skillData.Name.ToString())
                    .SetSkillImage(Resources.Load<Sprite>(skillData.IconPath))
                    .SetSkillDescribe(skillData.Describe)
                    .SetManaConsumed(SkillManaConsumptionCalculator.Instance.GetManaConsumption(skillData.Name, skillData.Level))
                    .SetOnUsed(() =>
                    {
                        if (BattleGameManager.Instance?.IsPlayerTurn ?? false)
                        {
                            var enemy = BattleUnitManager.Instance.EnemyAsBattleUnitBase;
                            BattleGameManager.Instance.WaitingForSkill = true;
                            Action onSkillSucceeded = () => _button.interactable = false;
                            Action onSkillFailed = () => BattleGameManager.Instance.WaitingForSkill = false;
                            Action onSkillExecuted = () => BattleGameManager.Instance.WaitingForSkill = false;
                            Action onSkillDone = () => _button.interactable = true;

                            player.ExecuteSkill(_iSkill, enemy, onSkillSucceeded, onSkillFailed, onSkillExecuted, onSkillDone);
                        }
                        else
                        {
                            AlertSnackbar.Instance?
                                        .SetText("This is not player's turn")
                                        .Show();
                        }
                    })
                    .Show();
    }
}