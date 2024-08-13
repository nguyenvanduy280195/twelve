using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public void OnCasualButtonClicked() => MySceneManager.Instance.LoadMazeScene();

    public void OnBattleButtonClicked() => MySceneManager.Instance.LoadInBattleScene();

}
