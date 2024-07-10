using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using System.Linq;

public class ItemArray
{
    private GameObject[,] _gameObjectList;

    private int _nRows;
    private int _nCols;
    private int _minNumberOfMatches;

    public ItemArray(int row, int col, int minNumberOfMatches)
    {
        _nRows = row;
        _nCols = col;
        _minNumberOfMatches = minNumberOfMatches;

        _gameObjectList = new GameObject[_nCols, _nRows];
    }

    public GameObject this[int c, int r]
    {
        get
        {
            try
            {
                return _gameObjectList[c, r];
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        set
        {
            try
            {
                _gameObjectList[c, r] = value;
            }
            catch (Exception e)
            {
                Debug.Log($"[_gameObjectList[{c}, {r}] = {value}] - {e}");
            }

        }
    }

    public List<GameObject> AsList => _gameObjectList.Cast<GameObject>().ToList();

    public Action<GameObject> DestroyItemCallback;

    public int RowLength => _nRows;
    public int ColLength => _nCols;
    public int MinNumberOfMatches => _minNumberOfMatches;

    public void Clear()
    {
        if (DestroyItemCallback is null)
        {
            Debug.LogWarning($"Please assign Destroy property");
            return;
        }

        foreach (var go in _gameObjectList)
        {
            if (go != null)
            {
                DestroyItemCallback(go);
            }
        }
    }
}
