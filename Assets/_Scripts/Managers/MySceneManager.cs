using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MySceneManager : SingletonPersistent<MySceneManager>
{
    [SerializeField] Animator _sceneTransitionAnimator;

    public void LoadInBattleScene() => StartCoroutine(_StartLoadingInBattleScene());

    private IEnumerator _StartLoadingInBattleScene()
    {
        _sceneTransitionAnimator.SetTrigger("end");
        yield return new WaitForSeconds(1f);
        SceneManager.LoadScene("InBattle");
    }

    public void LoadMazeScene() => StartCoroutine(_StartLoadingMazeScene());

    private IEnumerator _StartLoadingMazeScene()
    {
        _sceneTransitionAnimator.SetTrigger("end");
        yield return new WaitForSeconds(1f);
        SceneManager.LoadScene("Maze");
    }
}
