using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Assertions;

public class Player : MonoBehaviour
{
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

    private Vector3 _playerPosition = Vector3.zero;


    // =================================================================================

    public bool Idle => _state == PlayerState.Idle;

    public bool Dealth => _state == PlayerState.Death;

    public float HP => _healthBar.Value;

    public float Mana
    {
        get => _manaBar.Value;
        set => _manaBar.Value = value;
    }

    public float Stamina => _staminaBar.Value;

    private PlayerState State
    {
        set
        {
            _state = value;
            _animator.SetInteger(ANIMATOR_NAME, (int)_state);
        }
    }

    public int nTurns
    {
        get => _nTurns;
        set
        {
            _nTurns = value;
            _nTurnsText.text = $"{value}";
        }
    }

    private int _nTurns;

    [NonSerialized]
    private const string ANIMATOR_NAME = "AnimationState";

    [NonSerialized]
    private Animator _animator;

    private PlayerState _state = PlayerState.Idle;

    private bool InAttackingWave = false;

    private IDictionary<PlayerState, List<Action>> _actions;

    private void Awake()
    {
        Assert.IsNotNull(_healthBar, "Please assign '_healthBar'");
        Assert.IsNotNull(_manaBar, "Please assign '_manaBar'");
        Assert.IsNotNull(_staminaBar, "Please assign '_staminaBar'");
        Assert.IsNotNull(_attackPosition, "Please assign '_attackPosition'");
        Assert.IsNotNull(_nTurnsText, "Please assign '_nTurnsText'");
    }

    private void Start()
    {
        _healthBar.MaxValue = maxHP;
        _manaBar.MaxValue = maxMana;
        _manaBar.Value = 0f;
        _staminaBar.MaxValue = maxStamina;
        _playerPosition = transform.position;
        _animator = GetComponent<Animator>();

        _actions = new Dictionary<PlayerState, List<Action>>();
        _actions[PlayerState.Idle] = new List<Action>();
        _actions[PlayerState.Attack] = new List<Action>();
        _actions[PlayerState.TakeHit] = new List<Action>();
        _actions[PlayerState.Run] = new List<Action>();
        _actions[PlayerState.Ulti] = new List<Action>();

        State = PlayerState.Idle;
    }

    private void Update()
    {
        if (_state == PlayerState.Run)
        {
            if (InAttackingWave)
            {
                transform.position = Vector2.MoveTowards(transform.position, _attackPosition.position, _moveDuration);
                var delta = transform.position - _attackPosition.position;
                if (delta.sqrMagnitude < 0.001f)
                {
                    OnAnimationEnd(PlayerState.Run);
                }
            }
            else
            {
                transform.position = Vector2.MoveTowards(transform.position, _playerPosition, _moveDuration);
                var delta = transform.position - _playerPosition;
                if (delta.sqrMagnitude < 0.001f)
                {
                    OnAnimationEnd(PlayerState.Run);
                }
            }
        }
        else if (_state == PlayerState.Attack)
        {
            if (_actions[PlayerState.Attack].Count <= 0)
            {
                _actions[PlayerState.Attack].Add(() =>
                {
                    FlipX();
                    State = PlayerState.Run;
                    InAttackingWave = false;

                    _actions[PlayerState.Run].Add(() =>
                    {
                        FlipX();
                        State = PlayerState.Idle;
                    });
                });
            }
        }
        else if (_state == PlayerState.TakeHit)
        {
            if (_actions[PlayerState.TakeHit].Count <= 0)
            {
                State = PlayerState.Idle;
            }
        }
    }

    private void FixedUpdate()
    {
        if (HP <= 0)
        {
            State = PlayerState.Death;
        }
    }

    public void DoSkill(Player target, float bonusFactor)
    {
        // only once
        if (_state == PlayerState.Idle)
        {
            State = PlayerState.Run;
            InAttackingWave = true;

            _actions[PlayerState.Run].Add(() => State = PlayerState.Ulti);
        }

        for (int i = 0; i < 10; i++)
        {
            _actions[PlayerState.Ulti].Add(() => target.TakeHit(attack * bonusFactor));
        }

        _actions[PlayerState.Ulti].Add(() =>
        {
            FlipX();
            Mana = 0;
            State = PlayerState.Run;
            InAttackingWave = false;
        });

        _actions[PlayerState.Run].Add(() =>
        {
            FlipX();
            State = PlayerState.Idle;
        });
    }

    public void Attack(Player target, float bonusFactor)
    {
        // only once
        if (_state == PlayerState.Idle)
        {
            State = PlayerState.Run;
            InAttackingWave = true;

            _actions[PlayerState.Run].Add(() => State = PlayerState.Attack);
        }

        _actions[PlayerState.Attack].Add(() =>
        {
            if (_state == PlayerState.Attack)
            {
                State = PlayerState.Attack;
            }
            _animator.SetBool("again", true);
        });

        _actions[PlayerState.Attack].Add(() => target.TakeHit(attack * bonusFactor));

        _actions[PlayerState.Attack].Add(() => _animator.SetBool("again", false));

    }

    public void TakeHit(float damage)
    {
        if (_state == PlayerState.Idle)
        {
            State = PlayerState.TakeHit;
        }

        _actions[PlayerState.TakeHit].Add(() =>
        {
            _healthBar.Value -= damage;
            State = PlayerState.TakeHit;
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

    public void OnAnimationEnd(PlayerState state)
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

    public enum PlayerState
    {
        Idle = 4,
        Run = 6,
        Attack = 0,
        TakeHit = 7,
        Death = 2,
        Ulti = 8,
    }

}
