using System;
using System.Linq;
using TMPro;
using UnityEngine;


public class CreatingCharacterSceneUI : Singleton<CreatingCharacterSceneUI>
{
    public ScriptablePlayerStat Player { set; private get; }
    [SerializeField] private TMP_InputField _name;

    public void OnEnjoyButtonClicked()
    {
        if (_name.text.Count() <= 0)
        {
            Debug.Log("Please fill your name.");
            return;
        }

        if (_CreateCharacter())
        {
            MySceneManager.Instance?.LoadMazeScene();
            AudioManager.Instance?.PlayButton();
        }
    }

    public void OnNopeButtonClicked()
    {
        MySceneManager.Instance?.LoadMainMenuScene();
        AudioManager.Instance?.PlayButton();
    }

    private bool _CreateCharacter()
    {
        try
        {
            var playerStat = new PlayerStat(Player.PlayerStat);
            playerStat.Name = _name.text;
            SaveSystem.SavePlayerStat(playerStat);
        }
        catch (Exception e)
        {
            Debug.Log($"Creating character fails. {e}");
            return false;
        }
        Debug.Log("Creating character successes");
        return true;
    }
}
