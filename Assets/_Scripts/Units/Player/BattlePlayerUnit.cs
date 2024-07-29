using System;
using Unity.Burst.CompilerServices;
using UnityEngine;
using UnityEngine.Assertions;

[Serializable]
public class PlayerStat : UnitStat
{
    public int Gold;
    public int Exp;
}

public class BattlePlayerUnit : BattleUnitBase
{
    [SerializeField] private PlayerStat _stat;

    private bool IsDraggingSuccess(GameObject go) => IsDraggingSuccess(go.GetComponent<Item>());

    private bool IsDraggingSuccess(Item item)
    {
        var gameManager = BattleGameManager.Instance;
        bool duplicated = item.IsDuplicatedWith(gameManager.SelectedGameObject);
        bool neighbored = item.IsNeighborWith(gameManager.SelectedGameObject);
        return !duplicated && neighbored;
    }

    public override bool Control()
    {
        var gameManager = BattleGameManager.Instance;
 
        if (Input.GetMouseButtonDown(0))
        {
            var worldPoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            var hit = Physics2D.Raycast(worldPoint, Vector2.zero);
            if (hit.collider != null)
            {
                gameManager.SelectedGameObject = hit.collider.gameObject;
            }
        }

        if (Input.GetMouseButton(0) && gameManager.SelectedGameObject != null)
        {
            var worldPoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            var hit = Physics2D.Raycast(worldPoint, Vector2.zero);

            if (hit.collider != null && IsDraggingSuccess(hit.collider.gameObject))
            {
                gameManager.DraggedGameObject = hit.collider.gameObject;
                gameManager.PreSelectedPosition = gameManager.SelectedGameObject.transform.position;
                gameManager.PreDraggedPosition = gameManager.DraggedGameObject.transform.position;
                return true;
            }
        }

        return false;
    }

    protected override UnitStat GetStat() => _stat;
}
