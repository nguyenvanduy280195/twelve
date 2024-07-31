using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Assertions;

public abstract class BattleUnitBase : MonoBehaviour
{
    //============= Properties =============
    [SerializeField] private Bar _healthBar;
    [SerializeField] private Bar _manaBar;
    [SerializeField] private Bar _staminaBar;
    [SerializeField] private Transform _attackPosition;
    [SerializeField] private float _moveDuration = 1f;
    [SerializeField] private TextMeshProUGUI _nTurnsText;

    [NonSerialized] public int nGold = 0;
    [NonSerialized] public int nExp = 0;
    [NonSerialized] public bool Pausing = true;

    // ============= Fields =============
    private Vector3 _playerPosition = Vector3.zero;
    private UnitState _state = UnitState.Idle;
    private int _nTurns;
    private IDictionary<UnitState, Queue<Action>> _actions;
    private UnitAnimationHandler _animationHandler;

    //============= Getters & Setters =============

    public bool Idle => _state == UnitState.Idle;

    public bool Dealth => _state == UnitState.Dead;

    public float HP => Mathf.Floor(_healthBar.Value);

    public float Stamina => Mathf.Floor(_staminaBar.Value);

    public int nTurns
    {
        get => _nTurns;
        set
        {
            _nTurns = value;
            _nTurnsText.text = $"{value}";
        }
    }

    public UnitStat Stat => GetStat();

    //============= Methods for Inheriting =============

    public abstract bool Control();

    protected abstract UnitStat GetStat();


    //============= public Methods =============

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
                _animationHandler.RunIdleAnimation(transform.position, _playerPosition);
            });

            // active
            _state = UnitState.MoveToTargetPosition;
            _animationHandler.RunWalkAnimation(transform.position, target.transform.position);
        }

        _actions[UnitState.Attack].Enqueue(() =>
        {
            //_animationHandler.RunIdleRightAnimation();
            _animationHandler.RunAttackAnimation(transform.position, _playerPosition);

            target.TakeHit(Stat.Attack * bonusFactor);
        });
    }

    public void TakeHit(float damage)
    {
        _healthBar.Value -= damage;
        if (_healthBar.Value > 0)
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

    public void ConsumeStamina(float value)
    {
        _staminaBar.Value -= value;
    }

    public void RestoreHP(float bonusFactor)
    {
        _healthBar.Value += Stat.RegenHP * bonusFactor;
    }

    public void RestoreMana(float bonusFactor)
    {
        _manaBar.Value += Stat.RegenMana * bonusFactor;
    }

    public void RestoreStamina(float bonusFactor)
    {
        _staminaBar.Value += Stat.RegenStamina * bonusFactor;
    }

    #region ============= Unity Methods =============

    private void Start()
    {
        _healthBar.MaxValue = Stat.HP;
        _healthBar.Value = Stat.HP;
        _manaBar.MaxValue = Stat.Mana;
        _manaBar.Value = 0f;
        _staminaBar.MaxValue = Stat.Stamina;
        _staminaBar.Value = Stat.Stamina;

        _playerPosition = transform.position;

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
        Debug.Log($"UnitState = {_state}");

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

    private void FixedUpdate()
    {
        if (HP <= 0)
        {
            _state = UnitState.Dead;
        }
    }

    #endregion

    #region ============= Support Methods =============

    private void _HandleMoveToTargetPositionState()
    {
        transform.position = Vector2.MoveTowards(transform.position, _attackPosition.position, _moveDuration);
        var delta = transform.position - _attackPosition.position;
        if (delta.sqrMagnitude < 0.001f)
        {
            _actions?[UnitState.MoveToTargetPosition]?.Dequeue()?.Invoke();
        }
    }

    private void _HandleMoveToStartPositionState()
    {
        transform.position = Vector2.MoveTowards(transform.position, _playerPosition, _moveDuration);
        var delta = transform.position - _playerPosition;
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
                _animationHandler.RunWalkAnimation(transform.position, _playerPosition);
            }
            else
            {
                Debug.Log($"[Attack] - _actions[UnitState.Attack].Count = {_actions[UnitState.Attack].Count}");
                _actions?[UnitState.Attack]?.Dequeue()?.Invoke();
            }
        }
    }

    private void _HandleWaitState() => _actions?[UnitState.Wait]?.Dequeue()?.Invoke();

    private void _HandleHurtState()
    {
        if (!_animationHandler.CurrentStateLocked)
        {
            if (_actions[UnitState.Hurt].Count <= 0)
            {
                _state = UnitState.Idle;
                _animationHandler.RunIdleAnimation(transform.position, _attackPosition.position);
            }
            else
            {
                Debug.Log($"[Hurt] - _actions[UnitState.Hurt].Count = {_actions[UnitState.Hurt].Count}");
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

[Serializable]
public class UnitStat
{
    public string Name;
    public string Class;
    public int Level;
    public float Attack;
    public float HP;
    public float RegenHP;
    public float Mana;
    public float RegenMana;
    public float Stamina;
    public float RegenStamina;
}
