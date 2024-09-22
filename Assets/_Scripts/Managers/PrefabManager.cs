using AYellowpaper.SerializedCollections;
using UnityEngine;

public enum UnitDefaultType
{
    Human,
    Skeleton,
    BattleHuman,
    BattleSkeleton,
}

public class PrefabManager : PersistentSingleton<PrefabManager>
{
    [SerializedDictionary("Battle Unit Class", "Prefab")]
    [SerializeField] private SerializedDictionary<UnitClass, GameObject> _battleUnitPrefabs;

    [SerializedDictionary("Unit Class", "Prefab")]
    [SerializeField] private SerializedDictionary<UnitDefaultType, GameObject> _defaultUnitPrefabs;

    [SerializedDictionary("Unit Class", "Prefab")]
    [SerializeField] private SerializedDictionary<UnitClass, GameObject> _unitPrefabs;

    public GameObject GetBattleUnitPrefab(UnitClass type) => _battleUnitPrefabs[type];
    public GameObject GetDefaultUnitPrefab(UnitDefaultType type) => _defaultUnitPrefabs[type];
    public GameObject GetUnitPrefab(UnitClass type) => _unitPrefabs[type];
}

