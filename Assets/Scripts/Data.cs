using System;
using UnityEngine;

public class Data : MonoBehaviour
{
    [Header("General")]
    public int NumberOfRow = 8;
    public int NumberOfColumn = 8;
    public int MinMatches = 3;
    public float MyEpsilon = 0.000001f;
    public string inputFilename = "";

    [Header("Animation Durations")]
    public float SwapAnimationDuration = 0.2f;
    public float FallAnimationDuration = 0.2f;

    [Header("Players' information")]
    public Player player1;
    public Player player2;

    [NonSerialized]
    public ItemArray Items;

    public ItemsSupporter ItemsSupporter;

    public GameState GameState
    {
        get => _gameState;
        set
        {
            _gameState = value;
            Debug.Log($"GameState: {_gameState}");
        }
    }

    [NonSerialized]
    private GameState _gameState;

    
    public Data()
    {
        Items = new ItemArray(NumberOfRow, NumberOfColumn, MinMatches);
        ItemsSupporter = new ItemsSupporter(Items);
    }

}