using System.Linq;
using UnityEngine;
using UnityEngine.Assertions;

public class AIController : MonoBehaviour
{
    [SerializeField]
    private Leader _leader;

    private void Awake()
    {
        Assert.IsNotNull(_leader, "Please assign 'Leader'");
    }

    private void Update()
    {
        if (_leader.Data.GameState == GameState.Player2Turn)
        {
            (var iSelected, var iDragged) = Level1;

            _leader.SelectedGameObject = _leader.Data.Items[iSelected.iCol, iSelected.iRow];
            _leader.DraggedGameObject = _leader.Data.Items[iDragged.iCol, iDragged.iRow];
            _leader.PreSelectedPosition = _leader.SelectedGameObject.transform.position;
            _leader.PreDraggedPosition = _leader.DraggedGameObject.transform.position;

            _leader.Data.GameState = GameState.Swapping;
        }
    }

    private (ItemsSupporter.ItemLocation, ItemsSupporter.ItemLocation) Level1
    {
        get
        {
            var locationsOfSwappableItem = _leader.Data.ItemsSupporter.AllSwappableItems.Distinct();
            var randomIndex = Random.Range(0, locationsOfSwappableItem.Count());
            (var iSelected, var iDragged, _) = locationsOfSwappableItem.ElementAt(randomIndex);
            return (iSelected, iDragged);
        }
    }



    private (ItemsSupporter.ItemLocation, ItemsSupporter.ItemLocation) Level10
    {
        get
        {
            var locationsOfSwappableItem = _leader.Data.ItemsSupporter.AllSwappableItems.Distinct();
            (var iSelected, var iDragged, _) = locationsOfSwappableItem.Distinct()
                                                                       .Aggregate((it1, it2) => it1.Item3 > it2.Item3 ? it1 : it2);
            return (iSelected, iDragged);
        }
    }
}


