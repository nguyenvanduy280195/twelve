using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class PopupTemplate : MonoBehaviour
{
    public virtual void ShowPopup() => gameObject.SetActive(true);
    public virtual void HidePopup() => gameObject.SetActive(false);
    protected virtual void OnEnable() => GameManager.Instance?.SetPausing(true);
    protected virtual void OnDisable() => GameManager.Instance?.SetPausing(false);
    protected virtual void _ExitBattleInCasualMode() => MySceneManager.Instance?.LoadMazeScene();
    protected virtual void _ExitBattleInBattleMode() => MySceneManager.Instance?.LoadMainMenuScene();
    protected void _ReturnPreviousScene()
    {
        var gameMode = GameManager.Instance?.GetGameMode();
        switch (gameMode)
        {
            case GameMode.Battle:
                _ExitBattleInBattleMode();
                break;
            case GameMode.Casual:
                _ExitBattleInCasualMode();
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(gameMode), gameMode, null);
        }
    }
}
