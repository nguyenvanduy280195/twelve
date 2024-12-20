using System;
using System.Collections;
using UnityEngine;

public class MazeSceneUI : MySceneBase
{
    public static event Action OnEnabled;
    public static event Action OnStarted;

    [SerializeField] private GameObject _menuInGame;

    public void OnMenuInGameButtonClicked() => _menuInGame.SetActive(true);

    private void OnEnable() => OnEnabled?.Invoke();

    private void Start()
    {
        StartCoroutine(_SetupPlayer());
        OnStarted?.Invoke();
    }

    private IEnumerator _SetupPlayer()
    {
        yield return new WaitUntil(() => UnitManager.Instance != null);
        yield return new WaitUntil(() => UnitManager.Instance.GetPlayer() != null);

        var playerStat = UnitManager.Instance.PlayerData;
        var position = MatchingBattleManager.Instance?.PlayerPosition ?? playerStat.Position.ToVector3();
        UnitManager.Instance.GetPlayer().transform.position = position;
    }

}
