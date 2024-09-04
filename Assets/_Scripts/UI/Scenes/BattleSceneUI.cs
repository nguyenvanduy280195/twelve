using System;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class BattleSceneUI : MonoBehaviour
{
    [SerializeField] private GameObject _menuInGame;

    public void OnMenuInGameButtonClicked() => _menuInGame.SetActive(true);

    public void OnSkill1Clicked(Button button) => _LetPlayerExecuteSkill(0, button);

    public void OnSkill2Clicked(Button button) => _LetPlayerExecuteSkill(1, button);

    public void OnSkill3Clicked(Button button) => _LetPlayerExecuteSkill(2, button);

    private void _LetPlayerExecuteSkill(int iSkill, Button button)
    {
        if (BattleGameManager.Instance?.IsPlayerTurn ?? false)
        {
            button.interactable = false;
            var player = BattleUnitManager.Instance.PlayerAsBattleUnitBase;
            var enemy = BattleUnitManager.Instance.EnemyAsBattleUnitBase;
            Action onSkillExecuted = () =>
            {
                BattleGameManager.Instance?.SkipTurn();
                button.interactable = true;
            };
            player.ExecuteSkill(iSkill, enemy, onSkillExecuted);
        }
        else
        {
            Debug.Log("This is not player's turn");
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
