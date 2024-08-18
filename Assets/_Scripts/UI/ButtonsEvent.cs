using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonsEvent : MonoBehaviour
{
    [SerializeField]
    private GameObject _menuInGame;

    public void OnMenuInGameButtonClicked() => _menuInGame.SetActive(true);
}
