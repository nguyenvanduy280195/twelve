using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuInBattlePopup : MonoBehaviour
{
    [SerializeField]
    private GameObject _statsCanvas;


    public void OnBackButtonClicked()
    {
        gameObject.SetActive(false);
        GameManager.Instance.Pausing = false;
    }

    public void OnStatsButtonClicked()
    {
        gameObject.SetActive(false);
        _statsCanvas.SetActive(true);
    }

    public void OnInventoryButtonClicked()
    {

    }

    public void OnOutBattleButtonClicked()
    {
        MatchingBattleManager.Instance?.EndBattle();
    }
}
