using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(Rigidbody2D))]
public class MovingPlayerByJoystick : MonoBehaviour
{
    [SerializeField]
    private Joystick _joystick;

    [SerializeField]
    private float _moveSpeed;

    private Animator _animator;
    private Rigidbody2D _rigidbody;
    private SpriteRenderer _spriteRenderer;



    // Start is called before the first frame update
    void Start()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _animator = GetComponent<Animator>();
        _rigidbody = GetComponent<Rigidbody2D>();
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

        _spriteRenderer.flipX = x < 0;

        if(Mathf.Abs(x) < Mathf.Epsilon && Mathf.Abs(y) < Mathf.Epsilon)
        {
            _animator.SetBool("Moving", false);
        }
        else
        {
            _animator.SetBool("Moving", true);
        }
    }


}
