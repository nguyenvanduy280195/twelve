using System;
using UnityEngine;

public class MenuInBattlePopup : PopupTemplate
{
    [SerializeField]
    private GameObject _statsCanvas;

    protected override void OnEnable()
    {
        base.OnEnable();
        BattleGameManager.Instance?.SetResuming(false);
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        BattleGameManager.Instance?.SetResuming(true);
    }

    public void OnBackButtonClicked()
    {
        HidePopup();
        AudioManager.Instance?.PlayButton();
    }

    public void OnStatsButtonClicked()
    {
        HidePopup();
        _ShowStatsPopup();
        AudioManager.Instance?.PlayButton();
    }

    public void OnInventoryButtonClicked()
    {
        AudioManager.Instance?.PlayButton();
    }

    public void OnOutBattleButtonClicked()
    {
        HidePopup();
        _ReturnPreviousScene();
        AudioManager.Instance?.PlayButton();
    }

    private void _ShowStatsPopup() => _statsCanvas?.SetActive(true);
}
