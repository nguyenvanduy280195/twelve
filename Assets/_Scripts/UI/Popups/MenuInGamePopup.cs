using UnityEngine;

public class MenuInGamePopup : PopupTemplate
{
    [SerializeField]
    private GameObject _statsCanvas;

    public void OnBackButtonClicked() => HidePopup();

    public void OnStatsButtonClicked()
    {
        HidePopup();
        _ShowStatsPopup();
    }

    public void OnInventoryButtonClicked() { }

    public void OnMainMenuButtonClicked()
    {
        HidePopup();
        MySceneManager.Instance?.LoadMainMenuScene();
    }

    private void _ShowStatsPopup() => _statsCanvas?.SetActive(true);
}
