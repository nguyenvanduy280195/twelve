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
        HidePopup();
        AlertPopup.Instance
                  .SetTitle("Test title")
                  .SetMessage("Test message")
                  .SetOnYesButtonClicked(() => Debug.Log("Test yes button of AlertPopup"))
                  .SetOnNoButtonClicked(() => Debug.Log("Test no button of AlertPopup"))
                  .Show();
        AudioManager.Instance?.PlayButton();
    }

    public void OnMainMenuButtonClicked()
    {
        HidePopup();
        MySceneManager.Instance?.LoadMainMenuScene();
        AudioManager.Instance?.PlayButton();
    }
}
