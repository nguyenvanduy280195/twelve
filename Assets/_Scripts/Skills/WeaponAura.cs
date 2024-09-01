using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponAura : SkillBase
{
    [Header("Buff")]
    [SerializeField] private int _nEffectTurns;
    [SerializeField] private float _weightBuff;

    protected override IEnumerator _RunSkillAnimation(BattleUnitBase target)
    {
        _battleUnitBase.nEffectTurns = _nEffectTurns;
        _battleUnitBase.DamageBuff = _weightBuff;
        _skillAnimationRunning = false;
        yield return null;
    }
}
