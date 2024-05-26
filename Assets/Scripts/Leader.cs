
using System;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using Random = UnityEngine.Random;

//TODO no matches to swap

[RequireComponent(typeof(Data))]
public class Leader : MonoBehaviour
{

    [SerializeField]
    private GameObject[] itemPrefabs;

    [SerializeField]
    private GameObject[] explosionPrefabs;

    [SerializeField]
    private GameObject _selector;

    public Data Data { private set; get; }

    [NonSerialized]
    public GameObject SelectedGameObject;

    [NonSerialized]
    public GameObject DraggedGameObject;

    [NonSerialized]
    public Vector3 PreSelectedPosition;

    [NonSerialized]
    public Vector3 PreDraggedPosition;

    [NonSerialized]
    public List<GameObject> ExplosionAnimations;

    private List<GameObject> _matchedItems;

    private IEnumerable<int> _alterCols;

    private List<(GameObject, Vector3)> _itemsFallings;

    private Vector2 _itemSize = Vector2.zero;

    private Vector2 _origin;

    private Player _currentPlayer;

    private int _bonusFactor = 1;

    private Player TargetPlayer => _currentPlayer == Data.player1 ? Data.player2 : Data.player1;

    private void Awake()
    {
        _origin = transform.position;
        _matchedItems = new List<GameObject>();

        Data = GetComponent<Data>();
        ExplosionAnimations = new List<GameObject>();

        var itemSpriteRenderer = itemPrefabs[0].GetComponent<SpriteRenderer>();
        if (itemSpriteRenderer != null)
        {
            _itemSize = itemSpriteRenderer.size * itemPrefabs[0].transform.localScale;
        }
    }

    private void Start()
    {
        InitializeItems();
    }

    private void Update()
    {
        if (Data.GameState == GameState.ChoosingPlayer)
        {
            ChoosePlayer();
        }
        if (Data.GameState == GameState.Swapping)
        {
            Swap2Items();
        }
        else if (Data.GameState == GameState.FindingMatches)
        {
            FindMatches();
        }
        else if (Data.GameState == GameState.RemovingMatches)
        {
            RemoveMatches();
        }
        else if (Data.GameState == GameState.UndoSwapping)
        {
            UndoSwapping();
        }
        else if (Data.GameState == GameState.SetupItemsFall)
        {
            SetupItemsFall();
        }
        else if (Data.GameState == GameState.SpawningNewItems)
        {
            SpawnItemsInColumns();
        }
        else if (Data.GameState == GameState.ItemsFalling)
        {
            DoItemsFall();
        }
        else if (Data.GameState == GameState.ScanningMatchesInAlteredColumns)
        {
            ScanMatchesInAlteredColumns();
            _bonusFactor++;
        }
        else if (Data.GameState == GameState.ExplosionAnimationWaiting)
        {
            var explosionAlive = ExplosionAnimations.Select(it => it != null);
            if (explosionAlive.Count() <= 0)
            {
                Data.GameState = GameState.SetupItemsFall;
            }
        }
        else if (Data.GameState == GameState.CheckingGameOver)
        {
            CheckGameOver();
        }
    }

    private void InitializeItems()
    {
        for (int r = 0; r < Data.NumberOfRow; r++)
        {
            for (int c = 0; c < Data.NumberOfColumn; c++)
            {
                GameObject newItem = null;
                int count = 0;
                int maxCount = 5;
                while (true)
                {
                    newItem = GetRandomItem();

                    var matchesOfTheItemInCol = Data.ItemsSupporter.GetMatchesInCol(newItem);
                    var matchesOfTheItemInRow = Data.ItemsSupporter.GetMatchesInRow(newItem);
                    if (matchesOfTheItemInCol.Count + 1 < Data.MinNumberOfMatches && matchesOfTheItemInRow.Count + 1 < Data.MinNumberOfMatches)
                    {
                        break;
                    }
                    if (count > maxCount)
                    {
                        break;
                    }

                    count++;
                }

                if (newItem == null)
                {
                    Debug.LogWarning("InitializeItems() has something wrong");
                    continue;
                }

                var newGameObjectPosition = _origin + new Vector2(c * _itemSize.x, r * _itemSize.y);
                var newGameObject = Instantiate(newItem, newGameObjectPosition, Quaternion.identity, transform);
                var newGameObjectItem = newGameObject.GetComponent<Item>();
                if (newGameObjectItem != null)
                {
                    newGameObjectItem.row = r;
                    newGameObjectItem.col = c;
                }

                Data.Items[c, r] = newGameObject;
            }
        }

        var alterCols = new List<int>();
        for (int iCol = 0; iCol < Data.NumberOfColumn; iCol++)
        {
            alterCols.Add(iCol);
        }
        _alterCols = alterCols;

        Data.GameState = GameState.ScanningMatchesInAlteredColumns;
    }

