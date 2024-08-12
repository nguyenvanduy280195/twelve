using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuInGamePopup : MonoBehaviour
{
    [SerializeField]
    private GameObject _statsCanvas;


    public void OnBackButtonClicked()
    {
        gameObject.SetActive(false);
    }

    public void OnStatsButtonClicked()
    {
        gameObject.SetActive(false);
        _statsCanvas.SetActive(true);
    }

    public void OnInventoryButtonClicked()
    {

    }

    public void OnMainMenuButtonClicked()
    {

    }
}
