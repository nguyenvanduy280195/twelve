using System.Collections;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(MoveTo))]
public class ExitSnackbar : MonoBehaviour
{
    [SerializeField] private float _snackBarDuration = 5f;
    [SerializeField] private float _destinationY = 10f;

    private Vector3 _positionFirst;

    private Vector3 _PositionSecond => new Vector3(_positionFirst.x, _destinationY - 0.5f * _GetReferenceResolution().y, _positionFirst.z);

    private MoveTo _moveTo;

    private bool _snackbarShowing = false;

    private int _escapeCount = 0;

    private Coroutine _waitingForExit;

    private Vector2 _GetReferenceResolution()
    {
        var canvasScaler = GetComponentInParent<CanvasScaler>();
        if (canvasScaler != null)
        {
            return canvasScaler.referenceResolution;
        }

        return Vector2.zero;
    }

    private void Start()
    {
        _moveTo = GetComponent<MoveTo>();
        _positionFirst = transform.localPosition;
        _positionFirst.y -= 0.5f * _GetReferenceResolution().y;

        StartCoroutine(_WaitForEscapeKey());
        StartCoroutine(_WaitForShowingSnackBar());
    }

    private IEnumerator _WaitForEscapeKey()
    {
        while (true)
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                _escapeCount++;
            }

            yield return null;
        }
    }

    private IEnumerator _WaitForShowingSnackBar()
    {
        while (true)
        {
            yield return new WaitUntil(() => _escapeCount >= 1 && !_snackbarShowing);
            _snackbarShowing = true;
            _ShowSnackbar();
        }
    }

    private void _ShowSnackbar()
    {
        _moveTo.To = _PositionSecond;

        _waitingForExit = StartCoroutine(_WaitForExit());
        StartCoroutine(_HideSnackbar());
    }

    private IEnumerator _WaitForExit()
    {
        yield return new WaitUntil(() => _escapeCount >= 2);
        Application.Quit();
    }

    private IEnumerator _HideSnackbar()
    {
        yield return new WaitForSeconds(_snackBarDuration);

        _moveTo.To = _positionFirst;
        _escapeCount = 0;
        _snackbarShowing = false;
        StopCoroutine(_waitingForExit);
    }
}
