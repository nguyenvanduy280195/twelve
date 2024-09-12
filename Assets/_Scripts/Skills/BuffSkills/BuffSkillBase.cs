using System;
using System.Collections;
using UnityEngine;

public abstract class BuffSkillBase : SkillBase
{
    protected bool _skillAnimationRunning;
    protected bool _unitAnimationRunning;

    protected override IEnumerator _Execute(BattleUnitBase target)
    {
        _unitAnimationRunning = true;
        _skillAnimationRunning = true;
        StartCoroutine(_RunUnitAnimation());
        StartCoroutine(_RunSkillAnimation(target));
        StartCoroutine(_CountTheEffectBuff());
        StartCoroutine(_ShutdownBuffEffect());

        yield return new WaitUntil(() => !_unitAnimationRunning && !_skillAnimationRunning);
        if (_battleUnitBase is BattlePlayerUnit playerUnit)
        {
            playerUnit.ExecutingSkill = true;
            BattleGameManager.Instance?.SkipTurn();
        }

        _onExecuted?.Invoke();
    }


    protected abstract IEnumerator _RunUnitAnimation();
    // TODO _RunSkillAnimation(...) is unused
    protected abstract IEnumerator _RunSkillAnimation(BattleUnitBase target);
    protected abstract IEnumerator _CountTheEffectBuff();
    protected abstract IEnumerator _ShutdownBuffEffect();
}
