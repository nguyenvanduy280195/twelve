using System;
using TMPro;
using UnityEngine;

public class AlertPopup : Singleton<AlertPopup>
{
    [SerializeField] private TextMeshProUGUI _title;
    [SerializeField] private TextMeshProUGUI _message;

    private Vector3 _firstPosition;
    private MoveTo _moveTo;
    private Action _onYesButtonClicked;
    private Action _onNoButtonClicked;

    private void Start()
    {
        _moveTo = GetComponent<MoveTo>();
        _firstPosition = transform.localPosition;
    }

    public AlertPopup SetTitle(string value)
    {
        _title.text = value;
        return this;
    }

    public AlertPopup SetMessage(string value)
    {
        _message.text = value;
        return this;
    }

    public AlertPopup SetOnYesButtonClicked(Action onClick)
    {
        _onYesButtonClicked = onClick;
        return this;
    }

    public AlertPopup SetOnNoButtonClicked(Action onClick)
    {
        _onNoButtonClicked = onClick;
        return this;
    }

    public AlertPopup Show()
    {
        _moveTo.To = Vector3.zero;
        return this;
    }

    public void OnYesButtonClicked()
    {
        _onYesButtonClicked?.Invoke();
        _Hide();
        AudioManager.Instance?.PlayButton();
    }

    public void OnNoButtonClicked()
    {
        _onNoButtonClicked?.Invoke();
        _Hide();
        AudioManager.Instance?.PlayButton();
    }

    private void _Hide()
    {
        _moveTo.To = _firstPosition;
    }
}
