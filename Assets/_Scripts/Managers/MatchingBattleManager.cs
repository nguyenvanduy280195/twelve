using UnityEngine;

public class MatchingBattleManager : SingletonPersistent<MatchingBattleManager>
{
    [SerializeField]
    private ScriptableEnemyStat DefaultEnemyStat;

    public PlayerStat PlayerStat => ChoosingLevelUnitManager.Instance.PlayerStat;
    public EnemyStat EnemyStat { get; private set; }

    public Vector3 PlayerPositionBeforeBattle { get; private set; }
    public Vector3 EnemyPositionBeforeBattle { get; private set; }

    private int _enemyID;

    public void BeginBattle(GameObject enemy)
    {
        PlayerPositionBeforeBattle = ChoosingLevelUnitManager.Instance.Player.transform.position;
        
        PlayerStat.PositionX = PlayerPositionBeforeBattle.x;
        PlayerStat.PositionY = PlayerPositionBeforeBattle.y;
        SaveSystem.SavePlayerStat(PlayerStat);

        
        EnemyPositionBeforeBattle = enemy.transform.position;

        
        EnemyStat = _GetEnemyStatFromEnemy(enemy) ?? DefaultEnemyStat.EnemyStat;
        
        _enemyID = enemy.GetComponent<EnemyData>().ID;
        
        GameManager.Instance?.SetPausing(true);
        
        MySceneManager.Instance?.LoadInBattleScene();
    }

    public void EndBattle()
    {
        MySceneManager.Instance?.LoadMazeScene();
        _HideDeadEnemy();
    }

    private void _HideDeadEnemy() => ChoosingLevelUnitManager.Instance.AddDeadEnemy(_enemyID);

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
