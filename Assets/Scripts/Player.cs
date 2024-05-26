using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class Player : MonoBehaviour
{
    public int level;
    public float attack;
    public float maxHP;
    public int nTurns;

    [SerializeField]
    private HealthBar _healthBar;

    [SerializeField]
    private Record _records;

    [SerializeField]
    private Transform _target;

    [SerializeField]
    private float _moveDuration = 1f;

    [NonSerialized]
    private string ANIMATOR_NAME = "AnimationState";

    private Vector3 _playerPosition = Vector3.zero;

    public float HP => _healthBar.HP;

    private Animator _animator;
    private PlayerState _state = PlayerState.Idle;
    private bool InAttackingWave = false;

    private void Awake()
    {
        _healthBar.MaxHP = maxHP;
        _animator = GetComponent<Animator>();
        _playerPosition = transform.position;

        State = PlayerState.Idle;

        Assert.IsNotNull(_target, "Please assign 'Target'");
    }

    private void Update()
    {
        if (_state == PlayerState.Idle)
        {
        }
        else if (_state == PlayerState.Run)
        {
            if (InAttackingWave)
            {
                transform.position = Vector2.MoveTowards(transform.position, _target.position, _moveDuration);
                var delta = transform.position - _target.position;
                if (delta.sqrMagnitude < 0.001f)
                {
                    State = PlayerState.Attack;
                }
            }
            else
            {
                transform.position = Vector2.MoveTowards(transform.position, _playerPosition, _moveDuration);
                var delta = transform.position - _playerPosition;
                if (delta.sqrMagnitude < 0.001f)
                {
                    var spriteRenderer = GetComponent<SpriteRenderer>();
                    spriteRenderer.flipX = !spriteRenderer.flipX;
                    State = PlayerState.Idle;
                }
            }
        }
        else if (_state == PlayerState.Attack)
        {
        }
    }

    private PlayerState State
    {
        set
        {
            _state = value;
            _animator.SetInteger(ANIMATOR_NAME, (int)_state);
        }
    }

    public void OnAttackAnimationEnd()
    {
        InAttackingWave = false;
        State = PlayerState.Run;

        var spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.flipX = !spriteRenderer.flipX;
    }

    public void SetScore(string tagScore, int score)
    {
        if (tagScore == "Attack")
        {
            _records.AttackScore += score;
        }
        else if (tagScore == "Exp")
        {
            _records.ExpScore += score;
        }
        else if (tagScore == "Gold")
        {
            _records.GoldScore += score;
        }
        else if (tagScore == "HP")
        {
            _records.HPScore += score;
        }
        else if (tagScore == "MP")
        {
            _records.MPScore += score;
        }
        else if (tagScore == "Stamina")
        {
            _records.StaminaScore += score;
        }
    }

    public void Attack(Player target, float bonusFactor)
    {
        target.TakeHit(attack * bonusFactor);

        State = PlayerState.Run;
        InAttackingWave = true;
    }

    public void TakeHit(float damage)
    {
        _healthBar.HP -= damage;
    }

    public void Restore(float bonusFactor)
    {
        _healthBar.HP += bonusFactor;
    }

    private enum PlayerState
    {
        Idle = 4,
        Run = 6,
        Attack = 0
    }
}
