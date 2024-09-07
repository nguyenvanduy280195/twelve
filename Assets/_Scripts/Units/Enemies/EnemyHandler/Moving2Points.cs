using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Moving2Points : EnemyMovingHandler
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
        _InitStartPosition();

        _handler = GetComponent<UnitAnimationHandler>();
    }

    private void Update()
    {
        if (GameManager.Instance?.IsPausing() ?? false)
        {
            return;
        }

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

    // the start position will be the farest both positions
    private void _InitStartPosition()
    {
        _destination = _firstPoint.position;

        //TODO pending
        // var matchingBattleManager = MatchingBattleManager.Instance;
        // if (matchingBattleManager is not null)
        // {
        //     var playerPosition = matchingBattleManager.PlayerPositionBeforeBattle;
        //     var firstDistance = (playerPosition - _firstPoint.position).magnitude;
        //     var secondDistance = (playerPosition - _secondPoint.position).magnitude;
        //     if (firstDistance > secondDistance)
        //     {
        //         transform.position = _firstPoint.position;
        //         _destination = _secondPoint.position;
        //     }
        //     else
        //     {
        //         transform.position = _secondPoint.position;
        //         _destination = _firstPoint.position;
        //     }
        // }


    }
}
