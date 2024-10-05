using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : Singleton<EnemyManager>
{
    public IList<GameObject> Enemies
    {
        get
        {
            if (_enemies == null || _enemies.Count <= 0)
            {
                _Reload();
            }
            return _enemies;
        }
    }

    [Header("For Observing")]
    [SerializeField] private List<GameObject> _enemies;


    //private void OnEnable() => MazeSceneUI.OnCreate += _Reload;

    //private void OnDestroy() => MazeSceneUI.OnCreate -= _Reload;

    private void _Reload()
    {
        Debug.Log("EnemyManager._Reload()");
        _enemies = Finder.FindGameObjectsByTag("Enemy");
    }
}