    private GameObject GetRandomItem() => itemPrefabs[Random.Range(0, itemPrefabs.Length)];

    private void ChoosePlayer()
    {
        if (_currentPlayer == null)
        {
            if (Data.player1.level >= Data.player2.level)
            {
                _currentPlayer = Data.player1;
            }
            else
            {
                _currentPlayer = Data.player2;
            }

            _currentPlayer.nTurns = 1;
        }
        else
        {
            if (_currentPlayer.nTurns <= 0)
            {
                if (_currentPlayer == Data.player1)
                {
                    _currentPlayer = Data.player2;
                }
                else
                {
                    _currentPlayer = Data.player1;
                }
                _currentPlayer.nTurns++;
            }
        }
        _bonusFactor = 1;
        _currentPlayer.nTurns--;
        _selector.transform.position = _currentPlayer.transform.position;

        if (_currentPlayer == Data.player1)
        {
            Data.GameState = GameState.Player1Turn;
        }
        else
        {
            Data.GameState = GameState.Player2Turn;
        }
    }

    private void Swap2Items()
    {
        SelectedGameObject.transform.position = Vector2.MoveTowards(SelectedGameObject.transform.position, PreDraggedPosition, Data.SwapAnimationDuration);
        DraggedGameObject.transform.position = Vector2.MoveTowards(DraggedGameObject.transform.position, PreSelectedPosition, Data.SwapAnimationDuration);

        var delta = SelectedGameObject.transform.position - PreDraggedPosition;
        if (delta.magnitude < Data.MyEpsilon)
        {
            Data.ItemsSupporter.SwapItems(SelectedGameObject, DraggedGameObject);

            Data.GameState = GameState.FindingMatches;
            Debug.Log("swapping has done");
        }
    }

    private void UndoSwapping()
    {
        SelectedGameObject.transform.position = Vector2.MoveTowards(SelectedGameObject.transform.position, PreSelectedPosition, Data.SwapAnimationDuration);
        DraggedGameObject.transform.position = Vector2.MoveTowards(DraggedGameObject.transform.position, PreDraggedPosition, Data.SwapAnimationDuration);

        var delta = SelectedGameObject.transform.position - PreSelectedPosition;
        if (delta.magnitude < Data.MyEpsilon)
        {
            Data.ItemsSupporter.SwapItems(SelectedGameObject, DraggedGameObject);

            Data.GameState = GameState.ChoosingPlayer;
            Debug.Log("undo-swapping has done");
        }
    }

