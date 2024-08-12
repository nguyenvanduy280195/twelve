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

    private void Start()
    {
        var manager = MatchingBattleManager.Instance;

        PlayerAsBattleUnitBase.Stat = manager.PlayerStat;
        EnemyAsBattleUnitBase.Stat = manager.EnemyStat;
    }
}
