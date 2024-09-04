using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MySceneManager : Singleton<MySceneManager>
{
    private Animator _sceneTransitionAnimator;

    private void Start()
    {
        _sceneTransitionAnimator = GetComponent<Animator>();
    }

    public void LoadCreatingCharacterScene() => StartCoroutine(_StartLoadingCreatingCharacterScene());
    private IEnumerator _StartLoadingCreatingCharacterScene()
    {
        _sceneTransitionAnimator.SetTrigger("end");
        yield return new WaitForSeconds(1f);
        SceneManager.LoadScene("CreatingCharacter");
    }

    public void LoadInBattleScene() => StartCoroutine(_StartLoadingInBattleScene());

    private IEnumerator _StartLoadingInBattleScene()
    {
        _sceneTransitionAnimator.SetTrigger("end");
        yield return new WaitForSeconds(1f);
        SceneManager.LoadScene("InBattle");
        GameManager.Instance?.SetPausing(false);
    }

    public void LoadMazeScene() => StartCoroutine(_StartLoadingMazeScene());

    private IEnumerator _StartLoadingMazeScene()
    {
        _sceneTransitionAnimator.SetTrigger("end");
        yield return new WaitForSeconds(1f);
        SceneManager.LoadScene("Maze");
    }

    public void LoadMainMenuScene() => StartCoroutine(_StartLoadingMainMenuScene());

    private IEnumerator _StartLoadingMainMenuScene()
    {
        _sceneTransitionAnimator.SetTrigger("end");
        yield return new WaitForSeconds(1f);
        SceneManager.LoadScene("MainMenu");
    }
}
