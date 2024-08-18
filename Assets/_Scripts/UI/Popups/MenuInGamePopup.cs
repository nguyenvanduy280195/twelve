using UnityEngine;

public class MenuInGamePopup : PopupTemplate
{
    [SerializeField]
    private GameObject _statsCanvas;

    public void OnBackButtonClicked() => _HidePopup();

    public void OnStatsButtonClicked()
    {
        _HidePopup();
        _ShowStatsPopup();
    }

    public void OnInventoryButtonClicked() { }

    public void OnMainMenuButtonClicked()
    {
        _HidePopup();
        MySceneManager.Instance?.LoadMainMenuScene();
    }

    private void _ShowStatsPopup() => _statsCanvas?.SetActive(true);
}
