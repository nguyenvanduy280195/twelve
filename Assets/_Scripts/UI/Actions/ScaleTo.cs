using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScaleTo : UIAction
{
    private void Start() => To = _rectTransform.localScale;
    protected override bool Predicate => (_rectTransform.localScale - To).magnitude > Mathf.Epsilon;
    protected override void _ActionIfPredicateTrue() => _rectTransform.localScale = Vector2.MoveTowards(transform.localScale, To, Speed * Time.deltaTime);
}