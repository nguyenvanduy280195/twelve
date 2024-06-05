using System.Linq;
using UnityEngine;
using UnityEngine.Assertions;

public class AIController : MonoBehaviour
{
    [SerializeField]
    private Leader _leader;

    private ItemsSupporter.ItemLocation UNDEFINED = new ItemsSupporter.ItemLocation(-1, -1);

    private void Awake()
    {
        Assert.IsNotNull(_leader, "Please assign 'Leader'");
    }

    private void Update()
    {
        if (_leader.MyData.GameState == GameState.Player2Turn)
        {
            (var iSelected, var iDragged) = Level1;
            if (!iSelected.Equals(UNDEFINED) && !iDragged.Equals(UNDEFINED))
            {
                _leader.SelectedGameObject = _leader.MyData.Items[iSelected.iCol, iSelected.iRow];
                _leader.DraggedGameObject = _leader.MyData.Items[iDragged.iCol, iDragged.iRow];
                _leader.PreSelectedPosition = _leader.SelectedGameObject.transform.position;
                _leader.PreDraggedPosition = _leader.DraggedGameObject.transform.position;

                _leader.MyData.GameState = GameState.Swapping;
            }
        }
    }

    private (ItemsSupporter.ItemLocation, ItemsSupporter.ItemLocation) Level1
    {
        get
        {
            var locationsOfSwappableItem = _leader.MyData.ItemsSupporter.AllSwappableItems.Distinct();
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
            var locationsOfSwappableItem = _leader.MyData.ItemsSupporter.AllSwappableItems.Distinct();
            (var iSelected, var iDragged, _) = locationsOfSwappableItem.Distinct()
                                                                       .Aggregate((it1, it2) => it1.Item3 > it2.Item3 ? it1 : it2);
            return (iSelected, iDragged);
        }
    }
}


