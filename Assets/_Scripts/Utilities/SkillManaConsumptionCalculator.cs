using UnityEngine;

public class SkillManaConsumptionCalculator : PersistentSingleton<SkillManaConsumptionCalculator>
{
    [SerializeField] private float _weightSkill1;
    [SerializeField] private float _weightSkill2;
    [SerializeField] private float _weightSkill3;


    public float GetManaConsumption(SkillName type, int level)
    {
        var manaConsumption = -1f;
        var iSkill = (int)type % 3;
        switch (iSkill)
        {
            case 0:
                manaConsumption = _weightSkill1 * level;
                break;
            case 1:
                manaConsumption = _weightSkill2 * level;
                break;
            case 2:
                manaConsumption = _weightSkill3 * level;
                break;
        }

        return manaConsumption;
    }
}
