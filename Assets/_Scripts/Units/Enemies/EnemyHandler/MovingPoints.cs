using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingPoints : EnemyMovingHandler
{
    [SerializeField] private float Speed;
    [SerializeField] private Transform[] _points;

    private int iDestination = 0;

    private UnitAnimationHandler _handler;

    private void Start()
    {
        _handler = GetComponent<UnitAnimationHandler>();
    }

    private void Update()
    {
        if (GameManager.Instance?.IsPausing() ?? false)
        {
            return;
        }

        var delta = _points[iDestination].position - transform.position;
        
        _ChooseDestination(delta);
        _ChooseWalkAnimation(delta);
        _Move();
    }

    private void _ChooseDestination(Vector2 delta)
    {
        if (delta.magnitude < Mathf.Epsilon)
        {
            iDestination++;

            if(iDestination >= _points.Length)
            {
                iDestination = 0;
            }
        }
    }

    private void _ChooseWalkAnimation(Vector2 delta) => _handler.RunWalkAnimation(delta.x, delta.y);

    private void _Move() => transform.position = Vector2.MoveTowards(transform.position, _points[iDestination].position, Speed * Time.deltaTime);
}
