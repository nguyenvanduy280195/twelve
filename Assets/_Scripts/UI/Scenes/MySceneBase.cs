using System;
using UnityEngine;

public class MySceneBase : MonoBehaviour
{
    public static event Action OnCreate;

    [SerializeField] private GameObject _sceneTransition;

    private void Awake()
    {
        _sceneTransition?.SetActive(true);

        OnCreate?.Invoke();
    }
}
