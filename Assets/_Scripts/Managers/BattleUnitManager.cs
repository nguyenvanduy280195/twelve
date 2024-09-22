using System;
using UnityEngine;

public class BattleUnitManager : Singleton<BattleUnitManager>
{
    [Serializable]
    public class UnitInfo
    {
        public Transform AttackPosition;
        public Transform Container;
        public ScriptableUnitStat StatDefault;
    }


    [SerializeField] private UnitInfo _playerInfo;
    [SerializeField] private UnitInfo _enemyInfo;

    private GameObject _player;
    private GameObject _enemy;

    public Vector3 PlayerAttackPosition => _playerInfo.AttackPosition.position;
    public BattlePlayerUnit PlayerAsBattlePlayerUnit => _player.GetComponent<BattlePlayerUnit>();
    public BattleUnitBase PlayerAsBattleUnitBase => PlayerAsBattlePlayerUnit;


    public Vector3 EnemyAttackPosition => _enemyInfo.AttackPosition.position;
    public BattleEnemyUnit EnemyAsBattleEnemyUnit => _enemy.GetComponent<BattleEnemyUnit>();
    public BattleUnitBase EnemyAsBattleUnitBase => EnemyAsBattleEnemyUnit;

    private void Start()
    {
        _SetupBattleUnits();

        var gameMode = GameManager.Instance?.GetGameMode() ?? _GetGameMode();
        switch (gameMode)
        {
            case GameMode.Battle:
                _SetupBattleMode();
                break;
            case GameMode.Casual:
                _SetupCasualMode();
                break;
            default:
                break;
        }
    }

    private GameMode _GetGameMode() => ChoosingLevelUnitManager.Instance != null ? GameMode.Casual : GameMode.Battle;

    private void _SetupBattleMode()
    {
        if (_playerInfo.StatDefault is ScriptablePlayerStat scriptablePlayerStat)
        {
            PlayerAsBattleUnitBase.Stat = scriptablePlayerStat?.PlayerStat.Clone();
        }
        if (_enemyInfo.StatDefault is ScriptableEnemyStat scriptableEnemyStat)
        {
            EnemyAsBattleUnitBase.Stat = scriptableEnemyStat?.EnemyStat.Clone();
        }
    }

    private void _SetupCasualMode()
    {
        var manager = MatchingBattleManager.Instance;
        PlayerAsBattleUnitBase.Stat = manager?.PlayerStat;
        EnemyAsBattleUnitBase.Stat = manager?.EnemyStat;
    }

    private void _SetupBattleUnits()
    {
        var manager = MatchingBattleManager.Instance;
        if (manager != null)
        {
            _player = UnitFactory.CreateBattleUnit(manager.PlayerStat.Class, _playerInfo.Container);
            _enemy = UnitFactory.CreateBattleUnit(manager.EnemyStat.Class, _enemyInfo.Container);
        }
        else
        {
            _player = Instantiate(PrefabManager.Instance.GetDefaultUnitPrefab(UnitDefaultType.BattleHuman), _playerInfo.Container);
            _enemy = Instantiate(PrefabManager.Instance.GetDefaultUnitPrefab(UnitDefaultType.BattleSkeleton), _enemyInfo.Container);
        }
    }

}
