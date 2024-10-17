using System;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class BattleSceneUI : MySceneBase
{
    [SerializeField] private GameObject _menuInGame;
    [SerializeField] private SkillInfoPopup _skillInfoPopup;

    public void OnMenuInGameButtonClicked() => _menuInGame.SetActive(true);

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
