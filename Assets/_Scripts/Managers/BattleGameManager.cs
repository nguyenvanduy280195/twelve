using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using UnityEngine;
using Debug = UnityEngine.Debug;
using Random = UnityEngine.Random;
using IEnumerator = System.Collections.IEnumerator;


public class BattleGameManager : Singleton<BattleGameManager>
{
    #region Inspector's fields
    [SerializeField] private GameObject[] _itemPrefabs;

    [SerializeField] private GameObject[] _explosionPrefabs;

    [SerializeField] private GameObject _unitChooser;

    [SerializeField] private GameObject _itemSelector;

    [SerializeField] private Transform _origin;

    [SerializeField] private Canvas _winResult;

    [SerializeField] private Canvas _loseResult;

    [SerializeField] private float _delayHighlightMatches = 5f;

    [SerializeField] private Color _highlightColor = Color.white;
    #endregion

    #region private fields
    private GameState _gameState = GameState.Starting;

    private List<GameObject> _matchedItems;

    private IEnumerable<int> _alterCols;

    private List<(GameObject, Vector3)> _itemsFallings;

    private Vector2 _itemSize = Vector2.zero;

    private BattleUnitBase _currentUnit;

    private float _bonusFactor = 1f;

    private bool _resuming = true;

    private GameObject _itemSelected;
    #endregion

    #region Properties

    public BattleData MyData { private set; get; }

    public bool WaitingForSkill = false;

    public GameObject ItemSelected
    {
        get => _itemSelected;
        set
        {
            _itemSelected = value;

            if (_itemSelector != null && _itemSelected != null)
            {
                _itemSelector.SetActive(true);
                _itemSelector.transform.position = _itemSelected.transform.position;
            }
        }
    }

    [NonSerialized] public GameObject ItemDragged;
    public List<GameObject> ExplosionAnimations { get; private set; }

    public Vector3 PreSelectedPosition { private get; set; }
    public Vector3 PreDraggedPosition { private get; set; }

    #endregion

    #region Public methods
    public bool IsChoosingUnitState => _gameState == GameState.ChoosingUnit;
    public void SkipTurn() => _gameState = GameState.ChoosingUnit;

    public void SetResuming(bool value) => _resuming = value;

    public void HarvestItems(IEnumerable<GameObject> items)
    {
        _matchedItems.AddRange(items);
        _gameState = GameState.RemovingMatches;
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
        var watch = Stopwatch.StartNew();

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
                    newItem = _GetRandomItem(list);

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

        watch.Stop();
        Debug.Log($"_InitializeItems's time loading: {watch.ElapsedMilliseconds} ms");
    }

    public bool IsPlayerTurn => _gameState == GameState.PlayerTurn;

    #endregion

    private void Start()
    {
        StartCoroutine(_StartBattle());
        StartCoroutine(_CreateConsumingStaminaWorker());
    }

    private IEnumerator _StartBattle()
    {
        while (true)
        {
            yield return new WaitUntil(() => !GameManager.Instance?.IsPausing() ?? _resuming);

            Debug.Log($"_gameState = {_gameState}");

            yield return _SetState(_gameState);

            if (_gameState == GameState.GameOver)
            {
                break;
            }
        }
    }

