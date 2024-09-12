using System;
using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public abstract class SkillBase : MonoBehaviour
{
    [SerializeField] protected float _manaConsumed;
    [SerializeField] protected BattleUnitBase _battleUnitBase;
    [SerializeField] protected UnitAnimationHandler _unitAnimationHandler;
    [SerializeField] protected int _level = 1;

    protected Action _onDone;
    protected Action _onExecuted;

    public float ManaConsumed => _manaConsumed * _level;

    public void Execute(BattleUnitBase target, Action onExecuted, Action onDone)
    {
        _onDone = onDone;
        _onExecuted = onExecuted;
        
        _ShowSelf(); // gameobject must be active, if wanting starts a coroutine
        StartCoroutine(_Execute(target));
    }


    protected abstract IEnumerator _Execute(BattleUnitBase target);

    protected void _ShowSelf() => gameObject.SetActive(true);

    protected void _HideSelf() => gameObject.SetActive(false);

}
