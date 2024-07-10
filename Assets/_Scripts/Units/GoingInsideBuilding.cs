using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GoingInsideBuilding : MonoBehaviour
{
    [SerializeField]
    private GameObject _player;

    [SerializeField]
    private string SceneName = "";

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision != null && collision.gameObject == _player)
        {
            SceneManager.LoadScene(SceneName);
        }
    }
}
