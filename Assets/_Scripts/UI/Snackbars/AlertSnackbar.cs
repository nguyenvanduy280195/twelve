using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(MoveTo))]
public class AlertSnackbar : Singleton<AlertSnackbar>
{
    [SerializeField] private TextMeshProUGUI _text;
    [SerializeField] private float _snackBarDuration = 5f;
    [SerializeField] private float _destinationY = 10f;

    private Vector3 _positionFirst;
    private Vector3 _PositionSecond => new(_positionFirst.x, _destinationY - 0.5f * _ReferenceResolution.y, _positionFirst.z);
    private Vector2 _ReferenceResolution
    {
        get
        {
            var canvasScaler = GetComponentInParent<CanvasScaler>();
            if (canvasScaler != null)
            {
                return canvasScaler.referenceResolution;
            }

            return Vector2.zero;
        }
    }

    private MoveTo _moveTo;
    private bool _snackbarShowing = false;

    #region public methods

    public AlertSnackbar SetText(string value)
    {
        _text.text = value;
        return this;
    }

    public void Show()
    {
        if (_snackbarShowing)
        {
            return;
        }

        _snackbarShowing = true;
        _moveTo.To = _PositionSecond;
        StartCoroutine(_Hide());
    }

    #endregion

    #region Unity methods

    private void Start()
    {
        _moveTo = GetComponent<MoveTo>();
        _positionFirst = transform.localPosition;
    }

    #endregion

    #region Support methods

    private IEnumerator _Hide()
    {
        yield return new WaitForSeconds(_snackBarDuration);

        _moveTo.To = _positionFirst;
        _snackbarShowing = false;
    }

    #endregion
}