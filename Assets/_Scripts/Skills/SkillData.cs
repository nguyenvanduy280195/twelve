using System;
using UnityEngine;

[Serializable]
public class SkillData
{
    public int Level;
    public string IconPath;
    public SkillName Name;
    public string Describe;
}

[Serializable]
public enum SkillName
{
    Meditate = 0,
    Fireball,
    Meteor,
    PowerStrike,
    SpearSpin,
    MultiAttack,
}