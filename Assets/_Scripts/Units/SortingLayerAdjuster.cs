using UnityEngine;

public class SortingLayerAdjuster : MonoBehaviour
{
    [SerializeField] private bool _dynamic = false;

    private SpriteRenderer _spriteRenderer;

    private void Start()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _spriteRenderer.sortingOrder = (int)(-transform.position.y * _spriteRenderer.sprite.pixelsPerUnit);
    }

    private void Update()
    {
        if (_dynamic)
        {
            _spriteRenderer.sortingOrder = (int)(-transform.position.y * _spriteRenderer.sprite.pixelsPerUnit);
        }
    }
}
