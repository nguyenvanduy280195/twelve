using System;
using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public abstract class SkillBase : MonoBehaviour
{
    [SerializeField] private float _manaConsumed;
    [SerializeField] protected BattleUnitBase _battleUnitBase;
    [SerializeField] protected UnitAnimationHandler _unitAnimationHandler;

    protected bool _skillAnimationRunning;
    protected bool _unitAnimationRunning;
    protected bool _harvestingItemsRunning;

    public float ManaConsumed => _manaConsumed;

    public void Execute(BattleUnitBase target, Action OnDone)
    {
        _ShowSelf(); // gameobject must be active, if wanting starts a coroutine
        StartCoroutine(_Execute(target, OnDone));

    }

    private IEnumerator _Execute(BattleUnitBase target, Action OnDone)
    {
        _unitAnimationRunning = true;
        _skillAnimationRunning = true;
        _harvestingItemsRunning = true;

        StartCoroutine(_RunHavestingItems());
        StartCoroutine(_RunUnitAnimation());
        StartCoroutine(_RunSkillAnimation(target));

        yield return new WaitUntil(() => !_unitAnimationRunning && !_skillAnimationRunning && !_harvestingItemsRunning);
        OnDone?.Invoke();

        _HideSelf();
    }

    private void _ShowSelf() => gameObject.SetActive(true);

    private void _HideSelf() => gameObject.SetActive(false);

    protected abstract IEnumerator _RunHavestingItems();
    protected abstract IEnumerator _RunUnitAnimation();
    protected abstract IEnumerator _RunSkillAnimation(BattleUnitBase target);
}
