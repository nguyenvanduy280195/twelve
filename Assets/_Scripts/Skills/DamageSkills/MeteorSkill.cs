using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;


public class MeteorSkill : DamageSkillBase
{
    [Header("Meteor's info")]
    [SerializeField] private float _speed;

    [Tooltip("these stones will fall from top to down at the battle")]
    [SerializeField] private GameObject[] _listStoneForBattle;

    [Tooltip("These stones will fall from top to down at the target")]
    [SerializeField] private GameObject[] _listStoneForTarget;
    [SerializeField] private GameObject _explosionPrefab;
    [SerializeField] private float _stonesToTargetDelay;

    private List<Vector2> _destinationOfStoneForTarget;

    private readonly float _myEpsilon = 0.00001f;
    private IDictionary<GameObject, bool> _stonesArrived;
    private IDictionary<GameObject, Vector3> _stonesFirstPosition;
    private List<GameObject> _explosions;

    #region Unity methods

    private void Start()
    {
        _destinationOfStoneForTarget = new();

        _stonesArrived = new Dictionary<GameObject, bool>();

        _explosions = new();

        _stonesFirstPosition = new Dictionary<GameObject, Vector3>();
        foreach (var stone in _listStoneForBattle)
        {
            var x = Random.Range(-6f, -3f);
            var y = (_target?.transform.position.y + 7) ?? stone.transform.position.y;
            _stonesFirstPosition[stone] = new Vector2(x, y);
        }
        foreach (var stone in _listStoneForTarget)
        {
            var x = Random.Range(-6f, -3f);
            var y = (_target?.transform.position.y + 5) ?? stone.transform.position.y;
            _stonesFirstPosition[stone] = new Vector2(x, y);
        }
    }

    #endregion

    #region Inheriting via SkillBase
    protected override IEnumerator _RunUnitAnimation()
    {
        _unitAnimationHandler.RunCastRightAnimation();
        yield return new WaitUntil(() => !_unitAnimationHandler.CurrentStateLocked);
        _unitAnimationHandler.RunIdleRightAnimation();

        _unitAnimationRunning = false;
    }

    private IEnumerator _OnStoneToBattleArrived(GameObject stone)
    {
        yield return new WaitUntil(() => _stonesArrived[stone]);

        var explosion = Instantiate(_explosionPrefab, stone.transform.position, Quaternion.identity, transform);
        _explosions.Add(explosion);

        stone.SetActive(false);

    }

    private IEnumerator _OnStoneToTargetArrived(GameObject stone)
    {
        yield return new WaitUntil(() => _stonesArrived[stone]);
        _LetTargetTakeDamage(_target);
    }

    protected override IEnumerator _RunSkillAnimation(BattleUnitBase target)
    {
        _ResetPosition();
        _listStoneForBattle.ToList().ForEach(stone => _stonesArrived[stone] = false);
        _listStoneForTarget.ToList().ForEach(stone => _stonesArrived[stone] = false);

        yield return new WaitUntil(() => !_harvestingItemsRunning);

        for (int i = 0; i < _listStoneForBattle.Length; i++)
        {
            _listStoneForBattle[i].SetActive(true);
            StartCoroutine(_MoveTo(_listStoneForBattle[i].transform, _destinationOfStoneForTarget[i]));
            StartCoroutine(_OnStoneToBattleArrived(_listStoneForBattle[i]));
            yield return null;
        }

        for (int i = 0; i < _listStoneForTarget.Length; i++)
        {
            _listStoneForTarget[i].SetActive(true);
            StartCoroutine(_MoveTo(_listStoneForTarget[i].transform, target.transform.position));
            StartCoroutine(_OnStoneToTargetArrived(_listStoneForTarget[i]));
            yield return new WaitForSeconds(_stonesToTargetDelay);
        }

        // waiting for all stones arrived
        yield return new WaitUntil(() =>
        {
            var arrivedStones = _stonesArrived.Select(it => it.Value);
            foreach (var arrivedStone in arrivedStones)
            {
                if (!arrivedStone)
                {
                    return false;
                }
            }
            return true;
        });

        // waiting for explosion destroyed
        yield return new WaitUntil(() =>
        {
            foreach(var explosion in _explosions)
            {
                if(explosion != null)
                {
                    return false;
                }
            }
            return true;
        });

        _skillAnimationRunning = false;
        yield return null;
    }

    protected override IEnumerator _RunHavestingItems()
    {
        var battleGameManager = BattleGameManager.Instance;
        if (battleGameManager != null)
        {
            _destinationOfStoneForTarget.Clear();

            var battleData = battleGameManager.MyData;
            var items = battleData.Items;

            var itemsWillBeHarvested = new List<GameObject>();
            for (int iNumber = 0; iNumber < _listStoneForBattle.Length; iNumber++)
            {
                var iRandomRow = Random.Range(1, battleData.NumberOfRow - 2);
                var iRandomCol = Random.Range(1, battleData.NumberOfColumn - 2);

                var destination = items[iRandomCol, iRandomRow].transform.position;
                _destinationOfStoneForTarget.Add(destination);

                for (int iCol = iRandomCol - 1; iCol <= iRandomCol + 1; iCol++)
                {
                    for (int iRow = iRandomRow - 1; iRow <= iRandomRow + 1; iRow++)
                    {
                        itemsWillBeHarvested.Add(items[iCol, iRow]);
                    }
                }
            }

            var theItemsWillBeHarvested = itemsWillBeHarvested.Distinct()
                                                            .OrderBy(it => it.GetComponent<Item>().col)
                                                            .OrderByDescending(it => it.GetComponent<Item>().row)
                                                            ;
            battleGameManager.HarvestItems(theItemsWillBeHarvested);
        }
        _harvestingItemsRunning = false;
        yield return null;
    }

    #endregion


    #region Supports methods

    private bool _IsContactingTarget(Vector3 from, Vector3 to)
    {
        var delta = from - to;
        return delta.magnitude < _myEpsilon;
    }

    private IEnumerator _MoveTo(Transform from, Vector2 to)
    {
        while (!_IsContactingTarget(from.transform.position, to))
        {
            from.position = Vector2.MoveTowards(from.position, to, _speed * Time.deltaTime);
            yield return null;
        }
        _stonesArrived[from.gameObject] = true;
    }

    private void _ResetPosition()
    {
        foreach (var item in _listStoneForTarget)
        {
            item.transform.position = _stonesFirstPosition[item];
        }

        foreach (var item in _listStoneForBattle)
        {
            item.transform.position = _stonesFirstPosition[item];
        }
    }
    private void _LetTargetTakeDamage(BattleUnitBase target)
    {
        var damage = SkillDamageCalculator.Instance?.GetDamage(SkillName.Meteor, _battleUnitBase.Stat.Attack, _SkillData.Level) ?? -2f;
        target?.TakeHit(damage);
    }

    #endregion

}
