using System.Collections.Generic;
using UnityEngine;

public class ChoosingLevelUnitManager : SingletonPersistent<ChoosingLevelUnitManager>
{
    [SerializeField]
    private GameObject _player;
    public GameObject Player => _player;

    [SerializeField]
    private GameObject[] _enemies;
    public GameObject[] Enemies => _enemies;

    [SerializeField]
    private ScriptablePlayerStat _scriptablePlayerStat;

    private PlayerStat _playerStat;
    public PlayerStat PlayerStat => _playerStat ??= SaveSystem.LoadPlayerStat() ?? _scriptablePlayerStat.PlayerStat.Clone();

    //private List<EnemyStat> _enemiesStat;
    //public List<EnemyStat> EnemiesStat => _enemiesStat;

    private void Start()
    {
        Debug.Log($"ChoosingLevelUnitManager.Start");

        _player.transform.position = MatchingBattleManager.Instance.PlayerPositionBeforeBattle;

        //_enemiesStat ??= _GetEnemysStatFromEnemyGameObject();
    }

    #region Support methods

    private List<EnemyStat> _GetEnemysStatFromEnemyGameObject()
    {
        List<EnemyStat> enemiesStat = new List<EnemyStat>();
        foreach (var enemy in _enemies)
        {
            var battleEnemyUnit = enemy.GetComponent<BattleEnemyUnit>();
            var enemyStat = battleEnemyUnit.Stat as EnemyStat;
            enemiesStat.Add(enemyStat);
        }

        return enemiesStat;
    }

    #endregion
}
