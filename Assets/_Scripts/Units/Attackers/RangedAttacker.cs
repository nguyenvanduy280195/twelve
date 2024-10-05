using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RangedAttacker : AttackerBase
{
    [SerializeField] private BulletNormal _bulletPrefab;

    public override void Attack()
    {
        _myUnit.Actions[UnitState.Attack].Enqueue(() =>
        {
            _myUnit.State = UnitState.Attack;
            _animationHandler.RunAttackAnimation(_targetUnit.transform.position, transform.position);

            var bullet = Instantiate(_bulletPrefab, transform);
            bullet.SetDamage(_damage)
                .SetSpeed(_bulletSpeed)
                .SetTargetUnit(_targetUnit)
                .Fire();
        });
        _myUnit.Actions[UnitState.Attack].Enqueue(() =>
        {
            if (_myUnit.Actions[UnitState.Attack].Count <= 0)
            {
                _animationHandler.RunIdleAnimation(transform.position, _attackPosition);
                _myUnit.State = UnitState.Idle;
            }
        });

        // trigger the attack queue
        _myUnit.State = UnitState.Attack;
    }
}
