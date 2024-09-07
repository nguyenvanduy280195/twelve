using System;
using UnityEngine;

public abstract class UIAction : MonoBehaviour
{
    [SerializeField] protected float Speed;

    public Vector3 To { set; protected get; }

    public Action<GameObject> OnDone { set; private get; }

    public bool ActiveOnDoneOnce { set; private get; }

    protected RectTransform _rectTransform;

    private void Awake()
    {
        _rectTransform = GetComponent<RectTransform>();
    }

    private void Update()
    {
        if (_Predicate)
        {
            _ActionIfPredicateTrue();
        }
        else if (ActiveOnDoneOnce)
        {
            ActiveOnDoneOnce = false;
            OnDone?.Invoke(gameObject);
        }
    }

    protected abstract bool _Predicate { get; }

    protected abstract void _ActionIfPredicateTrue();
}
