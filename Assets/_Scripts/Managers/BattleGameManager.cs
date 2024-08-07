using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using Random = UnityEngine.Random;

public class BattleGameManager : Singleton<BattleGameManager>
{
    //============= Events =============

    public static event Action<GameState> OnBeforeStateChanged;
    public static event Action<GameState> OnAfterStateChanged;

    //============= Initialize by Inspector =============

    [SerializeField]
    private GameObject[] _itemPrefabs;

    [SerializeField]
    private GameObject[] _explosionPrefabs;

    [SerializeField]
    private GameObject _selector;

    [SerializeField]
    private Transform _origin;

    [SerializeField]
    private Canvas _winResult;

    [SerializeField]
    private Canvas _loseResult;

    //============= Readonly =============

    public Data MyData { private set; get; }

    public bool IsPlayerTurn => _state == GameState.PlayerTurn;

    public bool IsEnemyTurn => _state == GameState.EnemyTurn;

    //============= Setters =============

    [NonSerialized] public GameObject SelectedGameObject;
    [NonSerialized] public GameObject DraggedGameObject;
    public Vector3 PreSelectedPosition { private get; set; }
    public Vector3 PreDraggedPosition { private get; set; }

    [NonSerialized]
    public List<GameObject> ExplosionAnimations;

    //============= Fields =============

    private List<GameObject> _matchedItems;

    private IEnumerable<int> _alterCols;

    private List<(GameObject, Vector3)> _itemsFallings;

    private Vector2 _itemSize = Vector2.zero;

    private BattleUnitBase _currentPlayer;

    private float _bonusFactor = 1f;

    private BattleUnitBase _TargetPlayer
    {
        get
        {
            var unitManager = BattleUnitManager.Instance;
            return _currentPlayer == unitManager.Player ? unitManager.Enemy : unitManager.Player;
        }
    }

    private GameObject GetRandomItem() => GetRandomItem(_itemPrefabs);

    private GameObject GetRandomItem(List<GameObject> gos) => GetRandomItem(gos.ToArray());

    private GameObject GetRandomItem(GameObject[] gos) => gos[Random.Range(0, gos.Length)];

    private GameState _state;

    //============= Public Methods =============

    public void DestroyItem(GameObject go)
    {
        var item = go.GetComponent<Item>();
        if (MyData.Items[item.col, item.row] != null)
        {
            MyData.Items[item.col, item.row] = null;
            Destroy(go);
        }
    }

