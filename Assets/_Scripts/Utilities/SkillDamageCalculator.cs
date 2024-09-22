using System;
using System.Collections;
using System.Collections.Generic;
using AYellowpaper.SerializedCollections;
using UnityEngine;

// TODO assign it
public class SkillDamageCalculator : PersistentSingleton<SkillDamageCalculator>
{
    [Serializable]
    public class SkillDamageInfo
    {
        public float Damage0;
        public float WeightDamage;
    }

    [SerializedDictionary("Skill Type", "Skill Damage")]
    [SerializeField] private SerializedDictionary<SkillName, SkillDamageInfo> _skillDamageInfo;

    public float GetDamage(SkillName type, float attackPower, int skillLevel)
    {
        switch (type)
        {
            case SkillName.Meditate:
                break;
            case SkillName.Fireball:
                return _GetFireballDamage(attackPower, skillLevel);
            case SkillName.Meteor:
                return _GetMeteorDamage(attackPower, skillLevel);
        }
        return 0f;
    }

    private float _GetFireballDamage(float attackPower, int skillLevel) => attackPower * (_skillDamageInfo[SkillName.Fireball].Damage0 + _skillDamageInfo[SkillName.Fireball].WeightDamage * skillLevel);

    private float _GetMeteorDamage(float attackPower, int skillLevel) => attackPower * (_skillDamageInfo[SkillName.Meteor].Damage0 + _skillDamageInfo[SkillName.Meteor].WeightDamage * skillLevel);
}
