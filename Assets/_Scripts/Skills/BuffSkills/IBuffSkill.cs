public enum BuffType
{
    Attack,
    HP,
    Mana,
    Stamina
}

public interface IBuffSkill
{
    public bool StillBuffEffect { get; }
    public float BuffValue { get; }
    public BuffType Type { get; }
}
