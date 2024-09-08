using System;
using System.Collections;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class BattleEnemyUnit : BattleUnitBase
{
    [SerializeField] private EnemyStat _stat;

    private readonly ItemsSupporter.ItemLocation UNDEFINED = new(-1, -1);

    public override IEnumerator ControlCoroutine()
    {
        while (true)
        {
            var gameManager = BattleGameManager.Instance;

            (var iSelected, var iDragged) = Level1;
            if (!iSelected.Equals(UNDEFINED) && !iDragged.Equals(UNDEFINED))
            {
                gameManager.SelectedGameObject = gameManager.MyData.Items[iSelected.iCol, iSelected.iRow];
                gameManager.DraggedGameObject = gameManager.MyData.Items[iDragged.iCol, iDragged.iRow];
                gameManager.PreSelectedPosition = gameManager.SelectedGameObject.transform.position;
                gameManager.PreDraggedPosition = gameManager.DraggedGameObject.transform.position;
                break;
            }
            yield return null;
        }
    }

    public override bool Control()
    {
        var gameManager = BattleGameManager.Instance;

        (var iSelected, var iDragged) = Level1;
        if (!iSelected.Equals(UNDEFINED) && !iDragged.Equals(UNDEFINED))
        {
            gameManager.SelectedGameObject = gameManager.MyData.Items[iSelected.iCol, iSelected.iRow];
            gameManager.DraggedGameObject = gameManager.MyData.Items[iDragged.iCol, iDragged.iRow];
            gameManager.PreSelectedPosition = gameManager.SelectedGameObject.transform.position;
            gameManager.PreDraggedPosition = gameManager.DraggedGameObject.transform.position;

            return true;
        }

        return false;
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

    public override UnitStat Stat
    {
        get => _stat;
        set
        {
            _stat = (EnemyStat)value;
            _InitializeUIUnit();
        }
    }
}


