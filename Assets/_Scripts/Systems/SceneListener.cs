using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneListener : MonoBehaviour
{
    #region Events

    public static event Action OnCreate;

    #endregion

    private void Awake() => OnCreate?.Invoke();
}
