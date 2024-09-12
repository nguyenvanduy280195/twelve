using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeditateSkill : BuffSkillBase
{
    [Header("Buff")]
    [SerializeField] private int _nEffectTurns;
    [SerializeField] private float _weightBuff;

    private int _nTurns = 0;

    protected override IEnumerator _RunUnitAnimation()
    {
        _unitAnimationHandler.RunCastRightAnimation();
        yield return new WaitUntil(() => !_unitAnimationHandler.CurrentStateLocked);

        _unitAnimationHandler.RunIdleRightAnimation();
        _unitAnimationRunning = false;
    }

    protected override IEnumerator _RunSkillAnimation(BattleUnitBase target)
    {
        _skillAnimationRunning = false;
        _nTurns = 0;
        yield return null;
    }

    protected override IEnumerator _CountTheEffectBuff()
    {
        var battleGameManager = BattleGameManager.Instance;
        if (battleGameManager == null)
        {
            yield break;
        }

        while (true)
        {
            yield return new WaitUntil(() => battleGameManager.IsChoosingUnitState);

            _nTurns++;

            var manaRestored = _fManaRestore();
            _battleUnitBase.RestoreMana(manaRestored);

            yield return new WaitUntil(() => !battleGameManager.IsChoosingUnitState);
        }
    }

    protected override IEnumerator _ShutdownBuffEffect()
    {
        yield return new WaitUntil(() => _nTurns >= _nEffectTurns);
        _onDone?.Invoke();
        _HideSelf();
    }

    private float _fManaRestore() => _weightBuff * _level * _manaConsumed;
}
