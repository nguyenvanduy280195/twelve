using System;
using System.Collections;
using UnityEngine;

[RequireComponent(typeof(MovingCamera))]
public class ObservingPlayer : MonoBehaviour
{
    [SerializeField] private float _timeBreak = 0.5f;
    [SerializeField] private float _spaceX;
    [SerializeField] private float _spaceY;
    [SerializeField] private Transform[] _cameraPositions;

    private MovingCamera _movingCamera;

    private void Start()
    {
        _movingCamera = GetComponent<MovingCamera>();

        StartCoroutine(_ObservePlayer());
    }

    private IEnumerator _ObservePlayer()
    {
        yield return new WaitUntil(() => ChoosingLevelUnitManager.Instance.GetPlayer() != null);
        while (true)
        {
            yield return new WaitForSeconds(_timeBreak);

            var currentCameraposition = _movingCamera.transform.position;
            if (!_IsPlayerXInCameraSpace(currentCameraposition) || !_IsPlayerYInCameraSpace(currentCameraposition))
            {
                try
                {
                    _movingCamera.Target = FindPlayer();
                }
                catch (NullReferenceException e)
                {
                    Debug.Log($"Moving Camera - {e.Message}");
                }
            }

            yield return null;
        }
    }

    private Transform FindPlayer()
    {
        foreach (var camera in _cameraPositions)
        {
            var predicateX = _IsPlayerXInCameraSpace(camera.position);
            var predicateY = _IsPlayerYInCameraSpace(camera.position);
            if (predicateX && predicateY)
            {
                return camera;
            }
        }

        return null;
    }
    private Transform _PlayerTransform => ChoosingLevelUnitManager.Instance.GetPlayer().transform;
    private bool _IsPlayerXInCameraSpace(Vector3 cameraPosition) => cameraPosition.x - _spaceX <= _PlayerTransform.position.x && _PlayerTransform.position.x < cameraPosition.x + _spaceX;
    private bool _IsPlayerYInCameraSpace(Vector3 cameraPosition) => cameraPosition.y - _spaceY <= _PlayerTransform.position.y && _PlayerTransform.position.y < cameraPosition.y + _spaceY;
}
