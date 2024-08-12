using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class UnitAnimationHandler : MonoBehaviour
{
    //============== For inspector ==============

    [SerializeField] private string _idleUpAnimationName;
    [SerializeField] private string _idleDownAnimationName;
    [SerializeField] private string _idleLeftAnimationName;
    [SerializeField] private string _idleRightAnimationName;
    [SerializeField] private string _walkUpAnimationName;
    [SerializeField] private string _walkDownAnimationName;
    [SerializeField] private string _walkLeftAnimationName;
    [SerializeField] private string _walkRightAnimationName;
    [SerializeField] private string _animationDefaultName;

    [SerializeField] private string _attackRightAnimationName;
    [SerializeField] private float _attackRightAnimationDuration;

    [SerializeField] private string _attackLeftAnimationName;
    [SerializeField] private float _attackLeftAnimationDuration;

    [SerializeField] private string _hurtAnimationName;
    [SerializeField] private float _hurtAnimationDuration;

    [SerializeField] private string _deadAnimationName;
    [SerializeField] private float _deadAnimationDuration;

    private string _stateName;
    private Animator _animator;
    private float _lockedTill;

    public bool CurrentStateLocked => Time.time < _lockedTill;

    private void Start()
    {
        _animator = GetComponent<Animator>();
        _stateName = _animationDefaultName;
        RunAnimation(_stateName);
    }


    public void RunWalkAnimation(float x, float y) => RunAnimation(_GetState(x, y));
    
    private string _GetState(float x, float y) => _GetState(x, y, _stateName);

    private string _GetState(float x, float y, string currentStateName)
    {
        if(Time.time < _lockedTill)
        {
            return currentStateName;
        }

        var stateName = currentStateName;
        var absx = Mathf.Abs(x);
        var absy = Mathf.Abs(y);

        if (absx > absy)
        {
            if (x >= 0)
            {
                stateName = _walkRightAnimationName;
            }
            else
            {
                stateName = _walkLeftAnimationName;
            }
        }
        else if (absx < absy)
        {
            if (y >= 0)
            {
                stateName = _walkUpAnimationName;
            }
            else
            {
                stateName = _walkDownAnimationName;
            }
        }
        else
        {
            if (stateName == _walkDownAnimationName)
            {
                stateName = _idleDownAnimationName;
            }
            else if (stateName == _walkUpAnimationName)
            {
                stateName = _idleUpAnimationName;
            }
            else if (stateName == _walkLeftAnimationName)
            {
                stateName = _idleLeftAnimationName;
            }
            else if (stateName == _walkRightAnimationName)
            {
                stateName = _idleRightAnimationName;
            }
        }

        return stateName;
    }

    public void RunIdleAnimation(Vector2 left, Vector2 right) => RunAnimation(_GetIdleAnimation(left.x, right.x));

    public void RunIdleLeftAnimation() => RunAnimation(_idleLeftAnimationName);

    public void RunIdleRightAnimation() => RunAnimation(_idleRightAnimationName);

    private string _GetIdleAnimation(float l, float r) => l > r ? _idleLeftAnimationName : _idleRightAnimationName;

    public void RunWalkAnimation(Vector2 left, Vector2 right) => RunAnimation(_GetWalkAnimation(left.x, right.x));

    private string _GetWalkAnimation(float l, float r) => l > r ? _walkLeftAnimationName : _walkRightAnimationName;

    public void RunAttackAnimation(Vector2 left, Vector2 right) => RunAnimation(_GetAttackAnimation(left.x, right.x));

    private (string, float) _GetAttackAnimation(float l, float r) => l < r ? (_attackLeftAnimationName, _attackLeftAnimationDuration) : (_attackRightAnimationName, _attackRightAnimationDuration);

    public void RunHurtAnimation() => RunAnimation(_hurtAnimationName, _hurtAnimationDuration);

    public void RunDeadAnimation() => RunAnimation(_deadAnimationName, _deadAnimationDuration);

    public void RunAnimation(string name) => _animator.CrossFade(name, 0);

    public void RunAnimation((string, float) animationInfo) => RunAnimation(animationInfo.Item1, animationInfo.Item2);

    public void RunAnimation(string name, float duration)
    {
        _LockState(duration);
        _animator.CrossFade(name, duration);
    }

    private void _LockState(float duration) => _lockedTill = Time.time + duration;

}
