using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingTwoPoints : EnemyMovingHandler
{
    [SerializeField]
    private float Speed = 1f;

    [SerializeField]
    private Transform _firstPoint;

    [SerializeField]
    private Transform _secondPoint;

    private Vector3 _destination;
    private UnitAnimationHandler _handler;

    private void Start()
    {
        _destination = _firstPoint.position;

        _handler = GetComponent<UnitAnimationHandler>();
    }

    private void Update()
    {
        _ChooseDestination();
        _ChooseWalkAnimation();
        _Move();
    }

    private void _ChooseDestination()
    {
        var delta = transform.position - _destination;
        if (delta.magnitude < Mathf.Epsilon)
        {
            if (_destination != _firstPoint.position)
            {
                _destination = _firstPoint.position;
            }
            else
            {
                _destination = _secondPoint.position;
            }
        }
    }

    private void _ChooseWalkAnimation()
    {
        var delta = _destination - transform.position;
        _handler.RunWalkAnimation(delta.x, delta.y);
    }

    private void _Move() => transform.position = Vector2.MoveTowards(transform.position, _destination, Speed * Time.deltaTime);
}