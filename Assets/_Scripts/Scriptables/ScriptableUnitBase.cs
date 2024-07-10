using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ScriptableUnitBase : ScriptableObject
{
    public Faction Faction;

    [SerializeField]
    private Stats _stats;
    public Stats BaseStat => _stats;

    public GameObject Prefab;

    public string Description;
    public Sprite MenuSprite;
}

[SerializeField]
public enum Faction
{
    Hero,
    Enemy
}

[SerializeField]
public struct Stats
{
    float HP;
    float Attack;
}