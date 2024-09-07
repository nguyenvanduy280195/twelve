using System;
using UnityEngine;

public class MenuInBattlePopup : PopupTemplate
{
    [SerializeField]
    private GameObject _statsCanvas;

    public void OnBackButtonClicked()
    {
        HidePopup();
    }

    public void OnStatsButtonClicked()
    {
        HidePopup();
        _ShowStatsPopup();
    }

    public void OnInventoryButtonClicked()
    {

    }

    public void OnOutBattleButtonClicked()
    {
        HidePopup();
        _ReturnPreviousScene();
    }

    private void _ShowStatsPopup() => _statsCanvas?.SetActive(true);
}
