using System;
using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public abstract class SkillBase : MonoBehaviour
{
    [SerializeField] private float _manaConsumed;
    [SerializeField] protected BattleUnitBase _battleUnitBase;
    [SerializeField] private UnitAnimationHandler _unitAnimationHandler;

    protected bool _skillAnimationRunning;
    private bool _unitAnimationRunning;

    private Coroutine _unitCoroutine;
    private Coroutine _skillCoroutine;

    public float ManaConsumed => _manaConsumed;

    public void Execute(BattleUnitBase target)
    {
        Debug.Log("SkillBase.Execute(...)");


        if (!_unitAnimationRunning && !_skillAnimationRunning)
        {
            _skillAnimationRunning = true;
            _unitAnimationRunning = true;

            if (_unitCoroutine != null)
            {
                StopCoroutine(_unitCoroutine);
            }
            if (_skillCoroutine != null)
            {
                StopCoroutine(_skillCoroutine);
            }

            _ShowSelf(); // gameobject must be active, if wanting starts a coroutine
            _skillCoroutine = StartCoroutine(_RunSkillAnimation(target));
            _unitCoroutine = StartCoroutine(_RunUnitAnimation());
        }
        else
        {
            throw new Exception("Please dont spam the skill button");
        }
    }


    protected void _ShowSelf() => gameObject.SetActive(true);

    protected void _HideSelf() => gameObject.SetActive(false);


    private IEnumerator _RunUnitAnimation()
    {
        _unitAnimationHandler.RunCastRightAnimation();
        yield return new WaitUntil(() => !_unitAnimationHandler.CurrentStateLocked);
        _unitAnimationHandler.RunIdleRightAnimation();
        _unitAnimationRunning = false;
    }

    protected abstract IEnumerator _RunSkillAnimation(BattleUnitBase target);
}
