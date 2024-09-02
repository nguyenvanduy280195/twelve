using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public abstract class BattleUnitBase : MonoBehaviour
{
    #region============= Properties =============
    [SerializeField] private float _moveDuration = 1f;
    [SerializeField] private TextMeshProUGUI _nTurnsText;
    [SerializeField] private SkillBase[] _skills;
    #endregion

    #region ============= Fields =============
    private UnitState _state = UnitState.Idle;
    private int _nTurns;
    private IDictionary<UnitState, Queue<Action>> _actions;
    private UnitAnimationHandler _animationHandler;
    private Vector3 _unitPosition;
    #endregion

    #region ============= Getters & Setters =============
    public float nGold { get; protected set; } = 0;

    public float nExp { get; protected set; } = 0;

    public bool Idle => _state == UnitState.Idle;

    public bool Dealth => _state == UnitState.Dead;

    public int nTurns
    {
        get => _nTurns;
        set
        {
            if (_nTurns > value && nEffectTurns >= 0)
            {
                nEffectTurns--;
                if (nEffectTurns <= 0)
                {
                    foreach (var skill in _skills)
                    {
                        if (skill is WeaponAura aura)
                        {
                            aura.gameObject.SetActive(false);
                        }
                    }
                }
            }

            _nTurns = value;
            Debug.Log($"nTurns = {value}");

            if (_nTurnsText != null)
            {
                _nTurnsText.text = $"{value}";
            }
        }
    }

    public int nEffectTurns { set; private get; }
    public float DamageBuff { set; private get; } = 1;

    protected virtual float HP { get => UIUnit.HP.Value; set => UIUnit.HP.Value = value; }
    protected virtual float Mana { get => UIUnit.Mana.Value; set => UIUnit.Mana.Value = value; }
    protected virtual float Stamina { get => UIUnit.Stamina.Value; set => UIUnit.Stamina.Value = value; }

    #endregion

    #region ============= Methods for Inheriting =============

    public void ExecuteSkill(int iSkill, BattleUnitBase target, Action OnDone)
    {
        try
        {
            var manaRemain = Mana - _skills[iSkill].ManaConsumed;
            if (manaRemain >= 0)
            {
                _skills[iSkill].Execute(target, OnDone);
                Mana = manaRemain;
            }
            else
            {
                Debug.Log($"Not enough mana, you need some mana");
            }
        }
        catch (Exception e)
        {
            Debug.Log($"BattleUnitBase.ExecuteSkill - {e.Message}");
        }
    }

    public abstract bool Control();

    public abstract UnitStat Stat { get; set; }

    protected abstract UIUnit UIUnit { get; }

    protected abstract Vector3 UnitAttackPosition { get; }

    #endregion

    #region ============= For Devired Class =============

    protected virtual void _InitializeUIUnit()
    {
        UIUnit.HP.MaxValue = Stat.HPMax;
        UIUnit.Mana.MaxValue = Stat.ManaMax;
        UIUnit.Stamina.MaxValue = Stat.StaminaMax;

        UIUnit.HP.Value = Stat.HPMax;
        UIUnit.Mana.Value = 0;
        UIUnit.Stamina.Value = Stat.StaminaMax;
    }

    #endregion

    #region ============= public Methods =============

    public void DoSkill(BattleUnitBase target, float bonusFactor)
    {

    }

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

            var damage = Stat.Attack * bonusFactor;
            damage *= (nEffectTurns > 0) ? DamageBuff : 1;
            target.TakeHit(damage);

            Debug.Log($"[Attack] Damage = {damage}");
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

    public virtual void IncreaseGold(float bonusFactor) => nGold += bonusFactor;

    public virtual void IncreaseExp(float bonusFactor) => nExp += bonusFactor;

    public void ConsumeStamina(float value)
    {
        if (Stamina > 0)
        {
            Stamina -= value * Stat.StaminaConsumeWeight * Stat.Level;
        }
        else
        {
            _state = UnitState.Dead;
        }
    }

    public void RestoreHP(float bonusFactor) => HP = Mathf.Min(HP + Stat.HPRegen * bonusFactor, Stat.HPMax);

    public void RestoreMana(float bonusFactor) => Mana = Mathf.Min(Mana + Stat.ManaRegen * bonusFactor, Stat.ManaMax);

    public void RestoreStamina(float bonusFactor) => Stamina = Mathf.Min(Stamina + Stat.StaminaRegen * bonusFactor, Stat.StaminaMax);

    #endregion

    #region ============= Unity Methods =============

    private void Start()
    {
        _InitializeUIUnit();

        Stamina = Stat.StaminaMax;

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
