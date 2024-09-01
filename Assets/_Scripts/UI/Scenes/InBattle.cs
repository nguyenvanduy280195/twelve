using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class InBattle : MonoBehaviour
{
    [SerializeField] private GameObject _menuInGame;

    public void OnMenuInGameButtonClicked() => _menuInGame.SetActive(true);

    public void OnSkill1Clicked() => _LetPlayerExecuteSkill(0);

    public void OnSkill2Clicked() => _LetPlayerExecuteSkill(1);

    public void OnSkill3Clicked() => _LetPlayerExecuteSkill(2);

    private void _LetPlayerExecuteSkill(int iSkill)
    {
        if (BattleGameManager.Instance?.IsPlayerTurn ?? false)
        {
            var player = BattleUnitManager.Instance.PlayerAsBattleUnitBase;
            var enemy = BattleUnitManager.Instance.EnemyAsBattleUnitBase;
            player.ExecuteSkill(iSkill, enemy);
        }
    }

    public void OnExportItems()
    {
        const string filename = "Assets/Resources/Text/output.csv";
        var myData = BattleGameManager.Instance.MyData;

        using (StreamWriter outputFile = File.CreateText(filename))
        {
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
