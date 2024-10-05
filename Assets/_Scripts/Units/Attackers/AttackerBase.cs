using System;
using System.Collections.Generic;
using UnityEngine;

public abstract class AttackerBase : MonoBehaviour, IAttacker
{
    protected BattleUnitBase _myUnit;
    protected UnitAnimationHandler _animationHandler;
    protected BattleUnitBase _targetUnit;
    protected float _damage;
    protected float _moveSpeed;
    protected float _bulletSpeed;
    protected Vector3 _unitPosition;
    protected Vector3 _attackPosition;

    private void Start()
    {
        _unitPosition = transform.position;
        _myUnit = GetComponent<BattleUnitBase>();
        _animationHandler = GetComponent<UnitAnimationHandler>();
    }

    public IAttacker SetDamage(float damage)
    {
        _damage = damage;
        return this;
    }

    public IAttacker SetTargetUnit(BattleUnitBase unit)
    {
        _targetUnit = unit;
        return this;
    }

    public IAttacker SetMoveSpeed(float speed)
    {
        _moveSpeed = speed;
        return this;
    }

    public IAttacker SetBulletSpeed(float speed)
    {
        _bulletSpeed = speed;
        return this;
    }

    public IAttacker SetAttackPosition(Vector3 position)
    {
        _attackPosition = position;
        return this;
    }


    public virtual void Attack() { }

}