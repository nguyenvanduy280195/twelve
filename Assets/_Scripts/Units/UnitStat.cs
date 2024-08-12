using System;
using UnityEngine;

[Serializable]
public class UnitStat
{
    public string Name;
    public string Class;
    public int Level;
    public float Attack;
    public float MaxHP;
    public float HP;
    public float RegenHP;
    public float MaxMana;
    public float Mana;
    public float RegenMana;
    public float MaxStamina;
    public float Stamina;
    public float RegenStamina;
}

[Serializable]
public class PlayerStat : UnitStat
{
    public int Gold;
    public int Exp;
    public int nPoints;
}

[Serializable]
public class EnemyStat : UnitStat
{
    public int BonusExp;
}
