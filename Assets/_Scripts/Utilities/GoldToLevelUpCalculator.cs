using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//TODO please implement GoldToLevelUpCalculator
public class GoldToLevelUpCalculator : PersistentSingleton<GoldToLevelUpCalculator>
{
    [SerializeField] private int _weightSkill1;
    [SerializeField] private int _weightSkill2;
    [SerializeField] private int _weightSkill3;


    public int GetNextLevel(SkillName type, int level)
    {
        var gold = -1;
        switch (type)
        {
            case SkillName.Meditate:
                gold = _weightSkill1 * level;
                break;
            case SkillName.Fireball:
                gold = _weightSkill2 * level;
                break;
            case SkillName.Meteor:
                gold = _weightSkill3 * level;
                break;
        }

        return gold;
    }
}
