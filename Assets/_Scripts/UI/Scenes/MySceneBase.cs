using System;
using UnityEngine;

public class MySceneBase : MonoBehaviour
{
    public static event Action OnCreate;

    private void Awake() => OnCreate?.Invoke();
}
