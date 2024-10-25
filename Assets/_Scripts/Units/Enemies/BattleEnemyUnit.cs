using System;
using System.Collections;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class BattleEnemyUnit : BattleUnitBase
{
    [SerializeField] private EnemyStat _stat;
    [SerializeField] private float _delayMove = 0.5f;

    private readonly ItemsSupporter.ItemLocation UNDEFINED = new(-1, -1);

    public override IEnumerator ControlCoroutine()
    {
        while (true)
        {
            var gameManager = BattleGameManager.Instance;

            (var iSelected, var iDragged) = Level1;
            if (!iSelected.Equals(UNDEFINED) && !iDragged.Equals(UNDEFINED))
            {
                gameManager.ItemSelected = gameManager.MyData.Items[iSelected.iCol, iSelected.iRow];
                gameManager.ItemDragged = gameManager.MyData.Items[iDragged.iCol, iDragged.iRow];
                gameManager.PreSelectedPosition = gameManager.ItemSelected.transform.position;
                gameManager.PreDraggedPosition = gameManager.ItemDragged.transform.position;
                break;
            }
            yield return null;
        }

        yield return new WaitForSeconds(_delayMove);
    }

    private (ItemsSupporter.ItemLocation, ItemsSupporter.ItemLocation) Level1
    {
        get
        {
            var gameManager = BattleGameManager.Instance;
            var locationsOfSwappableItem = gameManager.MyData.ItemsSupporter.AllSwappableItems.Distinct();
            if (locationsOfSwappableItem.Count() <= 0)
            {
                return (UNDEFINED, UNDEFINED);
            }
            var randomIndex = Random.Range(0, locationsOfSwappableItem.Count());
            (var iSelected, var iDragged, _) = locationsOfSwappableItem.ElementAt(randomIndex);
            return (iSelected, iDragged);
        }
    }



    private (ItemsSupporter.ItemLocation, ItemsSupporter.ItemLocation) Level10
    {
        get
        {
            var gameManager = BattleGameManager.Instance;
            var locationsOfSwappableItem = gameManager.MyData.ItemsSupporter.AllSwappableItems.Distinct();
            (var iSelected, var iDragged, _) = locationsOfSwappableItem.Distinct()
                                                                       .Aggregate((it1, it2) => it1.Item3 > it2.Item3 ? it1 : it2);
            return (iSelected, iDragged);
        }
    }

    protected override Vector3 UnitAttackPosition => BattleUnitManager.Instance.EnemyAttackPosition;

    protected override UIUnit UIUnit => BattleUIManager.Instance.Enemy;

    public override UnitData Stat
    {
        get => _stat;
        set
        {
            _stat = (EnemyStat)value;
            _InitializeUIUnit();
            Initialized = true;
        }
    }
}