    public void InitializeItems(string filename)
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
            InitializeItems();
            return;
        }

        var lines = csvFile.text.Split("\r\n");
        for (int r = 0; r < MyData.NumberOfRow; r++)
        {
            var cells = lines[MyData.NumberOfRow - 1 - r].Split(",");
            for (int c = 0; c < MyData.NumberOfColumn; c++)
            {
                var go = _itemPrefabs.Where(it => it.tag == cells[c])
                                    .First();

                var newGameObjectPosition = new Vector2(_origin.position.x + c * _itemSize.x, _origin.position.y + r * _itemSize.y);
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

    public void InitializeItems()
    {
        var watch = System.Diagnostics.Stopwatch.StartNew();

        List<GameObject> list = new(_itemPrefabs);
        GameObject newItem;

        for (int r = 0; r < MyData.NumberOfRow; r++)
        {
            for (int c = 0; c < MyData.NumberOfColumn; c++)
            {
                list.Clear();
                list.AddRange(_itemPrefabs);
                while (true)
                {
                    newItem = GetRandomItem(list);

                    var matchesOfTheItemInCol = MyData.ItemsSupporter.GetMatchesInCol(c, r, newItem.tag);
                    var matchesOfTheItemInRow = MyData.ItemsSupporter.GetMatchesInRow(c, r, newItem.tag);
                    if (matchesOfTheItemInCol.Count + 1 < MyData.MinMatches && matchesOfTheItemInRow.Count + 1 < MyData.MinMatches)
                    {
                        break;
                    }
                    else
                    {
                        list.Remove(newItem);
                    }
                }

                var newGameObjectPosition = new Vector2(_origin.position.x + c * _itemSize.x, _origin.position.y + r * _itemSize.y);
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

        _state = GameState.ScanningMatchesInAlteredColumns;

        watch.Stop();
        Debug.Log($"_InitializeItems's time loading: {watch.ElapsedMilliseconds} ms");
    }

    //============= Private Methods =============

    private void Start() => _state = GameState.Starting;

    private void Update()
    {
        Debug.Log($"GameState: {_state}");

        switch (_state)
        {
            case GameState.Starting:
                _HandleStarting();
                break;
            case GameState.ChoosingPlayer:
                _HandleChoosingPlayer();
                break;
            case GameState.PlayerTurn:
                _HandlePlayerTurn();
                break;
            case GameState.EnemyTurn:
                _HandleEnemyTurn();
                break;
            case GameState.Swapping:
                _HandleSwaping2Items();
                break;
            case GameState.UndoSwapping:
                _HandleUndoSwapping();
                break;
            case GameState.FindingMatches:
                _HandleFindingMatches();
                break;
            case GameState.RemovingMatches:
                _HandleRemovingMatches();
                break;
            case GameState.SetupItemsFall:
                _HandleSetupItemsFall();
                break;
            case GameState.SpawningNewItems:
                _HandleSpawningItemsInColumns();
                break;
            case GameState.ItemsFalling:
                _HandleItemsFalling();
                break;
            case GameState.ScanningMatchesInAlteredColumns:
                _HandleScanningMatchesInAlteredColumns();
                break;
            case GameState.ExplosionAnimation:
                _HandleExplosionAnimation();
                break;
            case GameState.CheckingGameOver:
                _HandleCheckingGameOver();
                break;
            case GameState.CheckingNoSwappable:
                _HandleCheckingCantSwap();
                break;
            case GameState.Rearrangement:
                _HandleRearrangement();
                break;
            case GameState.UnitAnimation:
                _HandleUnitAnimation();
                break;
            case GameState.Win:
                _HandleWin();
                break;
            case GameState.Lose:
                _HandleLose();
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(_state), _state, null);
        }

        _currentPlayer?.ConsumeStamina(Time.deltaTime);
    }

    private bool _FindMatchedItems(GameObject go, Func<int, bool> predicateBonusTurn)
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
            if (predicateBonusTurn(matchesInRow.Count))
            {
                if (_currentPlayer != null)
                {
                    _currentPlayer.nTurns++;
                }
            }

            if (predicateBonusTurn(matchesInCol.Count))
            {
                if (_currentPlayer != null)
                {
                    _currentPlayer.nTurns++;
                }
            }
        }

        return true;
    }
    
    private void _HandleUnitAnimation()
    {
        var player = BattleUnitManager.Instance.Player;
        var enemy = BattleUnitManager.Instance.Enemy;
        if (player.Idle && enemy.Idle)
        {
            _state = GameState.CheckingGameOver;
        }
        if (player.Dealth || enemy.Dealth)
        {
            _state = GameState.CheckingGameOver;
        }
    }

    private void _HandleExplosionAnimation()
    {
        var explosionAlive = ExplosionAnimations.Select(it => it != null);
        if (explosionAlive.Count() <= 0)
        {
            _state = GameState.SetupItemsFall;
        }
    }

    private void _HandleStarting()
    {
        _matchedItems = new List<GameObject>();

        MyData = GetComponent<Data>();
        MyData.Items.DestroyItemCallback = go => DestroyItem(go);

        ExplosionAnimations = new List<GameObject>();

        var itemSpriteRenderer = _itemPrefabs[0].GetComponent<SpriteRenderer>();
        if (itemSpriteRenderer != null)
        {
            _itemSize = itemSpriteRenderer.size * _itemPrefabs[0].transform.localScale;
        }

        InitializeItems(MyData.inputFilename);

        var player = BattleUnitManager.Instance.Player;
        var enemy = BattleUnitManager.Instance.Enemy;

        if (player.Stat.Level >= enemy.Stat.Level)
        {
            _currentPlayer = player;
            _state = GameState.PlayerTurn;
        }
        else
        {
            _currentPlayer = enemy;
            _state = GameState.EnemyTurn;
        }
    }

    private void _HandleChoosingPlayer()
    {
        var player = BattleUnitManager.Instance.Player;
        var enemy = BattleUnitManager.Instance.Enemy;

        if (_currentPlayer.nTurns <= 0)
        {
            if (_currentPlayer == player)
            {
                _currentPlayer = enemy;
            }
            else
            {
                _currentPlayer = player;
            }
            _currentPlayer.nTurns++;
        }

        _bonusFactor = 1;
        _currentPlayer.nTurns--;
        _selector.transform.position = _currentPlayer.transform.position;


        if (_currentPlayer == player)
        {
            _state = GameState.PlayerTurn;
        }
        else
        {
            _state = GameState.EnemyTurn;
        }
    }

    private void _HandleSwaping2Items()
    {
        SelectedGameObject.transform.position = Vector2.MoveTowards(SelectedGameObject.transform.position, PreDraggedPosition, MyData.SwappingAnimationDuration);
        DraggedGameObject.transform.position = Vector2.MoveTowards(DraggedGameObject.transform.position, PreSelectedPosition, MyData.SwappingAnimationDuration);

        var delta = SelectedGameObject.transform.position - PreDraggedPosition;
        if (delta.magnitude < Mathf.Epsilon)
        {
            MyData.ItemsSupporter.SwapItems(SelectedGameObject, DraggedGameObject);

            _state = GameState.FindingMatches;
        }
    }

    private void _HandleUndoSwapping()
    {
        SelectedGameObject.transform.position = Vector2.MoveTowards(SelectedGameObject.transform.position, PreSelectedPosition, MyData.SwappingAnimationDuration);
        DraggedGameObject.transform.position = Vector2.MoveTowards(DraggedGameObject.transform.position, PreDraggedPosition, MyData.SwappingAnimationDuration);

        var delta = SelectedGameObject.transform.position - PreSelectedPosition;
        if (delta.magnitude < Mathf.Epsilon)
        {
            MyData.ItemsSupporter.SwapItems(SelectedGameObject, DraggedGameObject);

            _state = GameState.ChoosingPlayer;
        }
        else
        {
            _state = GameState.UndoSwapping;
        }
    }

    private void _HandleFindingMatches()
    {
        _matchedItems.Clear();

        Func<int, bool> predicate = nMatches => nMatches + 1 > MyData.MinMatches;

        var existMatchesInDragged = _FindMatchedItems(DraggedGameObject, predicate);
        var existMatchesInSelected = _FindMatchedItems(SelectedGameObject, predicate);

        if (!existMatchesInDragged && !existMatchesInSelected)
        {
            _state = GameState.UndoSwapping;
        }
        else
        {
            _state = GameState.RemovingMatches;
        }

    }

    private void _HandleRemovingMatches()
    {
        if (_matchedItems == null || _matchedItems.Count() <= 0)
        {
            _state = GameState.UnitAnimation;
            return;
        }

        var alterCols = new List<int>();
        foreach (var matchedItem in _matchedItems)
        {
            if (matchedItem != null)
            {
                alterCols.Add(matchedItem.GetComponent<Item>().col);


                if (_explosionPrefabs.Length != 0)
                {
                    var explosion = Instantiate(_explosionPrefabs[0], matchedItem.transform.position, Quaternion.identity, transform);
                    ExplosionAnimations.Add(explosion);
                }

                if (_currentPlayer != null)
                {
                    if (matchedItem.tag == "Attack")
                    {
                        _currentPlayer.Attack(_TargetPlayer, _bonusFactor);
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

        _state = GameState.ExplosionAnimation;
    }

    private void _HandleSetupItemsFall()
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

                        var des = new Vector2(itemsInColumn[i].transform.position.x, _origin.position.y + _itemSize.y * (item.row));
                        if (itemsInColumn[i] != null && itemsInColumn[i].transform != null)
                        {
                            _itemsFallings.Add((itemsInColumn[i], des));
                        }

                        MyData.Items[item.col, item.row] = itemsInColumn[i];
                    }

                }
            }
        }

        _state = GameState.SpawningNewItems;
    }

    private void _HandleSpawningItemsInColumns()
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
                    return it.GetComponent<Item>().col == iCol;
                })
                .Distinct();

            for (int iRow = items.Count(); iRow < MyData.NumberOfRow; iRow++)
            {
                var newItem = GetRandomItem();

                var newGameObjectPosition = new Vector2(_origin.position.x + iCol * _itemSize.x, _origin.position.y + (MyData.NumberOfRow + iRow - items.Count()) * _itemSize.y);
                var newGameObject = Instantiate(newItem, newGameObjectPosition, Quaternion.identity, transform);
                var newGameObjectItem = newGameObject.GetComponent<Item>();
                if (newGameObjectItem != null)
                {
                    newGameObjectItem.row = iRow;
                    newGameObjectItem.col = iCol;
                }

                MyData.Items[iCol, iRow] = newGameObject;

                var des = new Vector3(newGameObjectPosition.x, _origin.position.y + iRow * _itemSize.y);
                _itemsFallings.Add((newGameObject, des));
            }
        }

        _state = GameState.ItemsFalling;
    }

    private void _HandleItemsFalling()
    {
        if (_itemsFallings.Count <= 0)
        {
            _itemsFallings.Clear();
            _state = GameState.ScanningMatchesInAlteredColumns;
            return;
        }

        bool itemsFell = true;

        foreach ((var go, var des) in _itemsFallings)
        {
            go.transform.position = Vector2.MoveTowards(go.transform.position, des, MyData.FallingAnimationDuration);

            var delta = go.transform.position - des;
            if (delta.sqrMagnitude > Mathf.Epsilon)
            {
                itemsFell = false;
            }
        }

        if (itemsFell)
        {
            _state = GameState.ScanningMatchesInAlteredColumns;
            return;
        }

        _state = GameState.ItemsFalling;
    }

    private void _HandleScanningMatchesInAlteredColumns()
    {
        _matchedItems.Clear();

        Func<int, bool> predicate = nMatches => nMatches + 1 > MyData.MinMatches;

        foreach (var iCol in _alterCols)
        {
            for (int iRow = 0; iRow < MyData.NumberOfRow; iRow++)
            {
                if (MyData.Items[iCol, iRow] != null)
                {
                    _FindMatchedItems(MyData.Items[iCol, iRow], predicate);
                }
            }
        }

        _bonusFactor++;

        _state = GameState.RemovingMatches;
    }

    private void _HandleCheckingGameOver()
    {
        var player = BattleUnitManager.Instance.Player;
        var enemy = BattleUnitManager.Instance.Enemy;
        if (player.HP <= 0 || player.Stamina <= 0)
        {
            Debug.Log("You lose");
            _state = GameState.Lose;
        }
        else if (enemy.HP <= 0 || enemy.Stamina <= 0)
        {
            Debug.Log("You win");
            _state = GameState.Win;
        }
        else
        {
            _state = GameState.CheckingNoSwappable;
        }

    }

    private void _HandleCheckingCantSwap()
    {
        var nItems = MyData.ItemsSupporter.AllSwappableItems.Count();
        if (nItems <= 0)
        {
            _state = GameState.Rearrangement;
        }
        else
        {
            _state = GameState.ChoosingPlayer;
        }
    }

    private void _HandleRearrangement()
    {
        var items = new Dictionary<string, int>();

        foreach (var item in MyData.Items.AsList)
        {
            if (!items.ContainsKey(item.tag))
            {
                items[item.tag] = 0;
            }

            items[item.tag]++;
        }

        MyData.Items.Clear();
        _itemsFallings.Clear();

        for (int iRow = 0; iRow < MyData.NumberOfRow; iRow++)
        {
            for (int iCol = 0; iCol < MyData.NumberOfColumn; iCol++)
            {
                if (items.Keys.Count >= 0)
                {
                    var iItemPrefabs = Random.Range(0, _itemPrefabs.Length);
                    var key = _itemPrefabs[iItemPrefabs].tag;
                    while (items[key] <= 0)
                    {
                        iItemPrefabs = Random.Range(0, _itemPrefabs.Length);
                        key = _itemPrefabs[iItemPrefabs].tag;
                    }

                    items[key]--;

                    var newGameObjectPosition = new Vector2(_origin.position.x + iCol * _itemSize.x, _origin.position.y + (MyData.NumberOfRow + iRow - items.Count()) * _itemSize.y);
                    var newGameObject = Instantiate(_itemPrefabs[iItemPrefabs], newGameObjectPosition, Quaternion.identity, transform);
                    var newGameObjectItem = newGameObject.GetComponent<Item>();
                    if (newGameObjectItem != null)
                    {
                        newGameObjectItem.row = iRow;
                        newGameObjectItem.col = iCol;
                    }

                    MyData.Items[iCol, iRow] = newGameObject;

                    var des = new Vector3(newGameObjectPosition.x, _origin.position.y + iRow * _itemSize.y);
                    _itemsFallings.Add((newGameObject, des));
                }
            }
        }

        var alterCols = new List<int>();
        for (int iCol = 0; iCol < MyData.NumberOfColumn; iCol++)
        {
            alterCols.Add(iCol);
        }
        _alterCols = alterCols;

        _state = GameState.ItemsFalling;
    }

    private void _HandlePlayerTurn()
    {
        var result = BattleUnitManager.Instance.Player?.Control();
        if (result.HasValue && result.Value)
        {
            _state = GameState.Swapping;
        }
    }

    private void _HandleEnemyTurn()
    {
        var result = BattleUnitManager.Instance.Enemy?.Control();
        if (result.HasValue && result.Value)
        {
            _state = GameState.Swapping;
        }
    }

    private void _HandleWin()
    {
        var player = BattleUnitManager.Instance.Player;
        var playerStat = player.Stat as PlayerStat;
        playerStat.Gold += player.nGold;
        playerStat.Exp += player.nExp;

        _HandleResult(_winResult);
    }

    private void _HandleLose() => _HandleResult(_loseResult);

    private void _HandleResult(Canvas resultCanvas)
    {
        var player = BattleUnitManager.Instance.Player;
        var playerStat = player.Stat as PlayerStat;

        var info = resultCanvas?.GetComponent<ResultInfo>();
        info.Gold = $"{playerStat.Gold}";
        info.Exp = $"{playerStat.Exp}";

        resultCanvas?.gameObject.SetActive(true);
    }
}

[SerializeField]
public enum GameState
{
    Starting = 0,
    ChoosingPlayer,
    PlayerTurn,
    EnemyTurn,
    Swapping,
    UndoSwapping,
    FindingMatches,
    RemovingMatches,
    SetupItemsFall,
    SpawningNewItems,
    ItemsFalling,
    ScanningMatchesInAlteredColumns,
    ExplosionAnimation,
    UnitAnimation,
    CheckingGameOver,
    CheckingNoSwappable,
    Rearrangement,
    Win,
    Lose,
}