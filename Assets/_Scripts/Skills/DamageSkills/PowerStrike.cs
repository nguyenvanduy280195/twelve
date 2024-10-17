using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerStrike : DamageSkillBase
{
    [SerializeField] private float _speed = 1f;
    [SerializeField] private float _distanceToAttackTarget = 100f;

    protected override IEnumerator _RunHavestingItems()
    {
        var battleGameManager = BattleGameManager.Instance;
        if (battleGameManager != null)
        {
            var battleData = battleGameManager.MyData;
            var items = battleData.Items;
            var iRandomRow = Random.Range(0, battleData.NumberOfRow - 1);

            var itemsWillBeHarvested = new List<GameObject>();
            for (int iCol = 0; iCol < battleData.NumberOfColumn; iCol++)
            {
                Debug.Log($"Items[{iCol},{iRandomRow}] = {items[iCol, iRandomRow].tag}");
                itemsWillBeHarvested.Add(items[iCol, iRandomRow]);
            }
            battleGameManager.HarvestItems(itemsWillBeHarvested);
        }
        _harvestingItemsRunning = false;
        yield return null;
    }

    protected override IEnumerator _RunSkillAnimation(BattleUnitBase target)
    {
        yield return new WaitUntil(() => _unitAnimationRunning);

        // run skill animation
        _skillAnimationRunning = false;
    }

    protected override IEnumerator _RunUnitAnimation()
    {
        _unitAnimationHandler.RunWalkAnimation(transform.position, _target.transform.position);
        transform.position = Vector2.MoveTowards(transform.position, _target.transform.position, _speed * Time.deltaTime);

        yield return new WaitUntil(() =>
        {
            var delta = transform.position - _target.transform.position;
            var distance = delta.magnitude;
            return distance < _distanceToAttackTarget;
        });


        _unitAnimationHandler.RunAttackAnimation(transform.position, _target.transform.position);
        _unitAnimationRunning = false;
    }
}
