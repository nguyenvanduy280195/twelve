using System;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public abstract class SkillBase : MonoBehaviour
{
    [SerializeField] protected BattleUnitBase _battleUnitBase;
    [SerializeField] protected UnitAnimationHandler _unitAnimationHandler;

    private SkillData _skillData;
    protected Action _onDone;
    protected Action _onExecuted;
    protected BattleUnitBase _target;
    protected SkillData _SkillData
    {
        get
        {
            if (_skillData == null)
            {
                var unitStat = _battleUnitBase.Stat;
                if (unitStat is PlayerData playerData)
                {
                    var skillIndex = _battleUnitBase.GetSkillIndex(this);
                    _skillData = playerData.SkillData[skillIndex];
                }
            }
            return _skillData;
        }
    }

    public float ManaConsumed => SkillManaConsumptionCalculator.Instance.GetManaConsumption(_SkillData.Name, _SkillData.Level);

    public void Execute(BattleUnitBase target, Action onExecuted, Action onDone)
    {
        _onDone = onDone;
        _onExecuted = onExecuted;
        _target = target;

        // the gameobject must be active, if wanting to start a coroutine
        _ShowSelf();
        StartCoroutine(_Execute(target));
    }


    protected abstract IEnumerator _Execute(BattleUnitBase target);

    protected void _ShowSelf() => gameObject.SetActive(true);

    protected void _HideSelf() => gameObject.SetActive(false);

}
