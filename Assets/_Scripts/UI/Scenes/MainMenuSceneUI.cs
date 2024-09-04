using UnityEngine;

public class MainMenuSceneUI : MonoBehaviour
{
    [SerializeField] private MoveTo _casualMenuMoveTo;
    [SerializeField] private ScaleTo _casualMenuScaleTo;

    private Vector3 _casualMenuPositionFirst;
    private Vector3 _casualMenuLocalScaleFirst;

    private void Start()
    {
        _casualMenuPositionFirst = _casualMenuMoveTo.transform.position;
        _casualMenuLocalScaleFirst = Vector3.one;
    }

    public void OnCasualButtonClicked()
    {
        GameManager.Instance?.SetGameMode(GameMode.Casual);

        if (_casualMenuMoveTo != null)
        {
            _casualMenuMoveTo.To = Vector2.zero;
        }
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

    }

    public void OnNewGameButtonClicked()
    {
        MySceneManager.Instance?.LoadCreatingCharacterScene();
    }

    public void OnCloseButtonClicked()
    {
        if (_casualMenuScaleTo)
        {
            _casualMenuScaleTo.To = new Vector2(0.001f, 0.001f);
            _casualMenuScaleTo.ActiveOnDoneOnce = true;
            _casualMenuScaleTo.OnDone = go =>
            {
                Debug.Log("_casualMenuScaleTo");
                var rectTransform = go.GetComponent<RectTransform>();
                rectTransform.position = _casualMenuPositionFirst;
                rectTransform.localScale = _casualMenuLocalScaleFirst;

                _casualMenuMoveTo.To = _casualMenuPositionFirst;
                _casualMenuScaleTo.To = _casualMenuLocalScaleFirst;
            };
        }
    }

    public void OnBattleButtonClicked()
    {
        GameManager.Instance?.SetGameMode(GameMode.Battle);
        MySceneManager.Instance?.LoadInBattleScene();
    }
}
