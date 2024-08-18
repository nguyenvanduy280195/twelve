using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveTo : MonoBehaviour
{
    [SerializeField]
    private float Speed;

    public Vector2 To { private get; set; }

    private RectTransform _rectTransform;

    private void Start()
    {
        _rectTransform = GetComponent<RectTransform>();
        To = _rectTransform.position;
    }

    private void Update()
    {
        _rectTransform.position = Vector2.MoveTowards(transform.position, To, Speed * Time.deltaTime);
    }
}
