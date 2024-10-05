using System;
using System.Collections;
using UnityEngine;

public abstract class DamageSkillBase : SkillBase
{

    protected bool _skillAnimationRunning;
    protected bool _unitAnimationRunning;
    protected bool _harvestingItemsRunning;

    protected override IEnumerator _Execute(BattleUnitBase target)
    {
        _unitAnimationRunning = true;
        _skillAnimationRunning = true;
        _harvestingItemsRunning = true;

        yield return new WaitUntil(() => gameObject.activeSelf);

        StartCoroutine(_RunHavestingItems());
        StartCoroutine(_RunUnitAnimation());
        StartCoroutine(_RunSkillAnimation(target));

        yield return new WaitUntil(() => !_unitAnimationRunning && !_skillAnimationRunning && !_harvestingItemsRunning);

        if (_battleUnitBase is BattlePlayerUnit playerUnit)
        {
            playerUnit.ExecutingSkill = true;
        }
        
        _onExecuted?.Invoke();
        _onDone?.Invoke();

        _HideSelf();
    }

    protected abstract IEnumerator _RunUnitAnimation();
    protected abstract IEnumerator _RunSkillAnimation(BattleUnitBase target);
    protected abstract IEnumerator _RunHavestingItems();
}
