using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireballSkill : SkillBase
{
    [Header("Fireball's info")]
    [SerializeField] private float _weightDamage;
    [SerializeField] private float _speed;

    private readonly float _myEpsilon = 0.0001f;

    protected override IEnumerator _RunUnitAnimation()
    {
        _unitAnimationHandler.RunCastRightAnimation();
        yield return new WaitUntil(() => !_unitAnimationHandler.CurrentStateLocked);
        _unitAnimationHandler.RunIdleRightAnimation();

        _unitAnimationRunning = false;
    }

    protected override IEnumerator _RunSkillAnimation(BattleUnitBase target)
    {
        _ResetPosition();

        yield return StartCoroutine(_MoveToTargetByFrame(target));
        
        _LetTargetTakeDamage(target);
        _skillAnimationRunning = false;
    }

    private bool _IsContactingTarget(Vector3 targetPosition)
    {
        var delta = transform.position - targetPosition;
        return delta.magnitude < _myEpsilon;
    }

    private IEnumerator _MoveToTargetByFrame(BattleUnitBase target)
    {
        while (!_IsContactingTarget(target.transform.position))
        {
            var targetPosition = target.transform.position;
            transform.position = Vector2.MoveTowards(transform.position, targetPosition, _speed * Time.deltaTime);
            yield return null;
        }
    }

    private void _ResetPosition() => transform.localPosition = Vector3.zero;

    private void _LetTargetTakeDamage(BattleUnitBase target)
    {
        var damage = _battleUnitBase.Stat.Attack * _weightDamage;
        Debug.Log($"[FireballSkill] - Damage = {damage}");
        target?.TakeHit(damage);
    }

}