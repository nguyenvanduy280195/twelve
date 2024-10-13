using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuSceneUI : MySceneBase
{
    [SerializeField] private ConfirmPopup _confirmPopup;
    [SerializeField] private MoveTo _casualMenuMoveTo;
    [SerializeField] private ScaleTo _casualMenuScaleTo;

    private Vector3 _casualMenuPositionFirst;
    private Vector3 _casualMenuLocalScaleFirst;

    private void Start()
    {
        _casualMenuPositionFirst = _casualMenuMoveTo.transform.localPosition;
        _casualMenuLocalScaleFirst = Vector3.one;

        _confirmPopup.YesButtonClick = () => MySceneManager.Instance?.LoadCreatingCharacterScene();
        _confirmPopup.NoButtonClick = () =>
        {
            var scaleTo = _confirmPopup.GetComponent<ScaleTo>();
            scaleTo.To = new Vector2(0.0001f, 0.0001f);
        };

    }
    public void OnPlayButtonClicked(ChangableImage image)
    {
        AudioManager.Instance?.PlayButton();
    }

    public void OnCasualButtonClicked()
    {
        GameManager.Instance?.SetGameMode(GameMode.Casual);

        if (_casualMenuMoveTo != null)
        {
            _casualMenuMoveTo.To = Vector2.zero;
        }
        AudioManager.Instance?.PlayButton();
    }

    public void OnContinueButtonClicked()
    {
        if (SaveSystem.PlayerDataExists)
        {
            MySceneManager.Instance?.LoadMazeScene();
        }
        else
        {
            Debug.Log("What? You don't have save game");
        }
        AudioManager.Instance?.PlayButton();
    }

    public void OnNewGameButtonClicked()
    {
        OnCloseButtonClicked();

        _confirmPopup.ShowPopup();
        var scaleTo = _confirmPopup.GetComponent<ScaleTo>();
        scaleTo.To = new Vector2(1f, 1f);

        AudioManager.Instance?.PlayButton();
    }

    public void OnCloseButtonClicked()
    {
        if (_casualMenuScaleTo)
        {
            _casualMenuScaleTo.To = new Vector2(0.001f, 0.001f);
            _casualMenuScaleTo.ActiveOnDoneOnce = true;
            _casualMenuScaleTo.OnDone = go =>
            {
                var rectTransform = go.GetComponent<RectTransform>();
                rectTransform.localPosition = _casualMenuPositionFirst;
                _casualMenuMoveTo.To = _casualMenuPositionFirst;

                rectTransform.localScale = _casualMenuLocalScaleFirst;
                _casualMenuScaleTo.To = _casualMenuLocalScaleFirst;
            };
        }
        AudioManager.Instance?.PlayButton();
    }

    public void OnBattleButtonClicked()
    {
        GameManager.Instance?.SetGameMode(GameMode.Battle);
        MySceneManager.Instance?.LoadInBattleScene();
        AudioManager.Instance?.PlayButton();
    }

}

