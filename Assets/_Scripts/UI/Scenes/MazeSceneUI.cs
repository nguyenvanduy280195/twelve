using System;
using UnityEngine;

public class MazeSceneUI : MySceneBase
{
    [SerializeField] private GameObject _menuInGame;

    public void OnMenuInGameButtonClicked() => _menuInGame.SetActive(true);

    private void Start()
    {
        _SetupPlayer();
    }

    private void _SetupPlayer()
    {
        var choosingLevelUnitManager = ChoosingLevelUnitManager.Instance;
        if (choosingLevelUnitManager != null)
        {
            var player = choosingLevelUnitManager.Player;
            var playerStat = choosingLevelUnitManager.PlayerData;
            var position = playerStat.Position ?? new MyPosition();
            player.transform.position = position.ToVector3();
        }

    }

}
