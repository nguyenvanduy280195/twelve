using System;
using UnityEngine;

public class BattleData : MonoBehaviour
{
    [Header("General")]
    public int NumberOfRow = 8;
    public int NumberOfColumn = 8;
    public int MinMatches = 3;
    public string inputFilename = "";

    [Header("Animation Speed")]
    public float SwappingAnimationSpeed = 0.2f;
    public float FallingAnimationSpeed = 0.2f;

    [NonSerialized]
    public ItemArray Items;

    public ItemsSupporter ItemsSupporter;
    
    public BattleData()
    {
        Items = new ItemArray(NumberOfRow, NumberOfColumn, MinMatches);
        ItemsSupporter = new ItemsSupporter(Items);
    }

}