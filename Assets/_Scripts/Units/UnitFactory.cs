using System;
using UnityEngine;
using Object = UnityEngine.Object;

public static class UnitFactory
{
    public static GameObject CreateUnit(UnitClass unitClass, Transform container)
    {
        GameObject unit = null;
        try
        {
            unit = Object.Instantiate(PrefabManager.Instance.GetUnitPrefab(unitClass), container);
        }
        catch (Exception e)
        {
            Debug.LogWarning($"Probaly, I haven't implemented {unitClass} class yet");
            Debug.LogWarning($"Messages: {e}");
        }

        return unit;
    }

    public static GameObject CreateBattleUnit(UnitClass unitClass, Transform container)
    {
        GameObject unit = null;
        try
        {
            unit = Object.Instantiate(PrefabManager.Instance.GetBattleUnitPrefab(unitClass), container);
        }
        catch (Exception e)
        {
            Debug.LogWarning($"Probaly, I haven't implemented {unitClass} class yet");
            Debug.LogWarning($"Messages: {e}");
        }

        return unit;
    }
}
