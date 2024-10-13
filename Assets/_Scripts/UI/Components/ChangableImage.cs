using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// For implementing rotating action
public class ChangableImage : MonoBehaviour
{
    [SerializeField] private float _rotationSpeed;
    [SerializeField] private float _rotationAngle;
    [SerializeField] private Sprite _openRawImage;
    [SerializeField] private Sprite _closeRawImage;

    private bool _rotating = false;

    private bool _openning = false;
    private Image _image;

    private void Start()
    {

        _image = GetComponent<Image>();
        _image.sprite = _openRawImage;
    }


    public void Rotate()
    {
        if (!_rotating)
        {
            _rotating = true;

            if (!_openning)
            {
                _openning = true;
                StartCoroutine(_RotateBy(angle: _rotationAngle,
                                        beforeRotate: () => _image.sprite = _closeRawImage,
                                        afterRotate: null));

            }
            else
            {
                _openning = false;
                StartCoroutine(_RotateBy(angle: -_rotationAngle,
                                        beforeRotate: null,
                                        afterRotate: () => _image.sprite = _openRawImage));
            }
        }
    }

    public void Open()
    {
        if (!_rotating)
        {
            _rotating = true;
            StartCoroutine(_RotateBy(angle: _rotationAngle,
                                                beforeRotate: () => _image.sprite = _closeRawImage,
                                                afterRotate: null));
        }
    }

    public void Close()
    {
        if (_rotating)
        {
            _rotating = true;
            StartCoroutine(_RotateBy(angle: -_rotationAngle,
                                                    beforeRotate: null,
                                                    afterRotate: () => _image.sprite = _openRawImage));
        }
    }

    private IEnumerator _RotateBy(float angle, Action beforeRotate, Action afterRotate)
    {
        beforeRotate?.Invoke();

        float destinationAngle = _image.rectTransform.eulerAngles.z + angle;
        if (destinationAngle >= 360) destinationAngle -= 360f;
        if (destinationAngle < 0) destinationAngle += 360f;

        while (Mathf.Abs(_image.rectTransform.eulerAngles.z - destinationAngle) > _rotationSpeed + 2f)
        {
            //Debug.Log($"{Mathf.Abs(_image.rectTransform.eulerAngles.z - destinationAngle)}");

            var delta = Time.deltaTime * angle * _rotationSpeed;
            _image.rectTransform.Rotate(new Vector3(0, 0, delta));
            yield return null;
        }

        _image.rectTransform.eulerAngles = new Vector3(0, 0, destinationAngle);

        _rotating = false;

        afterRotate?.Invoke();
    }
}
