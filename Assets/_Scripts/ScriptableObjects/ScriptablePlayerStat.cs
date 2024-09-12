using System;
using UnityEngine;

[CreateAssetMenu(fileName = "PlayerStat", menuName = "Stats/PlayerStat")]
public class ScriptablePlayerStat : ScriptableUnitStat
{
    public PlayerStat PlayerStat => new PlayerStat(_statMini);

    [SerializeField] private PlayerStatMini _statMini;

}

[Serializable]
public struct PlayerStatMini
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