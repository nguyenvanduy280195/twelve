using UnityEngine;

public class MoveTo : UIAction
{
    private void Start() => To = _rectTransform.localPosition;
    protected override bool _Predicate => (_rectTransform.localPosition - To).magnitude > Mathf.Epsilon;
    protected override void _ActionIfPredicateTrue() => _rectTransform.localPosition = Vector2.MoveTowards(_rectTransform.localPosition, To, Speed);
}
