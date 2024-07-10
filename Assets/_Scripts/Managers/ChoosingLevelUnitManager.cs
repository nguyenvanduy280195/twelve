using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChoosingLevelUnitManager : Singleton<ChoosingLevelUnitManager>
{
    [SerializeField]
    private GameObject _player;

    public GameObject Player => _player;
}
