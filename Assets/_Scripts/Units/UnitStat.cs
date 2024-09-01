using System;

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
    public int Strength;
    public int Vitality;
    public int Endurance;
    public int Intelligent;
    public int Luck;
    public int nPoints;

    public string Name;
    public float HP;
    public float Mana;
    public float Stamina;

    public float PositionX;
    public float PositionY;

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
