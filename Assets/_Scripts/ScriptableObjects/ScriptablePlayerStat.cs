using System;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "PlayerStat", menuName = "Stats/PlayerStat")]
public class ScriptablePlayerStat : ScriptableUnitStat
{
    public PlayerData PlayerStat => new PlayerData(_statMini, _skillData.ToList());

    [SerializeField] private PlayerStatMini _statMini;

    [SerializeField] private SkillData[] _skillData;

}

[Serializable]
public struct PlayerStatMini
{
    public string Name;
    public UnitClass Class;
    public int Level;
    public int Strength;
    public int Vitality;
    public int Endurance;
    public int Intelligent;
    public int Luck;

}