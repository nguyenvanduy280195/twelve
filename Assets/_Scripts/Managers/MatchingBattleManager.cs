using UnityEngine;

public class MatchingBattleManager : PersistentSingleton<MatchingBattleManager>
{
    [SerializeField]
    private ScriptableEnemyStat DefaultEnemyStat;

    public PlayerStat PlayerStat => ChoosingLevelUnitManager.Instance.PlayerStat;
    public EnemyStat EnemyStat { get; private set; }


    public Vector3 EnemyPositionBeforeBattle { get; private set; }

    private int _enemyID;

    public void BeginBattle(GameObject enemy)
    {
        _SavePlayerPosition();

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

    #region Support methods

    private void _SavePlayerPosition()
    {
        PlayerStat.Position = new MyPosition(ChoosingLevelUnitManager.Instance.Player.transform.position);
        SaveSystem.SavePlayerStat(PlayerStat);
    }

    private void _HideDeadEnemy() => ChoosingLevelUnitManager.Instance?.AddDeadEnemy(_enemyID);

    private EnemyStat _GetEnemyStatFromEnemy(GameObject enemy)
    {
        var enemyData = enemy.GetComponent<EnemyData>();
        if (enemyData?.ScriptableEnemyStat.EnemyStat is EnemyStat enemyStat)
        {
            return enemyStat.Clone();
        }
        return null;
    }

    #endregion

}
