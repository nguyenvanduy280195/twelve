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
    }

    public void OnNoButtonClicked()
    {
        NoButtonClick?.Invoke();
    }
}
