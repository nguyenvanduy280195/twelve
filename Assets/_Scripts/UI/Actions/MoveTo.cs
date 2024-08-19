using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveTo : UIAction
{
    private void Start() => To = _rectTransform.position;
    protected override bool Predicate => (_rectTransform.position - To).magnitude > Mathf.Epsilon;
    protected override void _ActionIfPredicateTrue() => _rectTransform.position = Vector2.MoveTowards(transform.position, To, Speed * Time.deltaTime);
}
