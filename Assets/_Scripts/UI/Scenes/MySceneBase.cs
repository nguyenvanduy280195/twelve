using System;
using UnityEngine;

public class MySceneBase : MonoBehaviour
{
    [SerializeField] private GameObject _sceneTransition;

    protected virtual void Awake()
    {
        _sceneTransition?.SetActive(true);
    }
}
