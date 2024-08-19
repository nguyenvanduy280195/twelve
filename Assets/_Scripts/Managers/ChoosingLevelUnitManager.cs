using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ChoosingLevelUnitManager : SingletonPersistent<ChoosingLevelUnitManager>
{
    [SerializeField]
    private ScriptablePlayerStat _scriptablePlayerStat;

    private PlayerStat _playerStat;
    public PlayerStat PlayerStat => _playerStat ??= SaveSystem.LoadPlayerStat() ?? _scriptablePlayerStat.PlayerStat.Clone();

    public GameObject Player => Finder.FindGameObjectByTag("Player");

    public List<GameObject> Enemies { get; private set; }

    [SerializeField]
    private List<int> _listDeadEnemyID;

    private void Start()
    {
        SceneListener.OnCreate += _ReloadEnemies;
        SceneListener.OnCreate += _FilterDeadEnemy;

        _listDeadEnemyID = new();
    }

    private void OnDestroy()
    {
        SceneListener.OnCreate -= _ReloadEnemies;
        SceneListener.OnCreate -= _FilterDeadEnemy;
    }

    public void AddDeadEnemy(int id) => _listDeadEnemyID.Add(id);

    #region Support methods

    private void _ReloadEnemies() => Enemies = Finder.FindGameObjectsByTag("Enemy");

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
