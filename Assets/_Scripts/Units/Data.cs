using System;
using UnityEngine;

public class Data : MonoBehaviour
{
    [Header("General")]
    public int NumberOfRow = 8;
    public int NumberOfColumn = 8;
    public int MinMatches = 3;
    public string inputFilename = "";

    [Header("Animation Durations")]
    public float SwappingAnimationDuration = 0.2f;
    public float FallingAnimationDuration = 0.2f;

    [NonSerialized]
    public ItemArray Items;

    public ItemsSupporter ItemsSupporter;
    
    public Data()
    {
        Items = new ItemArray(NumberOfRow, NumberOfColumn, MinMatches);
        ItemsSupporter = new ItemsSupporter(Items);
    }

}