    private void FindMatches()
    {
        var matchesOfSelectedItemInCol = Data.ItemsSupporter.GetMatchesInCol(SelectedGameObject);
        var matchesOfSelectedItemInRow = Data.ItemsSupporter.GetMatchesInRow(SelectedGameObject);
        var matchesOfDraggedItemInCol = Data.ItemsSupporter.GetMatchesInCol(DraggedGameObject);
        var matchesOfDraggedItemInRow = Data.ItemsSupporter.GetMatchesInRow(DraggedGameObject);

        // if the swaping doesn't make matching, undo-swap
        if (!(matchesOfSelectedItemInCol.Count + 1 >= Data.MinNumberOfMatches ||
        matchesOfSelectedItemInRow.Count + 1 >= Data.MinNumberOfMatches ||
        matchesOfDraggedItemInCol.Count + 1 >= Data.MinNumberOfMatches ||
        matchesOfDraggedItemInRow.Count + 1 >= Data.MinNumberOfMatches))
        {
            Data.GameState = GameState.UndoSwapping;
            return;
        }

        // else add to matchingCandidates list
        _matchedItems.Clear();

        Action<GameObject, List<GameObject>, int> action = (go, gos, nMatches) =>
        {
            if (nMatches > Data.MinNumberOfMatches)
            {
                _currentPlayer.nTurns++;
            }

            _matchedItems.Add(go);
            gos.ForEach(it => _matchedItems.Add(it));
        };

        if (matchesOfSelectedItemInCol.Count + 1 >= Data.MinNumberOfMatches)
        {
            action(SelectedGameObject, matchesOfSelectedItemInCol, matchesOfSelectedItemInCol.Count + 1);
        }

        if (matchesOfSelectedItemInRow.Count + 1 >= Data.MinNumberOfMatches)
        {
            action(SelectedGameObject, matchesOfSelectedItemInRow, matchesOfSelectedItemInRow.Count + 1);
        }

        if (matchesOfDraggedItemInCol.Count + 1 >= Data.MinNumberOfMatches)
        {
            action(DraggedGameObject, matchesOfDraggedItemInCol, matchesOfDraggedItemInCol.Count + 1);
        }

        if (matchesOfDraggedItemInRow.Count + 1 >= Data.MinNumberOfMatches)
        {
            action(DraggedGameObject, matchesOfDraggedItemInRow, matchesOfDraggedItemInRow.Count + 1);
        }

        Data.GameState = GameState.RemovingMatches;
    }

    private void RemoveMatches()
    {
        if (_matchedItems == null || _matchedItems.Count() <= 0)
        {
            Data.GameState = GameState.CheckingGameOver;
            return;
        }


        var alterCols = new List<int>();
        foreach (var matchedItem in _matchedItems)
        {
            if (matchedItem != null)
            {
                alterCols.Add(matchedItem.GetItemCol());


                if (explosionPrefabs.Length != 0)
                {
                    var explosion = Instantiate(explosionPrefabs[0], matchedItem.transform.position, Quaternion.identity, transform);
                    ExplosionAnimations.Add(explosion);
                }

                _currentPlayer?.SetScore(matchedItem.tag, _bonusFactor);

                if (matchedItem.tag == "Attack")
                {
                    _currentPlayer?.Attack(TargetPlayer, _bonusFactor);
                }
                if (matchedItem.tag == "HP")
                {
                    _currentPlayer?.Restore(_bonusFactor);
                }

                DestroyItem(matchedItem);

            }
        }

        _alterCols = alterCols.Distinct().OrderBy(it => it);

        Data.GameState = GameState.ExplosionAnimationWaiting;
    }

    private void SetupItemsFall()
    {
        _itemsFallings = new List<(GameObject, Vector3)>();

        foreach (var alterCol in _alterCols)
        {
            for (int iRow = 0; iRow < Data.NumberOfRow; iRow++)
            {
                if (Data.Items[alterCol, iRow] == null)
                {
                    var itemsInColumn = Data.Items.AsList
                        .Where(it =>
                        {
                            if (it == null)
                            {
                                return false;
                            }
                            var item = it.GetComponent<Item>();

                            return item.col == alterCol && item.row > iRow;
                        })
                        .OrderBy(it => it.GetComponent<Item>().row)
                        .Distinct()
                        .ToArray();

                    for (int i = 0; i < itemsInColumn.Length; i++)
                    {
                        var item = itemsInColumn[i].GetComponent<Item>();

                        item.row = iRow + i;

                        var des = new Vector2(itemsInColumn[i].transform.position.x, _origin.y + _itemSize.y * (item.row));
                        if (itemsInColumn[i] != null && itemsInColumn[i].transform != null)
                        {
                            _itemsFallings.Add((itemsInColumn[i], des));
                        }

                        Data.Items[item.col, item.row] = itemsInColumn[i];
                    }

                }
            }
        }

        Data.GameState = GameState.SpawningNewItems;
    }

