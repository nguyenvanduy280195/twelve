
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.Rendering;
using Random = UnityEngine.Random;

[RequireComponent(typeof(Data))]
public class Leader : MonoBehaviour
{

    [SerializeField]
    private GameObject[] itemPrefabs;

    [SerializeField]
    private GameObject[] explosionPrefabs;

    [SerializeField]
    private GameObject _selector;

    public Data MyData { private set; get; }

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

    private float _bonusFactor = 1f;

    private Player TargetPlayer => _currentPlayer == MyData.player1 ? MyData.player2 : MyData.player1;

    private GameObject RandomItem => itemPrefabs[Random.Range(0, itemPrefabs.Length)];

    private void Awake()
    {
        _origin = transform.position;
        _matchedItems = new List<GameObject>();

        MyData = GetComponent<Data>();
        MyData.Items.Destroy = go => DestroyItem(go);

        ExplosionAnimations = new List<GameObject>();

        var itemSpriteRenderer = itemPrefabs[0].GetComponent<SpriteRenderer>();
        if (itemSpriteRenderer != null)
        {
            _itemSize = itemSpriteRenderer.size * itemPrefabs[0].transform.localScale;
        }
    }

    private void Start()
    {
        InitializeItems(MyData.inputFilename);
    }

    private void Update()
    {
        if (MyData.GameState == GameState.ChoosingPlayer)
        {
            ChoosePlayer();
        }
        else if (MyData.GameState == GameState.Swapping)
        {
            Swap2Items();
        }
        else if (MyData.GameState == GameState.FindingMatches)
        {
            FindMatches();
        }
        else if (MyData.GameState == GameState.RemovingMatches)
        {
            RemoveMatches();
        }
        else if (MyData.GameState == GameState.UndoSwapping)
        {
            UndoSwapping();
        }
        else if (MyData.GameState == GameState.SetupItemsFall)
        {
            SetupItemsFall();
        }
        else if (MyData.GameState == GameState.SpawningNewItems)
        {
            SpawnItemsInColumns();
        }
        else if (MyData.GameState == GameState.ItemsFalling)
        {
            DoItemsFall();
        }
        else if (MyData.GameState == GameState.ScanningMatchesInAlteredColumns)
        {
            ScanMatchesInAlteredColumns();
            _bonusFactor++;
        }
        else if (MyData.GameState == GameState.ExplosionAnimationWaiting)
        {
            var explosionAlive = ExplosionAnimations.Select(it => it != null);
            if (explosionAlive.Count() <= 0)
            {
                MyData.GameState = GameState.SetupItemsFall;
            }
        }
        else if (MyData.GameState == GameState.CheckingGameOver)
        {
            CheckGameOver();
        }
        else if (MyData.GameState == GameState.WaitingForAnimationDone)
        {
            if (MyData.player1.Idle && MyData.player2.Idle)
            {
                MyData.GameState = GameState.CheckingGameOver;
            }
        }

        _currentPlayer?.ConsumeStamina(Time.deltaTime);
    }

    private void FixedUpdate()
    {
        if (_currentPlayer != null)
        {
            if (_currentPlayer.Mana >= _currentPlayer.maxMana && _currentPlayer.Idle)
            {
                _currentPlayer.DoSkill(TargetPlayer, _bonusFactor);
            }
        }
    }

    private void InitializeItems(string filename)
    {
        MyData.Items.Clear();

        if (filename.Length == 0)
        {
            InitializeItems();
            return;
        }

        var csvFile = Resources.Load<TextAsset>(filename);
        if (csvFile is null)
        {
            Debug.Log($"{filename} doesn't exist!!!");
            return;
        }

        var lines = csvFile.text.Split("\r\n");
        for (int r = 0; r < MyData.NumberOfRow; r++)
        {
            var cells = lines[MyData.NumberOfRow - 1 - r].Split(",");
            for (int c = 0; c < MyData.NumberOfColumn; c++)
            {
                var go = itemPrefabs.Where(it => it.tag == cells[c])
                                    .First();

                var newGameObjectPosition = _origin + new Vector2(c * _itemSize.x, r * _itemSize.y);
                var newGameObject = Instantiate(go, newGameObjectPosition, Quaternion.identity, transform);
                var newGameObjectItem = newGameObject.GetComponent<Item>();
                if (newGameObjectItem != null)
                {
                    newGameObjectItem.row = r;
                    newGameObjectItem.col = c;
                }
                MyData.Items[c, r] = newGameObject;
            }
        }
    }

