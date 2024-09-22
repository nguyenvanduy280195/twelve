using System;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;

[Serializable]
public enum UnitClass
{
    Soldier,
    FireMage,
    Skeleton,
}

[Serializable]
public class UnitStat
{
    public UnitClass Class;
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
public class PlayerData : UnitStat
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
    public List<SkillData> SkillData;

    public PlayerData Clone() => (PlayerData)MemberwiseClone();

    public PlayerData() { SkillData = new(); }

    public PlayerData(PlayerStatMini statMini, List<SkillData> skillData)
    {
        Name = statMini.Name;
        Class = statMini.Class;
        Level = statMini.Level;

        Attack = _GenerateAttack(statMini.Strength);

        HPMax = _GenerateHPMax(statMini.Vitality);
        HP = HPMax;
        HPRegen = _GenerateHPRegen(statMini.Vitality);

        ManaMax = _GenerateManaMax(statMini.Intelligent);
        Mana = 0f;
        ManaRegen = _GenerateManaRegen(statMini.Intelligent);

        StaminaMax = _GenerateStaminaMax(statMini.Endurance);
        Stamina = StaminaMax;
        StaminaRegen = _GenerateStaminaRegen(statMini.Endurance);
        StaminaConsumeWeight = _GenerateStaminaConsumeWeight(statMini.Level);

        Strength = statMini.Strength;
        Vitality = statMini.Vitality;
        Endurance = statMini.Endurance;
        Intelligent = statMini.Intelligent;
        Luck = statMini.Luck;

        SkillData = skillData;
    }

    public PlayerData(PlayerData other)
    {
        Name = other.Name;
        Class = other.Class;
        Level = other.Level;

        Attack = _GenerateAttack(other.Strength);

        HPMax = _GenerateHPMax(other.Vitality);
        HP = HPMax;
        HPRegen = _GenerateHPRegen(other.Vitality);

        ManaMax = _GenerateManaMax(other.Intelligent);
        Mana = 0f;
        ManaRegen = _GenerateManaRegen(other.Intelligent);

        StaminaMax = _GenerateStaminaMax(other.Endurance);
        Stamina = StaminaMax;
        StaminaRegen = _GenerateStaminaRegen(other.Endurance);
        StaminaConsumeWeight = _GenerateStaminaConsumeWeight(other.Level);

        Strength = other.Strength;
        Vitality = other.Vitality;
        Endurance = other.Endurance;
        Intelligent = other.Intelligent;
        Luck = other.Luck;

        SkillData = other.SkillData;
    }

    private float _GenerateAttack(int strength) => 0.5f * strength;
    private float _GenerateHPMax(int vitality) => 10f * vitality;
    private float _GenerateHPRegen(int vitality) => 0.1f * vitality;
    private float _GenerateManaMax(int intelligent) => 10f * intelligent;
    private float _GenerateManaRegen(int intelligent) => 0.1f * intelligent;
    private float _GenerateStaminaMax(int endurance) => 10f * endurance;
    private float _GenerateStaminaRegen(int endurance) => 0.1f * endurance;
    private float _GenerateStaminaConsumeWeight(int level) => 0.2f * level;

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

    public Vector3 ToVector3() => new Vector3(x, y, z);
}