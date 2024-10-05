using System;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class BattleSceneUI : MySceneBase
{
    [SerializeField] private GameObject _menuInGame;
    [SerializeField] private SkillInfoPopup _skillInfoPopup;

    public void OnMenuInGameButtonClicked() => _menuInGame.SetActive(true);

    public void OnSkill1Clicked(Button button) => _LetPlayerExecuteSkill(0, button);

    public void OnSkill2Clicked(Button button) => _LetPlayerExecuteSkill(1, button);

    public void OnSkill3Clicked(Button button) => _LetPlayerExecuteSkill(2, button);

    private void _LetPlayerExecuteSkill(int iSkill, Button button)
    {
        if (_skillInfoPopup != null)
        {
            var player = BattleUnitManager.Instance.PlayerAsBattlePlayerUnit;
            var playerStat = player.PlayerStat;
            var skillData = playerStat.SkillData[iSkill];

            _skillInfoPopup.SkillName = skillData.Name.ToString();
            _skillInfoPopup.SkillImage = Resources.Load<Sprite>(skillData.IconPath);
            _skillInfoPopup.SkillDescribe = skillData.Describe;
            _skillInfoPopup.ManaConsumed = SkillManaConsumptionCalculator.Instance.GetManaConsumption(skillData.Name, skillData.Level);
            _skillInfoPopup.OnUsed = () =>
            {
                if (BattleGameManager.Instance?.IsPlayerTurn ?? false)
                {
                    var enemy = BattleUnitManager.Instance.EnemyAsBattleUnitBase;
                    BattleGameManager.Instance.WaitingForSkill = true;
                    Action onSkillSucceeded = () => button.interactable = false;
                    Action onSkillFailed = () => BattleGameManager.Instance.WaitingForSkill = false;
                    Action onSkillExecuted = () => BattleGameManager.Instance.WaitingForSkill = false;
                    Action onSkillDone = () => button.interactable = true;

                    player.ExecuteSkill(iSkill, enemy, onSkillSucceeded, onSkillFailed, onSkillExecuted, onSkillDone);
                }
                else
                {
                    AlertSnackbar.Instance
                                ?.SetText("This is not player's turn")
                                 .Show();
                }
                AudioManager.Instance?.PlayButton();
            };
            _skillInfoPopup.ShowPopup();
        }
    }

    public void OnExportItems()
    {
        const string filename = "Assets/Resources/Text/output.csv";
        var myData = BattleGameManager.Instance.MyData;

        using var outputFile = File.CreateText(filename);
        for (int r = myData.Items.RowLength - 1; r >= 0; r--)
        {
            var line = "";
            for (int c = 0; c < myData.Items.RowLength; c++)
            {
                line += myData.Items[c, r].tag + ",";
            }
            line = line.Remove(line.Length - 1);

            outputFile.WriteLine(line);
        }
    }

    public void OnTestcaseEditTextSubmit(string value)
    {
        if (value.Length <= 0)
        {
            BattleGameManager.Instance.InitializeItems("");
            return;
        }

        var testcaseNumber = string.Format("{0:000}", int.Parse(value));
        BattleGameManager.Instance.InitializeItems("Text/" + testcaseNumber);
    }

}
