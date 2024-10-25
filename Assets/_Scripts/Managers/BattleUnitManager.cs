using System;
using System.Collections;
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

    public GameObject Player => _player;
    public Vector3 PlayerAttackPosition => _playerInfo.AttackPosition.position;
    public BattlePlayerUnit PlayerAsBattlePlayerUnit => _player.GetComponent<BattlePlayerUnit>();
    public BattleUnitBase PlayerAsBattleUnitBase => PlayerAsBattlePlayerUnit;


    public GameObject Enemy => _enemy;
    public Vector3 EnemyAttackPosition => _enemyInfo.AttackPosition.position;
    public BattleEnemyUnit EnemyAsBattleEnemyUnit => _enemy.GetComponent<BattleEnemyUnit>();
    public BattleUnitBase EnemyAsBattleUnitBase => EnemyAsBattleEnemyUnit;

    private void Start()
    {
        StartCoroutine(_Start());
    }

    private IEnumerator _Start()
    {
        yield return _SetupBattleUnits();

        var gameMode = GameManager.Instance?.GetGameMode() ?? _GetGameMode();
        switch (gameMode)
        {
            case GameMode.Battle:
                yield return _SetupBattleMode();
                break;
            case GameMode.Casual:
                yield return _SetupCasualMode();
                break;
            default:
                break;
        }
    }

    private GameMode _GetGameMode() => ChoosingLevelUnitManager.Instance != null ? GameMode.Casual : GameMode.Battle;

    private IEnumerator _SetupBattleMode()
    {
        yield return new WaitUntil(() => _player != null && _enemy != null);

        if (_playerInfo.StatDefault is ScriptablePlayerData scriptablePlayerStat)
        {
            PlayerAsBattleUnitBase.Stat = scriptablePlayerStat?.PlayerData.Clone();
        }
        if (_enemyInfo.StatDefault is ScriptableEnemyStat scriptableEnemyStat)
        {
            EnemyAsBattleUnitBase.Stat = scriptableEnemyStat?.EnemyStat.Clone();
        }
    }

    private IEnumerator _SetupCasualMode()
    {
        yield return new WaitUntil(() => _player != null && _enemy != null);
        yield return new WaitUntil(() => MatchingBattleManager.Instance != null);

        var manager = MatchingBattleManager.Instance;
        PlayerAsBattleUnitBase.Stat = manager.PlayerStat;
        EnemyAsBattleUnitBase.Stat = manager.EnemyStat;
    }

    private IEnumerator _SetupBattleUnits()
    {
        var manager = MatchingBattleManager.Instance;
        if (manager != null)
        {
            PrefabManager.Instance.SpawnBattleUnit(manager.PlayerStat.Class, _playerInfo.Container, it => _player = it);
            PrefabManager.Instance.SpawnBattleUnit(manager.EnemyStat.Class, _enemyInfo.Container, it => _enemy = it);
        }
        else
        {
            PrefabManager.Instance.SpawnDefautUnit(UnitDefaultType.BattleHuman, _playerInfo.Container, it => _player = it);
            PrefabManager.Instance.SpawnDefautUnit(UnitDefaultType.BattleSkeleton, _enemyInfo.Container, it => _enemy = it);
        }
        yield return null;
    }

}
