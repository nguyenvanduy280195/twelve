using System;
using UnityEngine;

[DisallowMultipleComponent]
[RequireComponent(typeof(Animator))]
public class UnitAnimationHandler : MonoBehaviour
{

    [Serializable]
    public class AnimationInfo
    {
        public string Name;
        public float Duration;
    }

    //============== For inspector ==============

    [SerializeField] private string _animationDefaultName;
    [SerializeField] private AnimationInfo _attackLeftAnimation;
    [SerializeField] private AnimationInfo _attackRightAnimation;
    [SerializeField] private AnimationInfo _castLeftAnimation;
    [SerializeField] private AnimationInfo _castRightAnimation;
    [SerializeField] private AnimationInfo _hurtAnimation;
    [SerializeField] private AnimationInfo _aliveAnimation;
    [SerializeField] private AnimationInfo _deadAnimation;

    [Header("Idle")]
    [SerializeField] private string _idleUpAnimationName;
    [SerializeField] private string _idleDownAnimationName;
    [SerializeField] private string _idleLeftAnimationName;
    [SerializeField] private string _idleRightAnimationName;
    [Header("Walk")]
    [SerializeField] private string _walkUpAnimationName;
    [SerializeField] private string _walkDownAnimationName;
    [SerializeField] private string _walkLeftAnimationName;
    [SerializeField] private string _walkRightAnimationName;

    private string _stateName;
    private Animator _animator;
    private float _lockedTill;

    public bool CurrentStateLocked => Time.time < _lockedTill;
    public void RunWalkAnimation(float x, float y) => _RunAnimation(_GetState(x, y));
    public void RunIdleAnimation(Vector2 left, Vector2 right) => _RunAnimation(_GetIdleAnimation(left.x, right.x));
    public void RunIdleLeftAnimation() => _RunAnimation(_idleLeftAnimationName);
    public void RunIdleRightAnimation() => _RunAnimation(_idleRightAnimationName);
    public void RunWalkAnimation(Vector2 left, Vector2 right) => _RunAnimation(_GetWalkAnimation(left.x, right.x));
    public void RunAttackAnimation(Vector2 left, Vector2 right) => _RunAnimation(_GetAttackAnimation(left.x, right.x));
    public void RunHurtAnimation() => _RunAnimation(_hurtAnimation.Name, _hurtAnimation.Duration);
    public void RunAliveAnimation() => _RunAnimation(_aliveAnimation.Name, _aliveAnimation.Duration);
    public void RunDeadAnimation() => _RunAnimation(_deadAnimation.Name, _deadAnimation.Duration);
    public void RunCastLeftAnimation() => _RunAnimation(_castLeftAnimation.Name, _castLeftAnimation.Duration);
    public void RunCastRightAnimation() => _RunAnimation(_castRightAnimation.Name, _castRightAnimation.Duration);

    #region Supporting methods

    private void Start()
    {
        _animator = GetComponent<Animator>();
        _stateName = _animationDefaultName;
        _RunAnimation(_stateName);
    }

    private string _GetState(float x, float y)
    {
        _stateName = _GetState(x, y, _stateName);
        return _stateName;
    }

    private string _GetState(float x, float y, string currentStateName)
    {
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

    private string _GetIdleAnimation(float l, float r) => l > r ? _idleLeftAnimationName : _idleRightAnimationName;

    private string _GetWalkAnimation(float l, float r) => l > r ? _walkLeftAnimationName : _walkRightAnimationName;

    private (string, float) _GetAttackAnimation(float l, float r) => l < r ? (_attackLeftAnimation.Name, _attackLeftAnimation.Duration) : (_attackRightAnimation.Name, _attackRightAnimation.Duration);

    private void _RunAnimation(string name) => _animator?.CrossFade(name, 0);

    private void _RunAnimation((string, float) animationInfo) => _RunAnimation(animationInfo.Item1, animationInfo.Item2);

    private void _RunAnimation(string name, float duration)
    {
        _LockState(duration);
        _animator.CrossFade(name, duration);
    }

    private void _LockState(float duration) => _lockedTill = Time.time + duration;

    #endregion
}
