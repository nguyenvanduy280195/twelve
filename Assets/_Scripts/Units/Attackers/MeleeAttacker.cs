using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class MeleeAttacker : AttackerBase
{
    public override void Attack()
    {
        if (_myUnit.Actions[UnitState.Attack].Count <= 0)
        {
            _myUnit.Actions[UnitState.MoveToTargetPosition].Enqueue(() =>
            {
                _myUnit.State = UnitState.Attack;
            });
            _myUnit.Actions[UnitState.MoveToStartPosition].Enqueue(() =>
            {
                _myUnit.State = UnitState.Idle;
                _animationHandler.RunIdleAnimation(transform.position, _attackPosition);
            });

            // triggering the queue
            _myUnit.State = UnitState.MoveToTargetPosition;
            _animationHandler.RunWalkAnimation(transform.position, _targetUnit?.transform.position ?? Vector3.zero);
        }

        _myUnit.Actions[UnitState.Attack].Enqueue(() =>
        {
            _animationHandler.RunAttackAnimation(transform.position, _unitPosition);
            _targetUnit?.TakeHit(_damage);

            if (_myUnit.Actions[UnitState.Attack].Count <= 0)
            {
                _myUnit.State = UnitState.MoveToStartPosition;
                _animationHandler.RunWalkAnimation(transform.position, _unitPosition);
            }
        });
        _myUnit.Actions[UnitState.Attack].Enqueue(() =>
        {
            if (_myUnit.Actions[UnitState.Attack].Count <= 0)
            {
                _myUnit.State = UnitState.MoveToStartPosition;
                _animationHandler.RunWalkAnimation(transform.position, _unitPosition);
            }
        });
    }
}
