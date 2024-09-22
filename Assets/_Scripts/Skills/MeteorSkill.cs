using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeteorSkill : DamageSkillBase
{
    [Header("Meteor's info")]
    [SerializeField] private int _number;
    [SerializeField] private float _speed;

    [Header("Meteor's Damage = atk x (damage_zero + weight_damage * skill_level)")]
    [SerializeField] private float _damageZero;
    [SerializeField] private float _weightDamage;


    private readonly float _myEpsilon = 0.0001f;

    #region Supports methods

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
        var damage = SkillDamageCalculator.Instance?.GetDamage(SkillName.Meteor, _battleUnitBase.Stat.Attack, _SkillData.Level) ?? -2f;
        Debug.Log($"[Meteor] - Damage = {damage}");
        target?.TakeHit(damage);
    }

    #endregion

    #region Inheriting via SkillBase
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

    protected override IEnumerator _RunHavestingItems()
    {
        var battleGameManager = BattleGameManager.Instance;
        if (battleGameManager != null)
        {
            var battleData = battleGameManager.MyData;
            var items = battleData.Items;

            var itemsWillBeHarvested = new List<GameObject>();
            for (int iNumber = 0; iNumber < _number; iNumber++)
            {
                var iRandomRow = Random.Range(1, battleData.NumberOfRow - 2);
                var iRandomCol = Random.Range(1, battleData.NumberOfColumn - 2);
                for (int iCol = iRandomCol - 1; iCol <= iRandomCol + 1; iCol++)
                {
                    for (int iRow = iRandomRow - 1; iRow <= iRandomRow + 1; iRow++)
                    {
                        Debug.Log($"Items[{iCol},{iRow}] = {items[iCol, iRow].tag}");
                        itemsWillBeHarvested.Add(items[iCol, iRow]);
                    }
                }
            }

            battleGameManager.HarvestItems(itemsWillBeHarvested);
        }
        _harvestingItemsRunning = false;
        yield return null;
    }

    #endregion
}