    private void SpawnItemsInColumns()
    {
        for (int iCol = 0; iCol < Data.NumberOfColumn; iCol++)
        {
            var items = Data.Items.AsList
                .Where(it =>
                {
                    if (it == null)
                    {
                        return false;
                    }
                    return it.GetItemCol() == iCol;
                })
                .Distinct();

            for (int iRow = items.Count(); iRow < Data.NumberOfRow; iRow++)
            {
                var newItem = GetRandomItem();

                var newGameObjectPosition = new Vector2(_origin.x + iCol * _itemSize.x, _origin.y + (Data.NumberOfRow + iRow - items.Count()) * _itemSize.y);
                var newGameObject = Instantiate(newItem, newGameObjectPosition, Quaternion.identity, transform);
                var newGameObjectItem = newGameObject.GetComponent<Item>();
                if (newGameObjectItem != null)
                {
                    newGameObjectItem.row = iRow;
                    newGameObjectItem.col = iCol;
                }

                Data.Items[iCol, iRow] = newGameObject;

                var des = new Vector3(newGameObjectPosition.x, _origin.y + iRow * _itemSize.y);
                _itemsFallings.Add((newGameObject, des));
            }
        }

        Data.GameState = GameState.ItemsFalling;
    }

    private void DoItemsFall()
    {
        if (_itemsFallings.Count <= 0)
        {
            Data.GameState = GameState.ScanningMatchesInAlteredColumns;
            return;
        }

        foreach ((var go, var des) in _itemsFallings)
        {
            go.transform.position = Vector2.MoveTowards(go.transform.position, des, Data.FallAnimationDuration);

        }

        //TODO find a new way is better than that
        foreach ((var go, var des) in _itemsFallings)
        {
            var delta = go.transform.position - des;
            if (delta.sqrMagnitude > Data.MyEpsilon)
            {
                Data.GameState = GameState.ItemsFalling;
                break;
            }
            Data.GameState = GameState.ScanningMatchesInAlteredColumns;
        }
    }

    private void ScanMatchesInAlteredColumns()
    {
        _matchedItems.Clear();

        Action<GameObject, List<GameObject>> action = (go, gos) =>
        {
            _matchedItems.Add(go);
            gos.ForEach(it => _matchedItems.Add(it));
        };

        foreach (var iCol in _alterCols)
        {
            for (int iRow = 0; iRow < Data.NumberOfRow; iRow++)
            {
                if (Data.Items[iCol, iRow] != null)
                {
                    var matchesOfCandidateInCol = Data.ItemsSupporter.GetMatchesInCol(Data.Items[iCol, iRow]);
                    if (matchesOfCandidateInCol.Count + 1 >= Data.MinNumberOfMatches)
                    {
                        action(Data.Items[iCol, iRow], matchesOfCandidateInCol);
                    }

                    var matchesOfCandidateInRow = Data.ItemsSupporter.GetMatchesInRow(Data.Items[iCol, iRow]);
                    if (matchesOfCandidateInRow.Count + 1 >= Data.MinNumberOfMatches)
                    {
                        action(Data.Items[iCol, iRow], matchesOfCandidateInRow);
                    }
                }
            }
        }

        Data.GameState = GameState.RemovingMatches;
    }

    private void CheckGameOver()
    {
        if (Data.player1.HP <= 0)
        {
            Debug.Log("You lose");
            Data.GameState = GameState.GameOver;
        }
        else if (Data.player2.HP <= 0)
        {
            Debug.Log("You win");
            Data.GameState = GameState.GameOver;
        }
        else
        {
            Data.GameState = GameState.ChoosingPlayer;
        }

    }

    public void DestroyItem(GameObject go)
    {
        var item = go.GetComponent<Item>();
        if (Data.Items[item.col, item.row] != null)
        {
            Data.Items[item.col, item.row] = null;
            Destroy(go);
        }
    }
}

public static class GameObjectExtension
{
    public static int GetItemRow(this GameObject go) => go.GetComponent<Item>().row;
    public static int GetItemCol(this GameObject go) => go.GetComponent<Item>().col;
}