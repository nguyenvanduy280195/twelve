using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ChoosingLevelUnitManager : SingletonPersistent<ChoosingLevelUnitManager>
{
    [SerializeField] private List<int> _listDeadEnemyID;

    private PlayerStat _playerStat;

    public PlayerStat PlayerStat => _playerStat;
    public GameObject Player => Finder.FindGameObjectByTag("Player"); // TODO Let find a better way
    public List<GameObject> Enemies { get; private set; }

    private void Start()
    {
        SceneListener.OnCreate += _ReloadEnemies;
        SceneListener.OnCreate += _FilterDeadEnemy;

        _listDeadEnemyID = new();
        _playerStat = SaveSystem.LoadPlayerStat();
    }

    private void OnDestroy()
    {
        SceneListener.OnCreate -= _ReloadEnemies;
        SceneListener.OnCreate -= _FilterDeadEnemy;
    }

    public void AddDeadEnemy(int id) => _listDeadEnemyID.Add(id);

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
