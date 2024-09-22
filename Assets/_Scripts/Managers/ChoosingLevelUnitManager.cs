using System.Collections.Generic;
using UnityEngine;

public class ChoosingLevelUnitManager : PersistentSingleton<ChoosingLevelUnitManager>
{
    [SerializeField] private List<int> _listDeadEnemyID;

    [SerializeField] private Transform _playerContainer;

    /// <summary>
    /// Please dont use me, Just use my getter
    /// </summary>
    private PlayerData _playerStat;

    /// <summary>
    /// Please dont use me, Just use my getter
    /// </summary>
    private GameObject _player;

    #region Public methods
    public PlayerData PlayerData
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

    public GameObject Player
    {
        get
        {
            if (_player == null)
            {
                _player = UnitFactory.CreateUnit(PlayerData.Class, _playerContainer);
            }

            if (_player == null)
            {
                _player = Instantiate(PrefabManager.Instance.GetDefaultUnitPrefab(UnitDefaultType.Human), _playerContainer);
            }
            
            return _player;
        }
    }

    public List<GameObject> Enemies { get; private set; }
    public void AddDeadEnemy(int id) => _listDeadEnemyID?.Add(id);
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

    private void _ReloadEnemies()
    {
        Enemies = Finder.FindGameObjectsByTag("Enemy"); // TODO Let find a better way
    }

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
