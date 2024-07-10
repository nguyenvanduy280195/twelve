using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Assertions;

public abstract class BattleUnitBase : MonoBehaviour
{
    //============= Properties =============

    public int level;
    public float attack;
    public float maxHP;
    public float maxMana;
    public float maxStamina;

    [SerializeField]
    private Bar _healthBar;

    [SerializeField]
    [Tooltip("hp += bonusFactor * _regenHP")]
    private float _regenHP;

    [SerializeField]
    private Bar _manaBar;

    [SerializeField]
    [Tooltip("mana += bonusFactor * _regenMana")]
    private float _regenMana;

    [SerializeField]
    private Bar _staminaBar;

    [SerializeField]
    [Tooltip("stamina += bonusFactor * _regenStamina")]
    private float _regenStamina;

    [SerializeField]
    private Transform _attackPosition;

    [SerializeField]
    private float _moveDuration = 1f;

    [SerializeField]
    private TextMeshProUGUI _nTurnsText;

    [NonSerialized]
    public int nGold = 0;

    [NonSerialized]
    public int nExp = 0;

    [NonSerialized]
    public bool Pausing = true;


    //============= Constantss =============

    [NonSerialized]
    private const string ANIMATOR_NAME = "AnimationState";

    // ============= Fields =============

    [NonSerialized]
    private Animator _animator;

    private Vector3 _playerPosition = Vector3.zero;

    private UnitState _state = UnitState.Idle;

    private bool InAttackingWave = false;

    private IDictionary<UnitState, List<Action>> _actions;

    private UnitState _State
    {
        set
        {
            _state = value;
            _animator.SetInteger(ANIMATOR_NAME, (int)_state);
        }
    }

    private int _nTurns;

    //============= Getters & Setters =============
    public bool Idle => _state == UnitState.Idle;

    public bool Dealth => _state == UnitState.Death;

    public float HP => _healthBar.Value;

    public float Mana { get => _manaBar.Value; set => _manaBar.Value = value; }

    public float Stamina => _staminaBar.Value;

    public int nTurns
    {
        get => _nTurns;
        set
        {
            _nTurns = value;
            _nTurnsText.text = $"{value}";
        }
    }

    //============= Methods for Inheriting =============

    public abstract bool Control();


    //============= public Methods =============

    public void DoSkill(BattleUnitBase target, float bonusFactor)
    {
        // only once
        if (_state == UnitState.Idle)
        {
            _State = UnitState.Run;
            InAttackingWave = true;

            _actions[UnitState.Run].Add(() => _State = UnitState.Ulti);
        }

        for (int i = 0; i < 10; i++)
        {
            _actions[UnitState.Ulti].Add(() => target.TakeHit(attack * bonusFactor));
        }

        _actions[UnitState.Ulti].Add(() =>
        {
            FlipX();
            Mana = 0;
            _State = UnitState.Run;
            InAttackingWave = false;
        });

        _actions[UnitState.Run].Add(() =>
        {
            FlipX();
            _State = UnitState.Idle;
        });
    }

    public void Attack(BattleUnitBase target, float bonusFactor)
    {
        // only once
        if (_state == UnitState.Idle)
        {
            _State = UnitState.Run;
            InAttackingWave = true;

            _actions[UnitState.Run].Add(() => _State = UnitState.Attack);
        }

        _actions[UnitState.Attack].Add(() =>
        {
            if (_state == UnitState.Attack)
            {
                _State = UnitState.Attack;
            }
            _animator.SetBool("again", true);
        });

        _actions[UnitState.Attack].Add(() => target.TakeHit(attack * bonusFactor));

        _actions[UnitState.Attack].Add(() => _animator.SetBool("again", false));

    }

    public void TakeHit(float damage)
    {
        if (_state == UnitState.Idle)
        {
            _State = UnitState.TakeHit;
        }

        _actions[UnitState.TakeHit].Add(() =>
        {
            _healthBar.Value -= damage;
            _State = UnitState.TakeHit;
        });
    }

    public void ConsumeStamina(float value)
    {
        _staminaBar.Value -= value;
    }

    public void RestoreHP(float bonusFactor)
    {
        _healthBar.Value += _regenHP * bonusFactor;
    }

    public void RestoreMana(float bonusFactor)
    {
        _manaBar.Value += _regenMana * bonusFactor;
    }

    public void RestoreStamina(float bonusFactor)
    {
        _staminaBar.Value += _regenStamina * bonusFactor;
    }

    public void FlipX()
    {
        var spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.flipX = !spriteRenderer.flipX;
    }

    public void OnAnimationEnd(UnitState state)
    {
        if (_actions != null)
        {
            if (_actions[state].Count > 0)
            {
                var first = _actions[state][0];

                first();

                _actions[state].Remove(first);
            }
        }
    }

    //============= private Methods =============

    private void Start()
    {
        _healthBar.MaxValue = maxHP;
        _manaBar.MaxValue = maxMana;
        _manaBar.Value = 0f;
        _staminaBar.MaxValue = maxStamina;
        _playerPosition = transform.position;
        _animator = GetComponent<Animator>();

        _actions = new Dictionary<UnitState, List<Action>>();
        _actions[UnitState.Idle] = new List<Action>();
        _actions[UnitState.Attack] = new List<Action>();
        _actions[UnitState.TakeHit] = new List<Action>();
        _actions[UnitState.Run] = new List<Action>();
        _actions[UnitState.Ulti] = new List<Action>();

        _State = UnitState.Idle;
    }

    private void Update()
    {
        if (_state == UnitState.Run)
        {
            if (InAttackingWave)
            {
                transform.position = Vector2.MoveTowards(transform.position, _attackPosition.position, _moveDuration);
                var delta = transform.position - _attackPosition.position;
                if (delta.sqrMagnitude < 0.001f)
                {
                    OnAnimationEnd(UnitState.Run);
                }
            }
            else
            {
                transform.position = Vector2.MoveTowards(transform.position, _playerPosition, _moveDuration);
                var delta = transform.position - _playerPosition;
                if (delta.sqrMagnitude < 0.001f)
                {
                    OnAnimationEnd(UnitState.Run);
                }
            }
        }
        else if (_state == UnitState.Attack)
        {
            if (_actions[UnitState.Attack].Count <= 0)
            {
                _actions[UnitState.Attack].Add(() =>
                {
                    FlipX();
                    _State = UnitState.Run;
                    InAttackingWave = false;

                    _actions[UnitState.Run].Add(() =>
                    {
                        FlipX();
                        _State = UnitState.Idle;
                    });
                });
            }
        }
        else if (_state == UnitState.TakeHit)
        {
            if (_actions[UnitState.TakeHit].Count <= 0)
            {
                _State = UnitState.Idle;
            }
        }
    }

    private void FixedUpdate()
    {
        if (HP <= 0)
        {
            _State = UnitState.Death;
        }
    }

    public enum UnitState
    {
        Idle = 4,
        Run = 6,
        Attack = 0,
        TakeHit = 7,
        Death = 2,
        Ulti = 8,
    }
}

