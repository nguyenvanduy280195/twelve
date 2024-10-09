using UnityEngine;

public class Item : MonoBehaviour
{
    public int row = -1;
    public int col = -1;

    public void Swap(Item other)
    {
        (row, other.row) = (other.row, row);
        (col, other.col) = (other.col, col);
    }

    public bool IsDuplicatedWith(GameObject go) => gameObject == go;

    public bool IsDuplicatedWith(Item item) => IsDuplicatedWith(item.gameObject);

    public bool IsNeighborWith(GameObject go) => IsNeighborWith(go.GetComponent<Item>());

    public bool IsNeighborWith(Item item) => IsNeighborInRow(item) || IsNeighborInCol(item);

    private bool IsNeighborInRow(Item item) => row == item.row && Mathf.Abs(col - item.col) <= 1;
    //private bool IsNeighborInRow(Item item) => row == item.row;

    private bool IsNeighborInCol(Item item) => col == item.col && Mathf.Abs(row - item.row) <= 1;
    //private bool IsNeighborInCol(Item item) => col == item.col;

}
