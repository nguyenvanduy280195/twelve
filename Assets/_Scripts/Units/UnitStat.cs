using System;
using Unity.Collections;
using UnityEngine;

[Serializable]
public class UnitStat
{
    public string Class;
    public int Level; // this can be lost

    public float Attack;

    public float HPMax;
    public float HPRegen;

    public float ManaMax;
    public float ManaRegen;

    public float StaminaMax;
    public float StaminaConsumeWeight;
    public float StaminaRegen;
}

[Serializable]
public class PlayerStat : UnitStat
{
    [Tooltip("Increase Attack\nAttack = 0.1f * Strength")]
    public int Strength;
    [Tooltip("Increase HP\nHPMax = 10 * Vitality\nHPRegen = 0.1f * Vitality")]
    public int Vitality;
    [Tooltip("Increase Stamina\nStaminaMax = 10 * Endurance\nStaminaRegen = 0.1f * Endurance")]
    public int Endurance;
    [Tooltip("Increase Mana\nManaMax = 10 * Intelligent\nManaRegen = 0.1f * Intelligent")]
    public int Intelligent;
    [Tooltip("Receive golds and exp")]
    public int Luck;
    public int nPoints;

    public string Name;
    public float HP;
    public float Mana;
    public float Stamina;

    public MyPosition Position;

    public int Gold;
    public int Exp;

    public PlayerStat Clone() => (PlayerStat)MemberwiseClone();
}

[Serializable]
public class EnemyStat : UnitStat
{
    public int BonusExp;
    public int BonusGold;
    public EnemyStat Clone() => (EnemyStat)MemberwiseClone();
}

[Serializable]
public class MyPosition
{
    public float x;
    public float y;
    public float z;

    public MyPosition() { }

    public MyPosition(Vector3 vector3)
    {
        x = vector3.x;
        y = vector3.y;
        z = vector3.z;
    }

    public Vector3 ToVector3() => new Vector3(x,y,z);
}