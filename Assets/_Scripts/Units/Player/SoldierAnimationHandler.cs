using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class SoldierAnimationHandler : MonoBehaviour
{
    private static readonly int Idle_up = Animator.StringToHash("Soldier_idle_up");
    private static readonly int Idle_down = Animator.StringToHash("Soldier_idle_down");
    private static readonly int Idle_left = Animator.StringToHash("Soldier_idle_left");
    private static readonly int Idle_right = Animator.StringToHash("Soldier_idle_right");
    private static readonly int Walk_up = Animator.StringToHash("Soldier_walk_up");
    private static readonly int Walk_down = Animator.StringToHash("Soldier_walk_down");
    private static readonly int Walk_left = Animator.StringToHash("Soldier_walk_left");
    private static readonly int Walk_right = Animator.StringToHash("Soldier_walk_right");

    private int _state = Idle_down;

    private Animator _animator;
    private Rigidbody2D _rigidbody;

    private void Start()
    {
        _animator = GetComponent<Animator>();
        _rigidbody = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        _state = _GetState(_rigidbody.velocity.x, _rigidbody.velocity.y);
        _animator.CrossFade(_state, 0);
    }

    private int _GetState(float x, float y) => _GetState(x, y, _state);

    private int _GetState(float x, float y, int currentState)
    {
        var state = currentState;
        var absx = Mathf.Abs(x);
        var absy = Mathf.Abs(y);

        if (absx > absy)
        {
            if (x >= 0)
            {
                state = Walk_right;
            }
            else
            {
                state = Walk_left;
            }
        }
        else if (absx < absy)
        {
            if (y >= 0)
            {
                state = Walk_up;
            }
            else
            {
                state = Walk_down;
            }
        }
        else
        {
            if (state == Walk_down)
            {
                state = Idle_down;
            }
            else if (state == Walk_up)
            {
                state = Idle_up;
            }
            else if (state == Walk_left)
            {
                state = Idle_left;
            }
            else if (state == Walk_right)
            {
                state = Idle_right;
            }
        }

        return state;
    }
}
