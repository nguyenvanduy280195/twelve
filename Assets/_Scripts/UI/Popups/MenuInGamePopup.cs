using UnityEngine;

public class MenuInGamePopup : MonoBehaviour
{
    [SerializeField]
    private GameObject _statsCanvas;

    public void OnBackButtonClicked()
    {
        _HidePopup();
        _UnPauseGame();
    }

    public void OnStatsButtonClicked()
    {
        _HidePopup();
        _ShowStatsPopup();
    }

    public void OnInventoryButtonClicked()
    {
        _HidePopup();
    }

    public void OnMainMenuButtonClicked()
    {
        _HidePopup();
        _UnPauseGame();
        MySceneManager.Instance?.LoadMainMenuScene();
    }

    private void _UnPauseGame() => GameManager.Instance.SetPausing(false);

    private void _HidePopup() => gameObject.SetActive(false);

    private void _ShowStatsPopup() => _statsCanvas?.SetActive(true);
}