    private void InitializeItems()
    {
        for (int r = 0; r < MyData.NumberOfRow; r++)
        {
            for (int c = 0; c < MyData.NumberOfColumn; c++)
            {
                GameObject newItem = null;
                int maxCount = 10;

                for (int i = 0; i < maxCount; i++)
                {
                    newItem = RandomItem;

                    var matchesOfTheItemInCol = MyData.ItemsSupporter.GetMatchesInCol(newItem);
                    var matchesOfTheItemInRow = MyData.ItemsSupporter.GetMatchesInRow(newItem);
                    if (matchesOfTheItemInCol.Count + 1 < MyData.MinMatches && matchesOfTheItemInRow.Count + 1 < MyData.MinMatches)
                    {
                        break;
                    }
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

                MyData.Items[c, r] = newGameObject;
            }
        }

        var alterCols = new List<int>();
        for (int iCol = 0; iCol < MyData.NumberOfColumn; iCol++)
        {
            alterCols.Add(iCol);
        }
        _alterCols = alterCols;

        MyData.GameState = GameState.ScanningMatchesInAlteredColumns;
    }

    public void ExportItems()
    {
        const string filename = "Assets/Resources/Text/output.csv";

        using (StreamWriter outputFile = File.CreateText(filename))
        {
            for (int r = MyData.Items.RowLength - 1; r >= 0; r--)
            {
                var line = "";
                for (int c = 0; c < MyData.Items.RowLength; c++)
                {
                    line += MyData.Items[c, r].tag + ",";
                }
                line = line.Remove(line.Length - 1);

                outputFile.WriteLine(line);
            }
        }
    }

    public void OnTestcaseEditTextSubmit(string value)
    {
        if (value.Length <= 0)
        {
            InitializeItems("");
            return;
        }

        var testcaseNumber = string.Format("{0:000}", int.Parse(value));
        InitializeItems("Text/" + testcaseNumber);
    }

    private void ChoosePlayer()
    {
        if (_currentPlayer == null)
        {
            if (MyData.player1.level >= MyData.player2.level)
            {
                _currentPlayer = MyData.player1;
            }
            else
            {
                _currentPlayer = MyData.player2;
            }

            _currentPlayer.nTurns = 1;
        }
        else
        {
            if (_currentPlayer.nTurns <= 0)
            {
                if (_currentPlayer == MyData.player1)
                {
                    _currentPlayer = MyData.player2;
                }
                else
                {
                    _currentPlayer = MyData.player1;
                }
                _currentPlayer.nTurns++;
            }
        }
        _bonusFactor = 1;
        _currentPlayer.nTurns--;
        _selector.transform.position = _currentPlayer.transform.position;

        if (_currentPlayer == MyData.player1)
        {
            MyData.GameState = GameState.Player1Turn;
        }
        else
        {
            MyData.GameState = GameState.Player2Turn;
        }
    }

    private void Swap2Items()
    {
        SelectedGameObject.transform.position = Vector2.MoveTowards(SelectedGameObject.transform.position, PreDraggedPosition, MyData.SwapAnimationDuration);
        DraggedGameObject.transform.position = Vector2.MoveTowards(DraggedGameObject.transform.position, PreSelectedPosition, MyData.SwapAnimationDuration);

        var delta = SelectedGameObject.transform.position - PreDraggedPosition;
        if (delta.magnitude < MyData.MyEpsilon)
        {
            MyData.ItemsSupporter.SwapItems(SelectedGameObject, DraggedGameObject);

            MyData.GameState = GameState.FindingMatches;
            Debug.Log("swapping has done");
        }
    }

    private void UndoSwapping()
    {
        SelectedGameObject.transform.position = Vector2.MoveTowards(SelectedGameObject.transform.position, PreSelectedPosition, MyData.SwapAnimationDuration);
        DraggedGameObject.transform.position = Vector2.MoveTowards(DraggedGameObject.transform.position, PreDraggedPosition, MyData.SwapAnimationDuration);

        var delta = SelectedGameObject.transform.position - PreSelectedPosition;
        if (delta.magnitude < MyData.MyEpsilon)
        {
            MyData.ItemsSupporter.SwapItems(SelectedGameObject, DraggedGameObject);

            MyData.GameState = GameState.ChoosingPlayer;
            Debug.Log("undo-swapping has done");
        }
    }

    private void FindMatches()
    {
        _matchedItems.Clear();

        Func<int, bool> predicate = nMatches => nMatches + 1 > MyData.MinMatches;

        var existMatchesInDragged = FindMatchedItems(DraggedGameObject, predicate);
        var existMatchesInSelected = FindMatchedItems(SelectedGameObject, predicate);

        if (!existMatchesInDragged && !existMatchesInSelected)
        {
            MyData.GameState = GameState.UndoSwapping;
            return;
        }

        MyData.GameState = GameState.RemovingMatches;
    }

    private void RemoveMatches()
    {
        if (_matchedItems == null || _matchedItems.Count() <= 0)
        {
            MyData.GameState = GameState.WaitingForAnimationDone;
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

                if (_currentPlayer != null)
                {
                    if (matchedItem.tag == "Attack")
                    {
                        _currentPlayer.Attack(TargetPlayer, _bonusFactor);
                    }
                    else if (matchedItem.tag == "HP")
                    {
                        _currentPlayer.RestoreHP(_bonusFactor);
                    }
                    else if (matchedItem.tag == "MP")
                    {
                        _currentPlayer.RestoreMana(_bonusFactor);
                    }
                    else if (matchedItem.tag == "Stamina")
                    {
                        _currentPlayer.RestoreStamina(_bonusFactor);
                    }
                    else if (matchedItem.tag == "Gold")
                    {
                        _currentPlayer.nGold += (int)_bonusFactor;
                    }
                    else if (matchedItem.tag == "Exp")
                    {
                        _currentPlayer.nExp += (int)_bonusFactor;
                    }
                }
                DestroyItem(matchedItem);

            }
        }

        _alterCols = alterCols.Distinct().OrderBy(it => it);

        MyData.GameState = GameState.ExplosionAnimationWaiting;
    }

