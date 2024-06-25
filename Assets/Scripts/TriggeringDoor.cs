using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class TriggeringDoor : MonoBehaviour
{
    [SerializeField]
    private GameObject _player;

    [SerializeField]
    private Transform _newCameraPosition;

    private void Start()
    {
        Assert.IsNotNull(_player, "Please assign '_player'");
        Assert.IsNotNull(_newCameraPosition, "Please assign '_newCameraPosition'");
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject == _player)
        {
            Camera.main.GetComponent<MovingCamera>().Target = _newCameraPosition;
        }
    }
}
