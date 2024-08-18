using System;
using UnityEngine;

public class MenuInBattlePopup : PopupTemplate
{
    [SerializeField]
    private GameObject _statsCanvas;

    public void OnBackButtonClicked()
    {
        _HidePopup();
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
        _ReturnPreviousScene();
    }

    private void _ShowStatsPopup() => _statsCanvas?.SetActive(true);
}
