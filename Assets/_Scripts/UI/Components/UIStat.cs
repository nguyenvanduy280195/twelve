using System;
using TMPro;
using UnityEngine;

public abstract class UIStat<Type> : MonoBehaviour
{
    public Action<Type> OnContentChanged;

    [SerializeField] protected TextMeshProUGUI _content;

    public virtual Type Content
    {
        set
        {
            _content.text = ToContent(value);
            OnContentChanged?.Invoke(value);
        }
        get => ToType(_content.text);
    }

    protected abstract string ToContent(Type value);

    protected abstract Type ToType(string value);
}