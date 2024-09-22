using UnityEngine;

public class MenuInGamePopup : PopupTemplate
{
    [SerializeField] private PopupTemplate _statsPopup;
    [SerializeField] private PopupTemplate _skillTreePopup;

    public void OnBackButtonClicked()
    {
        HidePopup();
        AudioManager.Instance?.PlayButton();
    }

    public void OnStatsButtonClicked()
    {
        HidePopup();
        _statsPopup.ShowPopup();
        AudioManager.Instance?.PlayButton();
    }

    public void OnSkillTreeButtonClicked()
    {
        HidePopup();
        _skillTreePopup.ShowPopup();
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
}
