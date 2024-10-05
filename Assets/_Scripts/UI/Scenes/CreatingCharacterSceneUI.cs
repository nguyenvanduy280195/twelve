using System;
using System.Linq;
using TMPro;
using UnityEngine;


public class CreatingCharacterSceneUI : Singleton<CreatingCharacterSceneUI>
{
    [SerializeField] private TMP_InputField _playerName;

    private PlayerData _playerData;

    public void SetPlayerData(PlayerData value) => _playerData = value;

    public void OnEnjoyButtonClicked()
    {
        if (_playerName.text.Count() <= 0)
        {
            AlertSnackbar.Instance
                        ?.SetText("Please fill a name for your character")
                         .Show();
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
        if(_playerData == null)
        {
            AlertSnackbar.Instance
                        ?.SetText("Please choose class for your character")
                         .Show();
            return false;
        }

        try
        {
            var playerStat = new PlayerData(_playerData);
            playerStat.Name = _playerName.text;
            SaveSystem.SavePlayerStat(playerStat);
        }
        catch (Exception e)
        {
            Debug.Log($"Creating character fails - {e}");
            return false;
        }
        Debug.Log("Creating character successes");
        return true;
    }
}
