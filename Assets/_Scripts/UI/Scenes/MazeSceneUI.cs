using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MazeSceneUI : Singleton<MySceneBase>
{
    [SerializeField] private GameObject _menuInGame;

    public void OnMenuInGameButtonClicked() => _menuInGame.SetActive(true);

    private void Start()
    {
        _SetPlayerPreviousPosition();
    }


    private void _SetPlayerPreviousPosition()
    {
        var choosingLevelUnitManager = ChoosingLevelUnitManager.Instance;
        if (choosingLevelUnitManager != null)
        {
            var player = choosingLevelUnitManager.Player;
            var playerStat = choosingLevelUnitManager.PlayerStat;
            var position = playerStat.Position ?? new MyPosition();
            player.transform.position = position.ToVector3();
        }

    }

}
