using System.Linq;
using TMPro;
using UnityEngine;

public class CreatingCharacterSceneUI : MySceneBase
{

    [SerializeField] private ScriptablePlayerStat _soldier;
    [SerializeField] private TMP_InputField _name;

    public void OnEnjoyButtonClicked()
    {
        if (_name.text.Count() <= 0)
        {
            Debug.Log("Please fill your name.");
            return;
        }

        _CreateCharacter();
        MySceneManager.Instance?.LoadMazeScene();
        AudioManager.Instance?.PlayButton();
    }

    public void OnNopeButtonClicked()
    {
        MySceneManager.Instance?.LoadMainMenuScene();
        AudioManager.Instance?.PlayButton();
    }

    private void _CreateCharacter()
    {
        var playerStat = new PlayerStat(_soldier.PlayerStat);
        playerStat.Name = _name.text;
        SaveSystem.SavePlayerStat(playerStat);
    }
}
