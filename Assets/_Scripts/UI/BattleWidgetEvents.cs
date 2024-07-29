using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class BattleWidgetEvents : MonoBehaviour
{
    [SerializeField]
    private UIPlayerStat _uiPlayerStat;

    public void OnStatButtonClicked()
    {
        if (_uiPlayerStat is not null)
        {
            _uiPlayerStat.gameObject.SetActive(true);
            _uiPlayerStat.nPoints = 0;
            _uiPlayerStat.ApplyButtonEnabled = false;
            _uiPlayerStat.AllUpButtonsEnabled = false;
            _uiPlayerStat.AllDownButtonsEnabled = false;
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
