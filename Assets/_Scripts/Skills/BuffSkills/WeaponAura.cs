using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponAura : BuffSkillBase
{
    [Header("Buff")]
    [SerializeField] private int _nEffectTurns;
    [SerializeField] private float _weightBuff;

    protected override IEnumerator _RunUnitAnimation()
    {
        _unitAnimationHandler.RunCastRightAnimation();
        yield return new WaitUntil(() => !_unitAnimationHandler.CurrentStateLocked);
        _unitAnimationHandler.RunIdleRightAnimation();

        _unitAnimationRunning = false;
    }

    protected override IEnumerator _RunSkillAnimation(BattleUnitBase target)
    {
        _battleUnitBase.nEffectTurns = _nEffectTurns;
        _battleUnitBase.DamageBuff = _weightBuff;
        _skillAnimationRunning = false;
        yield return null;
    }

    protected override IEnumerator _CountTheEffectBuff()
    {
        yield return null;
    }

    protected override IEnumerator _ShutdownBuffEffect()
    {
        yield return null;
    }
}
