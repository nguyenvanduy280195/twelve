using System;
using AYellowpaper.SerializedCollections;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

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

    [SerializedDictionary("Battle Unit Class", "Prefab")]
    [SerializeField] private SerializedDictionary<UnitClass, AssetReference> _battleUnitPrefabs1;

    [SerializedDictionary("Unit Class", "Prefab")]
    [SerializeField] private SerializedDictionary<UnitDefaultType, GameObject> _defaultUnitPrefabs;

    [SerializedDictionary("Unit Class", "Prefab")]
    [SerializeField] private SerializedDictionary<UnitDefaultType, AssetReference> _defaultUnitPrefabs1;

    [SerializedDictionary("Unit Class", "Prefab")]
    [SerializeField] private SerializedDictionary<UnitClass, GameObject> _unitPrefabs;

    [SerializedDictionary("Unit Class", "Prefab")]
    [SerializeField] private SerializedDictionary<UnitClass, AssetReference> _unitPrefabs1;

    public void SpawnBattleUnit(UnitClass unitClass, Transform parent, Action<GameObject> onSpawned) => _Spawn(_battleUnitPrefabs1, unitClass, parent, onSpawned);
    
    public void SpawnDefautUnit(UnitDefaultType unitClass, Transform parent, Action<GameObject> onSpawned) => _Spawn(_defaultUnitPrefabs1, unitClass, parent, onSpawned);
    
    public void SpawnUnit(UnitClass unitClass, Transform parent, Action<GameObject> onSpawned) => _Spawn(_unitPrefabs1, unitClass, parent, onSpawned);

    private void _Spawn<T>(SerializedDictionary<T, AssetReference>dict, T type, Transform parent, Action<GameObject> onSpawned)
    {
        var handle = Addressables.LoadAssetAsync<GameObject>(dict[type]);
        handle.Completed += (AsyncOperationHandle<GameObject> task) =>
        {
            var unit = Instantiate(task.Result, parent);
            onSpawned?.Invoke(unit);
        };
    }
}

