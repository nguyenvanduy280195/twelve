using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : SingletonPersistent<GameManager>
{
    [Header("For Observing")]

    [SerializeField]
    private GameMode _mode = GameMode.Nope;
    public GameMode GetGameMode() => _mode;
    public void SetGameMode(GameMode value) => _mode = value;

    [SerializeField]
    private bool _pausing;
    public bool IsPausing() => _pausing;
    public void SetPausing(bool value) => _pausing = value;
}

public enum GameMode
{
    Nope,
    Battle,
    Casual
}