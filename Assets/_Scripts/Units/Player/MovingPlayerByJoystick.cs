using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(Rigidbody2D))]
public class MovingPlayerByJoystick : MonoBehaviour
{
    [SerializeField]
    private Joystick _joystick;

    [SerializeField]
    private float _moveSpeed;

    private Animator _animator;
    private Rigidbody2D _rigidbody;
    private State _state = State.Player_idle_facedown;

    private const string Moving_State = "State";

    // Start is called before the first frame update
    void Start()
    {
        _animator = GetComponent<Animator>();
        _rigidbody = GetComponent<Rigidbody2D>();

        _animator.SetInteger(Moving_State, (int)_state);
    }

    // Update is called once per frame
    void Update()
    {
        //_rigidbody.velocity = Vector3.zero;
    }

    private void FixedUpdate()
    {
        float x = _joystick.Horizontal * _moveSpeed;
        float y = _joystick.Vertical * _moveSpeed;
        _rigidbody.velocity = new Vector2(x, y);
    }

    private void LateUpdate()
    {
        var velocity = _joystick.Direction;
        var absx = Mathf.Abs(velocity.x);
        var absy = Mathf.Abs(velocity.y);

        if (absx > absy)
        {
            if (velocity.x >= 0)
            {
                _state = State.Player_walk_faceright;
            }
            else
            {
                _state = State.Player_walk_faceleft;
            }
        }
        else if (absx < absy)
        {
            if (velocity.y >= 0)
            {
                _state = State.Player_walk_faceup;
            }
            else
            {
                _state = State.Player_walk_facedown;
            }
        }
        else
        {
            if (velocity == Vector2.zero)
            {
                switch(_state)
                {
                    case State.Player_walk_facedown:
                    case State.Player_walk_faceleft:
                    case State.Player_walk_faceright:
                    case State.Player_walk_faceup:
                        _state--;
                        break;
                    default:
                        break;
                }
            }
        }
        _animator.SetInteger(Moving_State, (int)_state);
    }

    private enum State
    {
        Player_idle_facedown = 10,
        Player_walk_facedown = 11,
        Player_idle_faceleft = 20,
        Player_walk_faceleft = 21,
        Player_idle_faceright = 30,
        Player_walk_faceright = 31,
        Player_idle_faceup = 40,
        Player_walk_faceup = 41,
    }
}
