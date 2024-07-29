using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class JoystickPlayerController : MonoBehaviour
{
    [SerializeField]
    private Joystick _joystick;

    [SerializeField]
    private float _moveSpeed;

    private Rigidbody2D _rigidbody;
    private UnitAnimationHandler _unitAnimationHandler;

    // Start is called before the first frame update
    void Start()
    {
        _rigidbody = GetComponent<Rigidbody2D>();
        _unitAnimationHandler = GetComponent<UnitAnimationHandler>();
    }

    // Update is called once per frame
    void Update()
    {
        if (_joystick != null)
        {
            float x = _joystick.Horizontal * _moveSpeed;
            float y = _joystick.Vertical * _moveSpeed;
            _rigidbody.velocity = new Vector2(x, y);

            _unitAnimationHandler.RunWalkAnimation(x, y);
        }
    }
}
