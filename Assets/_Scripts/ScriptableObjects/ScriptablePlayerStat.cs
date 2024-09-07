using System;
using UnityEngine;

[CreateAssetMenu(fileName = "PlayerStat", menuName = "Stats/PlayerStat")]
public class ScriptablePlayerStat : ScriptableUnitStat
{
    public PlayerStat PlayerStat => _GeneratePlayerStat();

    [SerializeField] private PlayerStatMini _statMini;

    private PlayerStat _GeneratePlayerStat() => new()
    {
        Name = _statMini.Name,
        Class = _statMini.Class,
        Level = _statMini.Level,
        Attack = 0.5f * _statMini.Strength,
        HPMax = 10f * _statMini.Vitality,
        HP = 10f * _statMini.Vitality,
        HPRegen = 0.1f * _statMini.Vitality,
        ManaMax = 10f * _statMini.Intelligent,
        Mana = 0f,
        ManaRegen = 0.1f * _statMini.Intelligent,
        StaminaMax = 10f * _statMini.Endurance,
        StaminaRegen = 0.1f * _statMini.Endurance,
        StaminaConsumeWeight = 0.5f * _statMini.Level,
        Stamina = 10f * _statMini.Endurance,
        Strength = _statMini.Strength,
        Vitality = _statMini.Vitality,
        Endurance = _statMini.Endurance,
        Intelligent = _statMini.Intelligent,
        Luck = _statMini.Luck,
    };
}

[Serializable]
struct PlayerStatMini
{
    public string Name;
    public string Class;
    public int Level;
    public int Strength;
    public int Vitality;
    public int Endurance;
    public int Intelligent;
    public int Luck;

}