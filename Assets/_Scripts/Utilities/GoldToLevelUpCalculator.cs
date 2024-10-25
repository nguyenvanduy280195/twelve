using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoldToLevelUpCalculator : PersistentSingleton<GoldToLevelUpCalculator>
{
    [SerializeField] private int _weightSkill1;
    [SerializeField] private int _weightSkill2;
    [SerializeField] private int _weightSkill3;


    public int GetNextLevel(SkillName type, int level)
    {
        var gold = -1;
        var iSkill = (int)type % 3;
        switch (iSkill)
        {
            case 0:
                gold = _weightSkill1 * level;
                break;
            case 1:
                gold = _weightSkill2 * level;
                break;
            case 2:
                gold = _weightSkill3 * level;
                break;
        }

        return gold;
    }
}
