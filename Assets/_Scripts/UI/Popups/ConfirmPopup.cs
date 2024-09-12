using System;
using UnityEngine;
using UnityEngine.UI;

public class ConfirmPopup : PopupTemplate
{
    public Action YesButtonClick;
    public Action NoButtonClick;

    public void OnYesButtonClicked()
    {
        YesButtonClick?.Invoke();
        AudioManager.Instance?.PlayButton();
    }

    public void OnNoButtonClicked()
    {
        NoButtonClick?.Invoke();
        AudioManager.Instance?.PlayButton();
    }
}
