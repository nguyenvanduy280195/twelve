using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using IEnumerator = System.Collections.IEnumerator;

[RequireComponent(typeof(IAttacker))]
public abstract class BattleUnitBase : MonoBehaviour
{
    private readonly float MY_EPSILON = 0.0001f;

    #region============= Properties =============
    [SerializeField] private float _moveSpeed = 1f;
    [SerializeField] private float _bulletSpeed = 1f;
    [SerializeField] private TextMeshProUGUI _nTurnsText;
    [SerializeField] protected SkillBase[] _skills;
    #endregion

    #region ============= Fields =============

    private int _nTurns;
    private UnitAnimationHandler _animationHandler;
    private Vector3 _unitPosition;
    private Bubbles _bubbles;
    private IAttacker _attacker;

    #endregion

    #region ============= Getters & Setters =============
    [NonSerialized] public IDictionary<UnitState, Queue<Action>> Actions;
    public UnitState State = UnitState.Idle;
    public float nGold { get; protected set; } = 0;
    public float nExp { get; protected set; } = 0;
    public bool Idle => State == UnitState.Idle;
    public bool Dealth => State == UnitState.Dead;
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
            Debug.Log($"{gameObject.tag}.nTurns = {value}");

            if (_nTurnsText != null)
            {
                _nTurnsText.text = $"{value}";
            }
        }
    }
    public int nEffectTurns;
    public float DamageBuff { set; private get; } = 1f;
    public bool Initialized { get; protected set; } = false;
    protected virtual float HP { get => UIUnit.HP.Value; set => UIUnit.HP.Value = value; }
    protected virtual float Mana { get => UIUnit.Mana.Value; set => UIUnit.Mana.Value = value; }
    public virtual float Stamina { get => UIUnit.Stamina.Value; set => UIUnit.Stamina.Value = value; }
    #endregion

    #region ============= Methods for Inheriting =============

    public int GetSkillIndex(SkillBase skill) => _skills.ToList().FindIndex(0, it => it == skill);

    public abstract IEnumerator ControlCoroutine();

    public abstract UnitData Stat { get; set; }

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

    public void ExecuteSkill(int iSkill, BattleUnitBase target, Action onSucceeded, Action onFailed, Action onExecuted, Action onDone)
    {
        try
        {
            var skill = _skills[iSkill];
            var manaRemain = Mana - skill.ManaConsumed;
            if (manaRemain >= 0)
            {
                _bubbles?.ShowMana(-skill.ManaConsumed);
                skill.Execute(target, onExecuted, onDone);
                Mana = manaRemain;
                onSucceeded?.Invoke();
            }
            else
            {
                AlertSnackbar.Instance?.SetText("Not enough mana, you need some mana")
                                      ?.Show();
            }
        }
        catch (Exception e)
        {
            Debug.Log($"BattleUnitBase.ExecuteSkill - {e.Message}");
        }
        finally
        {
            onFailed?.Invoke();
        }
    }

    public void Attack(BattleUnitBase target, float bonusFactor) => _attacker.SetDamage(Stat.Attack * bonusFactor)
                                                                            .SetTargetUnit(target)
                                                                            .SetMoveSpeed(_moveSpeed)
                                                                            .SetBulletSpeed(_bulletSpeed)
                                                                            .SetAttackPosition(UnitAttackPosition)
                                                                            .Attack();

    public void TakeHit(float damage)
    {
        _bubbles?.ShowHP(-damage);

        HP -= damage;
        if (HP > 0)
        {
            State = UnitState.Hurt;
            _animationHandler.RunHurtAnimation();
        }
        else
        {
            State = UnitState.Dead;
            _animationHandler.RunDeadAnimation();
        }
    }
    public virtual void IncreaseGold(float bonusFactor) => nGold += bonusFactor;
    public virtual void IncreaseExp(float bonusFactor) => nExp += bonusFactor;
    public void ConsumeStamina()
    {
        if (Stamina > 0)
        {
            Stamina -= Stat.StaminaConsumeWeight * Time.deltaTime;
        }
        else
        {
            Debug.Log($"[BattleUnitBase] {gameObject.tag} is Dead");
            State = UnitState.Dead;
        }
    }
    public void RestoreHPByFormula(float bonusFactor)
    {
        var delta = Stat.HPRegen * bonusFactor;
        _bubbles?.ShowHP(delta);
        HP = Mathf.Min(HP + delta, Stat.HPMax);
    }
    public void RestoreManaByFormula(float bonusFactor) => RestoreMana(Stat.ManaRegen * bonusFactor);
    public void RestoreMana(float value)
    {
        _bubbles?.ShowMana(value);
        Mana = Mathf.Min(Mana + value, Stat.ManaMax);
    }
    public void RestoreStaminaByFormula(float bonusFactor)
    {
        var delta = Stat.StaminaRegen * bonusFactor;
        _bubbles?.ShowStamina(delta);
        Stamina = Mathf.Min(Stamina + delta, Stat.StaminaMax);
    }

    #endregion

    #region ============= Unity Methods =============

    private void Start()
    {
        _InitializeUIUnit();

        _attacker = GetComponent<IAttacker>();

        _unitPosition = transform.position;

        _bubbles = transform.parent.GetComponentInChildren<Bubbles>();
        _animationHandler = GetComponent<UnitAnimationHandler>();

        Actions = new Dictionary<UnitState, Queue<Action>>();
        Actions[UnitState.MoveToTargetPosition] = new();
        Actions[UnitState.Attack] = new();
        Actions[UnitState.MoveToStartPosition] = new();
        Actions[UnitState.Hurt] = new();
        Actions[UnitState.Dead] = new();

        StartCoroutine(_StartUnit());
    }

    private IEnumerator _StartUnit()
    {
        while (true)
        {
            yield return _SetState(State);
        }
    }
    #endregion

    #region ============= Support Methods =============


    private IEnumerator _SetState(UnitState state)
    {
        switch (state)
        {
            case UnitState.Idle:
                break;
            case UnitState.MoveToTargetPosition:
                yield return _HandleMoveToTargetPositionState();
                break;
            case UnitState.MoveToStartPosition:
                yield return _HandleMoveToStartPositionState();
                break;
            case UnitState.Attack:
                yield return _HandleAttackState();
                break;
            case UnitState.Hurt:
                yield return _HandleHurtState();
                break;
            case UnitState.Dead:
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(State), State, null);
        }
    }

    private IEnumerator _HandleMoveToTargetPositionState()
    {
        yield return _MoveTo(transform, UnitAttackPosition);
        Actions?[UnitState.MoveToTargetPosition]?.Dequeue()?.Invoke();
    }

    private IEnumerator _HandleMoveToStartPositionState()
    {
        yield return _MoveTo(transform, _unitPosition);
        Actions?[UnitState.MoveToStartPosition]?.Dequeue()?.Invoke();
    }

    private IEnumerator _MoveTo(Transform from, Vector3 toAsPosition)
    {
        while ((from.position - toAsPosition).magnitude > MY_EPSILON)
        {
            transform.position = Vector2.MoveTowards(from.position, toAsPosition, _moveSpeed * Time.deltaTime);
            yield return null;
        }
    }

    private IEnumerator _HandleAttackState()
    {
        yield return new WaitUntil(() => !_animationHandler.CurrentStateLocked);

        if (Actions[UnitState.Attack].Count > 0)
        {
            Actions?[UnitState.Attack]?.Dequeue()?.Invoke();
        }
    }

    private IEnumerator _HandleHurtState()
    {
        yield return new WaitUntil(() => !_animationHandler.CurrentStateLocked);

        if (Actions[UnitState.Hurt].Count <= 0)
        {
            State = UnitState.Idle;
            _animationHandler.RunIdleAnimation(transform.position, UnitAttackPosition);
        }
        else
        {
            Actions?[UnitState.Hurt]?.Dequeue()?.Invoke();
        }
    }

    #endregion
}

[Serializable]
public enum UnitState
{
    MoveToTargetPosition,
    MoveToStartPosition,
    Attack,
    Idle,
    Hurt,
    Dead,
}