    private IEnumerator _SetState(GameState gameState)
    {
        switch (gameState)
        {
            case GameState.Starting:
                yield return StartCoroutine(_HandleStarting());
                break;
            case GameState.ChoosingUnit:
                yield return StartCoroutine(_HandleChoosingPlayer());
                break;
            case GameState.PlayerTurn:
                yield return StartCoroutine(_HandlePlayerTurn());
                break;
            case GameState.EnemyTurn:
                yield return StartCoroutine(_HandleEnemyTurn());
                break;
            case GameState.Swapping:
                yield return StartCoroutine(_HandleSwaping2Items());
                break;
            case GameState.UndoSwapping:
                yield return StartCoroutine(_HandleUndoSwapping());
                break;
            case GameState.FindingMatches:
                yield return StartCoroutine(_HandleFindingMatches());
                break;
            case GameState.HighlightMatches:
                yield return StartCoroutine(_HandleHighlightMatches());
                break;
            case GameState.RemovingMatches:
                yield return StartCoroutine(_HandleRemovingMatches());
                break;
            case GameState.SetupItemsFall:
                yield return StartCoroutine(_HandleSetupItemsFall());
                break;
            case GameState.SpawningNewItems:
                yield return StartCoroutine(_HandleSpawningItemsInColumns());
                break;
            case GameState.ItemsFalling:
                yield return StartCoroutine(_HandleItemsFalling());
                break;
            case GameState.ScanningMatchesInAlteredColumns:
                yield return StartCoroutine(_HandleScanningMatchesInAlteredColumns());
                break;
            case GameState.WaitingForExplosionAnimation:
                yield return StartCoroutine(_HandleWaitingForExplosionAnimation());
                break;
            case GameState.CheckingGameOver:
                yield return StartCoroutine(_HandleCheckingGameOver());
                break;
            case GameState.CheckingNoSwappable:
                yield return StartCoroutine(_HandleCheckingCantSwap());
                break;
            case GameState.Rearrangement:
                yield return StartCoroutine(_HandleRearrangement());
                break;
            case GameState.WaitingForUnitAnimation:
                yield return StartCoroutine(_HandleWaitingForUnitAnimation());
                break;
            case GameState.Win:
                yield return StartCoroutine(_HandleWin());
                break;
            case GameState.Lose:
                yield return StartCoroutine(_HandleLose());
                break;
            case GameState.GameOver:
                break;
            case GameState.WaitingForSkill:
                yield return StartCoroutine(_HandleWaitingForSkill());
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(gameState), gameState, null);
        }
    }

    private IEnumerator _CreateConsumingStaminaWorker()
    {
        while (true)
        {
            _currentUnit?.ConsumeStamina();
            yield return null;
        }
    }

    #region All state handler

    private IEnumerator _HandleLose()
    {
        var player = BattleUnitManager.Instance.PlayerAsBattleUnitBase;
        var playerStat = player.Stat as PlayerData;

        var info = _loseResult?.GetComponent<LoseResultPopup>();
        info.Gold = playerStat.Gold;
        info.Exp = playerStat.Exp;

        _loseResult?.gameObject.SetActive(true);
        _gameState = GameState.GameOver;

        yield return null;
    }

    private IEnumerator _HandleWin()
    {
        var player = BattleUnitManager.Instance.PlayerAsBattleUnitBase;
        var enemy = BattleUnitManager.Instance.EnemyAsBattleUnitBase;
        var enemyStat = enemy.Stat as EnemyStat;

        var popup = _winResult.GetComponent<WinResultPopup>();
        popup.ExpInBattle = player.nExp;
        popup.ExpFromEnemy = enemyStat.BonusExp;
        popup.GoldInBattle = player.nGold;
        popup.GoldFromEnemy = enemyStat.BonusGold;
        _winResult.gameObject.SetActive(true);

        _gameState = GameState.GameOver;

        yield return null;
    }

    private IEnumerator _HandleWaitingForSkill() // TODO find a better way
    {
        yield return new WaitUntil(() => !WaitingForSkill);
        _gameState = GameState.ChoosingUnit;
    }

    private IEnumerator _HandleRearrangement() // TODO let try it with yield return null in loop
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
                    yield return null;
                }
            }
        }



        var alterCols = new List<int>();
        for (int iCol = 0; iCol < MyData.NumberOfColumn; iCol++)
        {
            alterCols.Add(iCol);
        }
        _alterCols = alterCols;

        _gameState = GameState.ItemsFalling;
        yield return null;
    }

    private IEnumerator _HandleCheckingCantSwap()
    {
        var nItems = MyData.ItemsSupporter.AllSwappableItems.Count();
        if (nItems <= 0)
        {
            _gameState = GameState.Rearrangement;
        }
        else
        {
            _gameState = GameState.WaitingForSkill;
        }
        yield return null;
    }

    private IEnumerator _HandleCheckingGameOver()
    {
        var player = BattleUnitManager.Instance.PlayerAsBattleUnitBase;
        var enemy = BattleUnitManager.Instance.EnemyAsBattleUnitBase;
        if (player.Dealth)
        {
            _gameState = GameState.Lose;
        }
        else if (enemy.Dealth)
        {
            _gameState = GameState.Win;
        }
        else
        {
            _gameState = GameState.CheckingNoSwappable;
        }
        yield return null;
    }

    private IEnumerator _HandleScanningMatchesInAlteredColumns()
    {
        _matchedItems.Clear();

        Func<int, bool> predicate = nMatches => nMatches + 1 > MyData.MinMatches;
        for (int iCol = 0; iCol < MyData.NumberOfColumn; iCol++)
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

        _gameState = GameState.HighlightMatches;
        yield return null;
    }

    private IEnumerator _HandleItemsFalling()
    {
        if (_itemsFallings.Count <= 0)
        {
            _itemsFallings.Clear();
            _gameState = GameState.ScanningMatchesInAlteredColumns;
            yield break;
        }


        while (true)
        {
            bool itemsFell = true;
            foreach ((var go, var des) in _itemsFallings)
            {
                go.transform.position = Vector2.MoveTowards(go.transform.position, des, MyData.FallingAnimationSpeed);

                var delta = go.transform.position - des;
                if (delta.sqrMagnitude > Mathf.Epsilon)
                {
                    // try a predicate: delta.sqrMagnitude > Mathf.Epsilon && !itemsFell
                    // => No. Because we need all items to fall
                    itemsFell = false;
                }
            }

            if (itemsFell)
            {
                break;
            }

            yield return null;
        }

        _gameState = GameState.ScanningMatchesInAlteredColumns;
    }

    private IEnumerator _HandleSpawningItemsInColumns()
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
                var newItem = _GetRandomItem();

                var newGameObjectX = _origin.position.x + iCol * _itemSize.x;
                var newGameObjectY = _origin.position.y + (MyData.NumberOfRow + iRow - items.Count()) * _itemSize.y;
                var newGameObjectPosition = new Vector2(newGameObjectX, newGameObjectY);
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

        _gameState = GameState.ItemsFalling;
        yield return null;
    }

    private IEnumerator _HandleSetupItemsFall()
    {
        if (_itemsFallings == null)
        {
            _itemsFallings = new List<(GameObject, Vector3)>();
        }
        if (_itemsFallings.Count > 0)
        {
            _itemsFallings.Clear();
        }

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

        _gameState = GameState.SpawningNewItems;
        yield return null;
    }

    private IEnumerator _HandleWaitingForExplosionAnimation()
    {
        var explosionAlive = ExplosionAnimations.Select(it => it != null);
        yield return new WaitUntil(() => explosionAlive.Count() <= 0);
        AudioManager.Instance?.PlayRemovingItem();
        _gameState = GameState.SetupItemsFall;
    }

    private IEnumerator _HandleRemovingMatches()
    {
        if (_matchedItems == null || _matchedItems.Count() <= 0)
        {
            _gameState = GameState.WaitingForUnitAnimation;
            yield break;
        }

        var alterCols = new List<int>();
        foreach (var matchedItem in _matchedItems)
        {
            if (matchedItem != null)
            {
                alterCols.Add(matchedItem.GetComponent<Item>().col);
                _CreateExplosionForMatchedItem(matchedItem);
                _HarvestItem(matchedItem);
                _DestroyItem(matchedItem);
            }
            yield return null;
        }

        _alterCols = alterCols.Distinct().OrderBy(it => it);
        _gameState = GameState.WaitingForExplosionAnimation;
    }

    private IEnumerator _HandleWaitingForUnitAnimation()
    {
        var player = BattleUnitManager.Instance.PlayerAsBattleUnitBase;
        var enemy = BattleUnitManager.Instance.EnemyAsBattleUnitBase;

        yield return new WaitUntil(() => (player.Idle || player.Dealth) && (enemy.Idle || enemy.Dealth));

        _gameState = GameState.CheckingGameOver;
    }

    private IEnumerator _HandleHighlightMatches()
    {
        if (_matchedItems == null || _matchedItems.Count() <= 0)
        {
            _gameState = GameState.WaitingForUnitAnimation;
            yield break;
        }

        foreach (var matchedItem in _matchedItems)
        {
            matchedItem.GetComponent<SpriteRenderer>().color = _highlightColor;
        }

        yield return new WaitForSeconds(_delayHighlightMatches);

        _gameState = GameState.RemovingMatches;
    }

    private IEnumerator _HandleUndoSwapping()
    {
        while (true)
        {
            ItemSelected.transform.position = Vector2.MoveTowards(ItemSelected.transform.position, PreSelectedPosition, MyData.SwappingAnimationSpeed);
            ItemDragged.transform.position = Vector2.MoveTowards(ItemDragged.transform.position, PreDraggedPosition, MyData.SwappingAnimationSpeed);

            var delta = ItemSelected.transform.position - PreSelectedPosition;
            if (delta.magnitude < Mathf.Epsilon)
            {
                MyData.ItemsSupporter.SwapItems(ItemSelected, ItemDragged);
                _gameState = GameState.ChoosingUnit;
                break;
            }
            yield return null;
        }
    }

    private IEnumerator _HandleFindingMatches()
    {
        _matchedItems.Clear();

        Func<int, bool> predicate = nMatches => nMatches + 1 > MyData.MinMatches;

        var existMatchesInDragged = _FindMatchedItems(ItemDragged, predicate);
        var existMatchesInSelected = _FindMatchedItems(ItemSelected, predicate);

        if (!existMatchesInDragged && !existMatchesInSelected)
        {
            _gameState = GameState.UndoSwapping;
        }
        else
        {
            _gameState = GameState.HighlightMatches;
        }

        yield return null;
    }

    private IEnumerator _HandleSwaping2Items()
    {
        while (true)
        {
            ItemSelected.transform.position = Vector2.MoveTowards(ItemSelected.transform.position, PreDraggedPosition, MyData.SwappingAnimationSpeed);
            ItemDragged.transform.position = Vector2.MoveTowards(ItemDragged.transform.position, PreSelectedPosition, MyData.SwappingAnimationSpeed);

            var delta = ItemSelected.transform.position - PreDraggedPosition;

            if (delta.magnitude < Mathf.Epsilon)
            {
                MyData.ItemsSupporter.SwapItems(ItemSelected, ItemDragged);
                _gameState = GameState.FindingMatches;
                break;
            }

            yield return null;
        }


        _itemSelector?.SetActive(false);
    }

    private IEnumerator _HandleEnemyTurn()
    {
        yield return StartCoroutine(BattleUnitManager.Instance.EnemyAsBattleEnemyUnit.ControlCoroutine());
        _gameState = GameState.Swapping;
    }

    private IEnumerator _HandlePlayerTurn()
    {
        yield return StartCoroutine(BattleUnitManager.Instance.PlayerAsBattleUnitBase.ControlCoroutine());
        if (ItemSelected != null && ItemDragged != null)
        {
            _gameState = GameState.Swapping;
        }

        // TODO Find a better way, because clicking skill is able to occur error if ItemSelected and ItemDragged are not null.

    }

    private IEnumerator _HandleChoosingPlayer()
    {
        var player = BattleUnitManager.Instance.PlayerAsBattleUnitBase;
        var enemy = BattleUnitManager.Instance.EnemyAsBattleUnitBase;

        if (_currentUnit.nTurns <= 0)
        {
            if (_currentUnit == player)
            {
                _currentUnit = enemy;
            }
            else
            {
                _currentUnit = player;
            }
            _currentUnit.nTurns++;
        }

        _bonusFactor = 1;
        _currentUnit.nTurns--;

        if (_unitChooser != null)
        {
            _unitChooser.transform.position = _currentUnit.transform.position;
        }


        if (_currentUnit == player)
        {
            _gameState = GameState.PlayerTurn;
        }
        else
        {
            _gameState = GameState.EnemyTurn;
        }
        yield return null;
    }

    private IEnumerator _HandleStarting()
    {
        _matchedItems = new List<GameObject>();

        MyData = GetComponent<BattleData>();
        MyData.Items.DestroyItemCallback = go => _DestroyItem(go);

        ExplosionAnimations = new List<GameObject>();

        var itemSpriteRenderer = _itemPrefabs[0].GetComponent<SpriteRenderer>();
        if (itemSpriteRenderer != null)
        {
            _itemSize = itemSpriteRenderer.size * _itemPrefabs[0].transform.localScale;
        }

        InitializeItems(MyData.inputFilename);

        _ChooseUnitInFirstTurn();
        yield return null;
    }

    #endregion

    #region Support methods

    private void _ChooseUnitInFirstTurn()
    {

        var player = BattleUnitManager.Instance.PlayerAsBattleUnitBase;
        var enemy = BattleUnitManager.Instance.EnemyAsBattleUnitBase;

        if (player.Stat.Level >= enemy.Stat.Level)
        {
            _currentUnit = player;
            _gameState = GameState.PlayerTurn;
        }
        else
        {
            _currentUnit = enemy;
            _gameState = GameState.EnemyTurn;
        }

        if (_unitChooser != null)
        {
            _unitChooser.transform.position = _currentUnit.transform.position;
        }
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
                if (_currentUnit != null)
                {
                    _currentUnit.nTurns++;
                }
            }

            if (predicateBonusTurn(matchesInCol.Count))
            {
                if (_currentUnit != null)
                {
                    _currentUnit.nTurns++;
                }
            }
        }

        return true;
    }

    private BattleUnitBase _GetTargetUnit()
    {
        var unitManager = BattleUnitManager.Instance;
        if (unitManager == null)
        {
            return null;
        }
        else if (_currentUnit == unitManager.PlayerAsBattleUnitBase)
        {
            return unitManager.EnemyAsBattleUnitBase;
        }

        return unitManager.PlayerAsBattleUnitBase;
    }

    private GameObject _GetRandomItem() => _GetRandomItem(_itemPrefabs);

    private GameObject _GetRandomItem(List<GameObject> gos) => _GetRandomItem(gos.ToArray());

    private GameObject _GetRandomItem(GameObject[] gos) => gos[Random.Range(0, gos.Length)];

    private void _DestroyItem(GameObject go)
    {
        var item = go.GetComponent<Item>();
        if (MyData.Items[item.col, item.row] != null)
        {
            MyData.Items[item.col, item.row] = null;
            Destroy(go);
        }
    }

    private void _HarvestItem(GameObject item)
    {
        if (item.tag == "Attack")
        {
            _currentUnit?.Attack(_GetTargetUnit(), _bonusFactor);
        }
        else if (item.tag == "HP")
        {
            _currentUnit?.RestoreHPByFormula(_bonusFactor);
        }
        else if (item.tag == "MP")
        {
            _currentUnit?.RestoreManaByFormula(_bonusFactor);
        }
        else if (item.tag == "Stamina")
        {
            _currentUnit?.RestoreStaminaByFormula(_bonusFactor);
        }
        else if (item.tag == "Gold")
        {
            _currentUnit?.IncreaseGold(_bonusFactor);
        }
        else if (item.tag == "Exp")
        {
            _currentUnit?.IncreaseExp(_bonusFactor);
        }
    }

    private void _CreateExplosionForMatchedItem(GameObject item)
    {
        if (_explosionPrefabs.Length != 0)
        {
            var explosion = Instantiate(_explosionPrefabs[0], item.transform.position, Quaternion.identity, transform);
            ExplosionAnimations.Add(explosion);
        }
    }

    #endregion
}



[SerializeField]
public enum GameState
{
    Starting = 0,
    ChoosingUnit,
    PlayerTurn,
    EnemyTurn,
    Swapping,
    UndoSwapping,
    FindingMatches,
    HighlightMatches,
    RemovingMatches,
    SetupItemsFall,
    SpawningNewItems,
    ItemsFalling,
    ScanningMatchesInAlteredColumns,
    WaitingForExplosionAnimation,
    WaitingForUnitAnimation,
    CheckingGameOver,
    CheckingNoSwappable,
    Rearrangement,
    WaitingForSkill,
    Win,
    Lose,
    GameOver

}