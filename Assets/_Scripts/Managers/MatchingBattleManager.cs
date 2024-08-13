using UnityEngine;
using UnityEngine.SceneManagement;

public class MatchingBattleManager : SingletonPersistent<MatchingBattleManager>
{
    private static readonly string _inBattleSceneName = "InBattle";
    private static readonly string _outBattleSceneName = "Maze";

    [SerializeField]
    private ScriptableEnemyStat DefaultEnemyStat;

    public PlayerStat PlayerStat => ChoosingLevelUnitManager.Instance.PlayerStat;

    public EnemyStat EnemyStat { get; private set; }

    public Vector3 PlayerPositionBeforeBattle { get; private set; }
    public Vector3 EnemyPositionBeforeBattle { get; private set; }

    public void BeginBattle(GameObject enemy)
    {
        PlayerPositionBeforeBattle = ChoosingLevelUnitManager.Instance.Player.transform.position;

        EnemyPositionBeforeBattle = enemy.transform.position;
        EnemyStat = _GetEnemyStatFromEnemy(enemy) ?? DefaultEnemyStat.EnemyStat;

        SceneManager.LoadScene(_inBattleSceneName);
    }

    public void EndBattle()
    {
        SceneManager.LoadScene(_outBattleSceneName);
    }

    private EnemyStat _GetEnemyStatFromEnemy(GameObject enemy)
    {
        var enemyData = enemy.GetComponent<EnemyData>();
        if (enemyData?.ScriptableEnemyStat.EnemyStat is EnemyStat enemyStat)
        {
            return enemyStat.Clone();
        }
        return null;
    }
}
