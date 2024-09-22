using UnityEngine;

public class SkillManaConsumptionCalculator : PersistentSingleton<SkillManaConsumptionCalculator>
{
    [SerializeField] private float _weightSkill1;
    [SerializeField] private float _weightSkill2;
    [SerializeField] private float _weightSkill3;


    public float GetManaConsumption(SkillName type, int level)
    {
        var manaConsumption = -1f;
        switch (type)
        {
            case SkillName.Meditate:
                manaConsumption = _weightSkill1 * level;
                break;
            case SkillName.Fireball:
                manaConsumption = _weightSkill2 * level;
                break;
            case SkillName.Meteor:
                manaConsumption = _weightSkill3 * level;
                break;
        }

        return manaConsumption;
    }
}
