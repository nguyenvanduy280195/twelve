using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PrefabManager : Singleton<PrefabManager>
{
    [SerializeField] private GameObject _prefabSoldier;
    [SerializeField] private GameObject _prefabBattleSoldier;
    [SerializeField] private GameObject _prefabPriest;
    [SerializeField] private GameObject _prefabBattlePriest;
    [SerializeField] private GameObject _prefabSkeletonNoArmor;
    [SerializeField] private GameObject _prefabBattleSkeletonNoArmor;
    [SerializeField] private GameObject _prefabHumanDefault;
    [SerializeField] private GameObject _prefabBattleHumanDefault;
    [SerializeField] private GameObject _prefabSkeletonDefault;
    [SerializeField] private GameObject _prefabBattleSkeletonDefault;


    public GameObject PrefabSoldier => _prefabSoldier;
    public GameObject PrefabBattleSoldier => _prefabBattleSoldier;
    public GameObject PrefabPriest => _prefabPriest;
    public GameObject PrefabBattlePriest => _prefabBattlePriest;
    public GameObject PrefabSkeletonNoArmor => _prefabSkeletonNoArmor;
    public GameObject PrefabBattleSkeletonNoArmor => _prefabBattleSkeletonNoArmor;
    public GameObject PrefabHumanDefault => _prefabHumanDefault;
    public GameObject PrefabBattleHumanDefault => _prefabBattleHumanDefault;
    public GameObject PrefabSkeletonDefault => _prefabSkeletonDefault;
    public GameObject PrefabBattleSkeletonDefault => _prefabBattleSkeletonDefault;
}

