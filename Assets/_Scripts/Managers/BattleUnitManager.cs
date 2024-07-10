using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleUnitManager : Singleton<BattleUnitManager>
{
    [SerializeField]
    private BattleUnitBase _player;
    public BattleUnitBase Player => _player;

    [SerializeField]
    private BattleUnitBase _enemy;
    public BattleUnitBase Enemy => _enemy;

    public void SpawnItems()
    {

    }


}
