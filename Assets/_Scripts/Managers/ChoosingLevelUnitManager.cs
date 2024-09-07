using System.Collections.Generic;
using UnityEngine;

public class ChoosingLevelUnitManager : SingletonPersistent<ChoosingLevelUnitManager>
{
    private List<int> _listDeadEnemyID;

    private PlayerStat _playerStat;

    #region Public methods
    public PlayerStat PlayerStat
    {
        get
        {
            if (_playerStat == null)
            {
                _playerStat = SaveSystem.LoadPlayerStat();
            }
            return _playerStat;
        }
    }
    public GameObject Player => Finder.FindGameObjectByTag("Player"); // TODO Let find a better way
    public List<GameObject> Enemies { get; private set; }
    public void AddDeadEnemy(int id) => _listDeadEnemyID.Add(id);
    #endregion

    #region Unity methods

    private void Start()
    {
        MySceneBase.OnCreate += _ReloadEnemies;
        MySceneBase.OnCreate += _FilterDeadEnemy;

        _listDeadEnemyID = new();
    }

    private void OnDestroy()
    {
        MySceneBase.OnCreate -= _ReloadEnemies;
        MySceneBase.OnCreate -= _FilterDeadEnemy;
    }

    #endregion

    #region Support methods

    private void _ReloadEnemies() => Enemies = Finder.FindGameObjectsByTag("Enemy"); // TODO Let find a better way

    private void _FilterDeadEnemy()
    {
        var enemies = Enemies;
        foreach (var deadEnemyID in _listDeadEnemyID)
        {
            foreach (var enemy in enemies)
            {
                var enemyData = enemy.GetComponent<EnemyData>();
                if (enemyData.ID == deadEnemyID)
                {
                    enemy.SetActive(false);
                }
            }
        }
    }

    #endregion
}
