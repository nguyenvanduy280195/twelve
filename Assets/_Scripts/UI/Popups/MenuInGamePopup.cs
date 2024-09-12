using UnityEngine;

public class MenuInGamePopup : PopupTemplate
{
    [SerializeField]
    private GameObject _statsCanvas;

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

    public void OnMainMenuButtonClicked()
    {
        HidePopup();
        MySceneManager.Instance?.LoadMainMenuScene();
        AudioManager.Instance?.PlayButton();
    }

    private void _ShowStatsPopup() => _statsCanvas?.SetActive(true);
}
