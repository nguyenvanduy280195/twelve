using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class TriggeringDoor : MonoBehaviour
{
    [SerializeField]
    private Transform _newCameraPosition;

    private void Start()
    {
        Assert.IsNotNull(_newCameraPosition, "Please assign '_newCameraPosition'");
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (UnitManager.Instance.GetPlayer() != null)
        {
            if (collision.gameObject == UnitManager.Instance.GetPlayer())
            {
                Camera.main.GetComponent<MovingCamera>().Target = _newCameraPosition;
            }
        }
    }
}
