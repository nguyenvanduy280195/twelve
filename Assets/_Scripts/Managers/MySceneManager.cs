using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.SceneManagement;

public class MySceneManager : Singleton<MySceneManager>
{
    private Animator _sceneTransitionAnimator;

    private void Start()
    {
        _sceneTransitionAnimator = GetComponent<Animator>();
    }

    public void LoadCreatingCharacterScene() => StartCoroutine(_StartLoadingScene("SceneCreatingCharacter"));
    public void LoadInBattleScene() => StartCoroutine(_StartLoadingScene("SceneInBattle"));
    public void LoadMazeScene() => StartCoroutine(_StartLoadingScene("SceneMaze"));
    public void LoadMainMenuScene() => StartCoroutine(_StartLoadingScene("SceneMainMenu"));
    private IEnumerator _StartLoadingScene(string sceneLabel)
    {
        _sceneTransitionAnimator.SetTrigger("end");
        yield return new WaitForSeconds(1f);
        Addressables.LoadSceneAsync(sceneLabel);
    }
}
