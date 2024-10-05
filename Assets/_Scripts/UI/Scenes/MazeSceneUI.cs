using System;
using UnityEngine;

public class MazeSceneUI : MySceneBase
{
    public static event Action OnEnabled;
    public static event Action OnStarted;

    [SerializeField] private GameObject _menuInGame;

    public void OnMenuInGameButtonClicked() => _menuInGame.SetActive(true);

    protected override void Awake()
    {
        Debug.Log("MazeSceneUI.Awake()");
        base.Awake();
    }

    private void OnEnable() => OnEnabled?.Invoke();

    private void Start()
    {
        _SetupPlayer();
        OnStarted?.Invoke();
    }

    private void _SetupPlayer()
    {
        var choosingLevelUnitManager = ChoosingLevelUnitManager.Instance;
        if (choosingLevelUnitManager != null)
        {
            var player = choosingLevelUnitManager.Player;
            var playerStat = choosingLevelUnitManager.PlayerData;
            var position = MatchingBattleManager.Instance?.PlayerPosition ?? playerStat.Position.ToVector3();
            player.transform.position = position;
        }

    }

}
