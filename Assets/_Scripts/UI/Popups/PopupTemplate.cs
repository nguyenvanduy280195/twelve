using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class PopupTemplate : MonoBehaviour
{
    protected virtual void OnEnable() => GameManager.Instance?.SetPausing(true);
    protected virtual void OnDisable() => GameManager.Instance?.SetPausing(false);
    protected void _HidePopup() => gameObject.SetActive(false);
    protected void _ReturnPreviousScene()
    {
        var gameMode = GameManager.Instance?.GetGameMode();
        switch (gameMode)
        {
            case GameMode.Battle:
                MySceneManager.Instance?.LoadMainMenuScene();
                break;
            case GameMode.Casual:
                MySceneManager.Instance?.LoadMazeScene();
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(gameMode), gameMode, null);
        }
    }
}
