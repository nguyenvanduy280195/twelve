using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(Rigidbody2D))]
public class MovingPlayer : MonoBehaviour
{
    public float MovementForce = 100f;
    public float MovementDuration = 0.1f;


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
        if (Input.GetKey(KeyCode.UpArrow))
        {
            MoveY(1);
        }
        else if (Input.GetKey(KeyCode.DownArrow))
        {
            MoveY(-1);
        }
        else if (Input.GetKey(KeyCode.LeftArrow))
        {
            MoveX(-1);
        }
        else if (Input.GetKey(KeyCode.RightArrow))
        {
            MoveX(1);
        }
        else
        {
            _animator.SetBool("Moving", false);
            _rigidbody.velocity = Vector2.zero;
        }
    }

    private void MoveX(float x)
    {
        Face(x);
        _rigidbody.velocity = new Vector2(x * MovementForce, 0);
        _animator.SetBool("Moving", true);
    }

    private void MoveY(float y)
    {
        _rigidbody.velocity = new Vector2(0, y * MovementForce);
        _animator.SetBool("Moving", true);
    }

    private void Face(float x) => _spriteRenderer.flipX = x < 0;

}
