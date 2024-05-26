using Unity.Burst.CompilerServices;
using UnityEngine;
using UnityEngine.Assertions;

public class PlayerController : MonoBehaviour
{
    [SerializeField]
    private Leader _leader;

    private void Awake()
    {
        Assert.IsNotNull(_leader, "Please assign 'Leader'");
    }

    // Update is called once per frame
    void Update()
    {
        if (_leader.Data.GameState == GameState.Player1Turn)
        {
            if (Input.GetMouseButtonDown(0))
            {
                var worldPoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                var hit = Physics2D.Raycast(worldPoint, Vector2.zero);
                if (hit.collider != null)
                {
                    _leader.SelectedGameObject = hit.collider.gameObject;
                }
            }

            if (Input.GetMouseButton(0) && _leader.SelectedGameObject != null)
            {
                var worldPoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                var hit = Physics2D.Raycast(worldPoint, Vector2.zero);

                if (hit.collider != null && IsDraggingSuccess(hit.collider.gameObject))
                {
                    _leader.DraggedGameObject = hit.collider.gameObject;
                    _leader.PreSelectedPosition = _leader.SelectedGameObject.transform.position;
                    _leader.PreDraggedPosition = _leader.DraggedGameObject.transform.position;
                    _leader.Data.GameState = GameState.Swapping;
                }
            }
        }
    }

    private bool IsDraggingSuccess(GameObject go) => IsDraggingSuccess(go.GetComponent<Item>());

    private bool IsDraggingSuccess(Item item)
    {
        bool duplicated = item.IsDuplicatedWith(_leader.SelectedGameObject);
        bool neighbored = item.IsNeighborWith(_leader.SelectedGameObject);
        return !duplicated && neighbored;
    }

}
