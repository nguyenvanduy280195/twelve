using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public void OnCasualButtonClicked()
    {
        GameManager.Instance?.SetGameMode(GameMode.Casual);
        MySceneManager.Instance?.LoadMazeScene();
    }

    public void OnBattleButtonClicked()
    {
        GameManager.Instance?.SetGameMode(GameMode.Battle);
        MySceneManager.Instance?.LoadInBattleScene();
    }
}
