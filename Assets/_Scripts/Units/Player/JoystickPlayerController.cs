using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class JoystickPlayerController : MonoBehaviour
{
    [SerializeField] private float _moveSpeed;

    private JoystickManager _joystickManager;
    private Rigidbody2D _rigidbody;
    private UnitAnimationHandler _unitAnimationHandler;

    // Start is called before the first frame update
    void Start()
    {
        _joystickManager = JoystickManager.Instance;
        _rigidbody = GetComponent<Rigidbody2D>();
        _unitAnimationHandler = GetComponent<UnitAnimationHandler>();
    }

    // Update is called once per frame
    void Update()
    {
        if (GameManager.Instance?.IsPausing() ?? false)
        {
            _rigidbody.velocity = Vector3.zero;
            return;
        }

        var joystick = _joystickManager?.Joystick;
        if (joystick != null)
        {
            float x = joystick.Horizontal * _moveSpeed;
            float y = joystick.Vertical * _moveSpeed;
            _rigidbody.velocity = new Vector2(x, y);

            _unitAnimationHandler.RunWalkAnimation(x, y);
        }
    }

    private void OnDisable()
    {
        _rigidbody.velocity = Vector3.zero;
    }
}