    private void SetupItemsFall()
    {
        _itemsFallings = new List<(GameObject, Vector3)>();

        foreach (var alterCol in _alterCols)
        {
            for (int iRow = 0; iRow < MyData.NumberOfRow; iRow++)
            {
                if (MyData.Items[alterCol, iRow] == null)
                {
                    var itemsInColumn = MyData.Items.AsList
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

                        MyData.Items[item.col, item.row] = itemsInColumn[i];
                    }

                }
            }
        }

        MyData.GameState = GameState.SpawningNewItems;
    }

    private void SpawnItemsInColumns()
    {
        for (int iCol = 0; iCol < MyData.NumberOfColumn; iCol++)
        {
            var items = MyData.Items.AsList
                .Where(it =>
                {
                    if (it == null)
                    {
                        return false;
                    }
                    return it.GetItemCol() == iCol;
                })
                .Distinct();

            for (int iRow = items.Count(); iRow < MyData.NumberOfRow; iRow++)
            {
                var newItem = RandomItem;

                var newGameObjectPosition = new Vector2(_origin.x + iCol * _itemSize.x, _origin.y + (MyData.NumberOfRow + iRow - items.Count()) * _itemSize.y);
                var newGameObject = Instantiate(newItem, newGameObjectPosition, Quaternion.identity, transform);
                var newGameObjectItem = newGameObject.GetComponent<Item>();
                if (newGameObjectItem != null)
                {
                    newGameObjectItem.row = iRow;
                    newGameObjectItem.col = iCol;
                }

                MyData.Items[iCol, iRow] = newGameObject;

                var des = new Vector3(newGameObjectPosition.x, _origin.y + iRow * _itemSize.y);
                _itemsFallings.Add((newGameObject, des));
            }
        }

        MyData.GameState = GameState.ItemsFalling;
    }

    private void DoItemsFall()
    {
        if (_itemsFallings.Count <= 0)
        {
            MyData.GameState = GameState.ScanningMatchesInAlteredColumns;
            return;
        }

        bool itemsFell = true;

        foreach ((var go, var des) in _itemsFallings)
        {
            go.transform.position = Vector2.MoveTowards(go.transform.position, des, MyData.FallAnimationDuration);

            var delta = go.transform.position - des;
            if (delta.sqrMagnitude > MyData.MyEpsilon)
            {
                itemsFell = false;
            }
        }


        if (itemsFell)
        {
            MyData.GameState = GameState.ScanningMatchesInAlteredColumns;
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

        Func<int, bool> predicate = nMatches => nMatches + 1 > MyData.MinMatches;

        foreach (var iCol in _alterCols)
        {
            for (int iRow = 0; iRow < MyData.NumberOfRow; iRow++)
            {
                if (MyData.Items[iCol, iRow] != null)
                {
                    FindMatchedItems(MyData.Items[iCol, iRow], predicate);
                }
            }
        }

        MyData.GameState = GameState.RemovingMatches;
    }

    private void CheckGameOver()
    {
        if (MyData.player1.HP <= 0 || MyData.player1.Stamina <= 0)
        {
            Debug.Log("You lose");
            MyData.GameState = GameState.GameOver;
        }
        else if (MyData.player2.HP <= 0 || MyData.player2.Stamina <= 0)
        {
            Debug.Log("You win");
            MyData.GameState = GameState.GameOver;
        }
        else
        {
            MyData.GameState = GameState.ChoosingPlayer;
        }

    }

    //TODO bug: nMatches is MinMatches, bonus turn still increases
    private bool FindMatchedItems(GameObject go, Func<int, bool> predicateBonusTurn)
    {
        if (_matchedItems.Contains(go)) // in case matches in altered cols
        {
            return false;
        }

        var matchesInCol = MyData.ItemsSupporter.GetMatchesInCol(go);
        var matchesInRow = MyData.ItemsSupporter.GetMatchesInRow(go);

        if (matchesInCol.Count + 1 < MyData.MinMatches &&
            matchesInRow.Count + 1 < MyData.MinMatches)
        {
            return false;
        }

        Action<GameObject, List<GameObject>> action = (go, gos) =>
        {
            _matchedItems.Add(go);
            gos.ForEach(it => _matchedItems.Add(it));
        };

        if (matchesInCol.Count + 1 >= MyData.MinMatches)
        {
            action(go, matchesInCol);
        }

        if (matchesInRow.Count + 1 >= MyData.MinMatches)
        {
            action(go, matchesInRow);
        }


        if (predicateBonusTurn != null)
        {
            var nMatchesSelected = matchesInCol.Count + matchesInRow.Count;
            if (predicateBonusTurn(nMatchesSelected))
            {
                if (_currentPlayer != null)
                {
                    _currentPlayer.nTurns++;
                }
            }
        }

        return true;
    }

    public void DestroyItem(GameObject go)
    {
        var item = go.GetComponent<Item>();
        if (MyData.Items[item.col, item.row] != null)
        {
            MyData.Items[item.col, item.row] = null;
            Destroy(go);
        }
    }
}

public static class GameObjectExtension
{
    public static int GetItemRow(this GameObject go) => go.GetComponent<Item>().row;
    public static int GetItemCol(this GameObject go) => go.GetComponent<Item>().col;
}