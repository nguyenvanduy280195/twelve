using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScaleTo : MonoBehaviour
{
    [SerializeField]
    private float Speed;

    public Vector3 To { set; private get; }

    public Action<GameObject> OnDone { set; private get; }

    public bool ActiveOnDoneOnce { set; private get; }

    private RectTransform _rectTransform;

    private void Start()
    {
        _rectTransform = GetComponent<RectTransform>();
        To = _rectTransform.localScale;
    }

    private void Update()
    {
        var delta = _rectTransform.localScale - To;
        if (delta.magnitude > Mathf.Epsilon)
        {
            _rectTransform.localScale = Vector2.MoveTowards(transform.localScale, To, Speed * Time.deltaTime);
        }
        else if (ActiveOnDoneOnce)
        {
            ActiveOnDoneOnce = false;
            OnDone?.Invoke(gameObject);
        }
    }
}
