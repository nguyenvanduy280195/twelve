using System;
using UnityEngine;

public class MenuInBattlePopup : MonoBehaviour
{
    [SerializeField]
    private GameObject _statsCanvas;

    public void OnBackButtonClicked()
    {
        _HidePopup();
        _UnPauseGame();
    }

    public void OnStatsButtonClicked()
    {
        _HidePopup();
        _ShowStatsPopup();
    }

    public void OnInventoryButtonClicked()
    {

    }

    public void OnOutBattleButtonClicked()
    {
        _HidePopup();
        _UnPauseGame();

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

    private void OnEnable() => GameManager.Instance?.SetPausing(true);

    private void _UnPauseGame() => GameManager.Instance?.SetPausing(false);

    private void _HidePopup() => gameObject.SetActive(false);

    private void _ShowStatsPopup() => _statsCanvas?.SetActive(true);
}
