using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Assertions;

public abstract class BattleUnitBase : MonoBehaviour
{
    //============= Properties =============
    [SerializeField] private float _moveDuration = 1f;
    [SerializeField] private TextMeshProUGUI _nTurnsText;

    // ============= Fields =============
    private UnitState _state = UnitState.Idle;
    private int _nTurns;
    private IDictionary<UnitState, Queue<Action>> _actions;
    private UnitAnimationHandler _animationHandler;
    private Vector3 _unitPosition;


    #region ============= Getters & Setters =============
    public int nGold { get; private set; } = 0;

    public int nExp { get; private set; } = 0;

    public bool Idle => _state == UnitState.Idle;

    public bool Dealth => _state == UnitState.Dead;

    public int nTurns
    {
        get => _nTurns;
        set
        {
            _nTurns = value;
            Debug.Log($"nTurns = {value}");

            if (_nTurnsText != null)
            {
                _nTurnsText.text = $"{value}";
            }
        }
    }

    public float HP
    {
        get => Stat.HP;
        set
        {
            Stat.HP = value;
            UIUnit.HP.Value = value;
        }
    }


    public float Mana
    {
        get => Stat.Mana;
        set
        {
            Stat.Mana = value;
            UIUnit.Mana.Value = value;
        }
    }

    public float Stamina
    {
        get => Stat.Stamina;
        set
        {
            Stat.Stamina = value;
            UIUnit.Stamina.Value = value;
        }
    }

    #endregion

    #region ============= Methods for Inheriting =============

    public abstract bool Control();

    public abstract UnitStat Stat { get; set; }

    protected abstract UIUnit UIUnit { get; }

    protected abstract Vector3 UnitAttackPosition { get; }

    #endregion

    #region ============= For Devired Class =============

    protected void _InitializeUIUnit()
    {
        UIUnit.HP.MaxValue = Stat.MaxHP;
        UIUnit.HP.Value = Stat.HP;
        UIUnit.Mana.MaxValue = Stat.MaxMana;
        UIUnit.Mana.Value = Stat.Mana;
        UIUnit.Stamina.MaxValue = Stat.MaxStamina;
        UIUnit.Stamina.Value = Stat.Stamina;
    }

    #endregion

    #region ============= public Methods =============

    public void DoSkill(BattleUnitBase target, float bonusFactor) { }

    public void Attack(BattleUnitBase target, float bonusFactor)
    {
        if (_actions[UnitState.Attack].Count <= 0)
        {
            _actions[UnitState.MoveToTargetPosition].Enqueue(() =>
            {
                _state = UnitState.Attack;
            });
            _actions[UnitState.MoveToStartPosition].Enqueue(() =>
            {
                _state = UnitState.Idle;
                _animationHandler.RunIdleAnimation(transform.position, UnitAttackPosition);
            });

            // active
            _state = UnitState.MoveToTargetPosition;
            _animationHandler.RunWalkAnimation(transform.position, target.transform.position);
        }

        _actions[UnitState.Attack].Enqueue(() =>
        {
            _animationHandler.RunAttackAnimation(transform.position, _unitPosition);
            target.TakeHit(Stat.Attack * bonusFactor);

            Debug.Log($"[Attack] Damage = {Stat.Attack * bonusFactor}");
        });
    }

    public void TakeHit(float damage)
    {
        HP -= damage;
        if (HP > 0)
        {
            _state = UnitState.Hurt;
            _animationHandler.RunHurtAnimation();
        }
        else
        {
            _state = UnitState.Dead;
            _animationHandler.RunDeadAnimation();
        }
    }

    public void IncreaseGold(float bonusFactor) => nGold += (int)bonusFactor;

    public void IncreaseExp(float bonusFactor) => nExp += (int)bonusFactor;

    public void ConsumeStamina(float value)
    {
        if (UIUnit.Stamina.Value > 0)
        {
            UIUnit.Stamina.Value -= value;
        }
        else
        {
            _state = UnitState.Dead;
        }
    }

    public void RestoreHP(float bonusFactor) => HP = Mathf.Min(HP + Stat.RegenHP * bonusFactor, Stat.MaxHP);

    public void RestoreMana(float bonusFactor) => Mana = Mathf.Min(Mana + Stat.RegenMana * bonusFactor, Stat.MaxMana);

    public void RestoreStamina(float bonusFactor) => Stamina = Mathf.Min(Stamina + Stat.RegenStamina * bonusFactor, Stat.MaxStamina);

    #endregion

    #region ============= Unity Methods =============

    private void Start()
    {
        _InitializeUIUnit();

        _unitPosition = transform.position;

        _animationHandler = GetComponent<UnitAnimationHandler>();

        _actions = new Dictionary<UnitState, Queue<Action>>();
        _actions[UnitState.MoveToTargetPosition] = new();
        _actions[UnitState.Attack] = new();
        _actions[UnitState.MoveToStartPosition] = new();
        _actions[UnitState.Wait] = new();
        _actions[UnitState.Hurt] = new();
        _actions[UnitState.Dead] = new();
    }

    private void Update()
    {
        //Debug.Log($"UnitState = {_state}");

        switch (_state)
        {
            case UnitState.Idle:
                break;
            case UnitState.MoveToTargetPosition:
                _HandleMoveToTargetPositionState();
                break;
            case UnitState.MoveToStartPosition:
                _HandleMoveToStartPositionState();
                break;
            case UnitState.Attack:
                _HandleAttackState();
                break;
            case UnitState.Wait:
                _HandleWaitState();
                break;
            case UnitState.Hurt:
                _HandleHurtState();
                break;
            case UnitState.Dead:
                _HandleDeadState();
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(_state), _state, null);
        }
    }

    #endregion

    #region ============= Support Methods =============

    private void _HandleMoveToTargetPositionState()
    {
        transform.position = Vector2.MoveTowards(transform.position, UnitAttackPosition, _moveDuration * Time.deltaTime);
        var delta = transform.position - UnitAttackPosition;
        if (delta.sqrMagnitude < 0.001f)
        {
            _actions?[UnitState.MoveToTargetPosition]?.Dequeue()?.Invoke();
        }
    }

    private void _HandleMoveToStartPositionState()
    {
        transform.position = Vector2.MoveTowards(transform.position, _unitPosition, _moveDuration * Time.deltaTime);
        var delta = transform.position - _unitPosition;
        if (delta.sqrMagnitude < 0.001f)
        {
            _actions?[UnitState.MoveToStartPosition]?.Dequeue()?.Invoke();
        }
    }

    private void _HandleAttackState()
    {
        if (!_animationHandler.CurrentStateLocked)
        {
            if (_actions[UnitState.Attack].Count <= 0)
            {
                _state = UnitState.MoveToStartPosition;
                _animationHandler.RunWalkAnimation(transform.position, _unitPosition);
            }
            else
            {
                _actions?[UnitState.Attack]?.Dequeue()?.Invoke();
            }
        }
    }

    private void _HandleWaitState() { }

    private void _HandleHurtState()
    {
        if (!_animationHandler.CurrentStateLocked)
        {
            if (_actions[UnitState.Hurt].Count <= 0)
            {
                _state = UnitState.Idle;
                _animationHandler.RunIdleAnimation(transform.position, UnitAttackPosition);
            }
            else
            {
                _actions?[UnitState.Hurt]?.Dequeue()?.Invoke();
            }
        }
    }

    private void _HandleDeadState() { }

    #endregion
}

[Serializable]
public enum UnitState
{
    MoveToTargetPosition,
    MoveToStartPosition,
    Attack,
    Idle,
    Wait,
    Hurt,
    Dead,
}
