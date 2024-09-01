using UnityEngine;

public class BattleUnitManager : Singleton<BattleUnitManager>
{
    [SerializeField] private Transform _playerAttackPosition;
    public Vector3 PlayerAttackPosition => _playerAttackPosition.position;

    [SerializeField] private GameObject _player;
    public GameObject Player => _player;
    public BattlePlayerUnit PlayerAsBattlePlayerUnit => Player?.GetComponent<BattlePlayerUnit>();
    public BattleUnitBase PlayerAsBattleUnitBase => PlayerAsBattlePlayerUnit;


    [SerializeField] private Transform _enemyAttackPosition;
    public Vector3 EnemyAttackPosition => _enemyAttackPosition.position;

    [SerializeField] private GameObject _enemy;
    public GameObject Enemy => _enemy;
    public BattleEnemyUnit EnemyAsBattleEnemyUnit => Enemy.GetComponent<BattleEnemyUnit>();
    public BattleUnitBase EnemyAsBattleUnitBase => EnemyAsBattleEnemyUnit;

    [SerializeField] private ScriptablePlayerStat _playerStatDefault;
    [SerializeField] private ScriptableEnemyStat _enemyStatDefault;

    private void Start()
    {
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
        PlayerAsBattleUnitBase.Stat = _playerStatDefault?.PlayerStat.Clone();
        EnemyAsBattleUnitBase.Stat = _enemyStatDefault?.EnemyStat.Clone();
    }

    private void _SetupCasualMode()
    {
        var manager = MatchingBattleManager.Instance;
        PlayerAsBattleUnitBase.Stat = manager?.PlayerStat ?? _playerStatDefault?.PlayerStat.Clone();
        EnemyAsBattleUnitBase.Stat = manager?.EnemyStat ?? _enemyStatDefault?.EnemyStat.Clone();
    }
}
