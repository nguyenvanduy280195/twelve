using System.Collections;
using TMPro;
using UnityEngine;

public class Bubbles : MonoBehaviour
{
    [SerializeField] private TextMeshPro _hpBubblePrefab;
    [SerializeField] private TextMeshPro _manaBubblePrefab;
    [SerializeField] private TextMeshPro _staminaBubblePrefab;

    [SerializeField] private Transform _destination;
    [SerializeField] private float _duration;
    [SerializeField] private float _delay;

    public void ShowHP(float value) => StartCoroutine(_ShowBubble(_hpBubblePrefab, value));

    public void ShowMana(float value) => StartCoroutine(_ShowBubble(_manaBubblePrefab, value));

    public void ShowStamina(float value) => StartCoroutine(_ShowBubble(_staminaBubblePrefab, value));


    private bool nexting = true;

    private IEnumerator _ShowBubble(TextMeshPro bubblePrefab, float value)
    {

        yield return new WaitUntil(() => nexting);

        nexting = false;

        yield return new WaitForSeconds(_delay);

        nexting = true;

        var bubble = Instantiate(bubblePrefab, transform);
        bubble.text = string.Format("{0:#0.00}", value);
        yield return _MoveBubble(bubble);
    }

    private IEnumerator _MoveBubble(TextMeshPro bubble)
    {
        while (Vector2.Distance(bubble.transform.position, _destination.position) > float.Epsilon)
        {
            bubble.transform.position = Vector2.MoveTowards(bubble.transform.position, _destination.position, _duration * Time.deltaTime);
            yield return null;
        }
        Destroy(bubble);
    }

}
