using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MazeSceneUI : MonoBehaviour
{
    [SerializeField]
    private GameObject _menuInGame;

    public void OnMenuInGameButtonClicked() => _menuInGame.SetActive(true);
}
