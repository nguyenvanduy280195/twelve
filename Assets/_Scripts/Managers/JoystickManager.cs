using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JoystickManager : Singleton<JoystickManager>
{
    [SerializeField] private Joystick _joystick;

    public Joystick Joystick => _joystick;
}
