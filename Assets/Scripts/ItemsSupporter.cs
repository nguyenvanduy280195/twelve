using System.Collections.Generic;
using System;
using UnityEngine;
using System.Linq;
using UnityEngine.UIElements;

public class ItemsSupporter
{
    public struct ItemLocation
    {
        public int iCol;
        public int iRow;

        public ItemLocation(int iCol, int iRow)
        {
            this.iCol = iCol;
            this.iRow = iRow;
        }

        public bool Equals(ItemLocation other) => iCol == other.iCol && iRow == other.iRow;
    }

    private ItemArray _items;

    public ItemsSupporter(ItemArray items)
    {
        _items = items;
    }

    public void SwapItems(GameObject go1, GameObject go2) => SwapItems(go1.GetComponent<Item>(), go2.GetComponent<Item>());

    public void SwapItems(Item item1, Item item2)
    {
        item1.Swap(item2);

        (_items[item1.col, item1.row], _items[item2.col, item2.row]) = (_items[item2.col, item2.row], _items[item1.col, item1.row]);
    }


    public List<GameObject> GetMatchesInCol(GameObject candidate) => GetMatchesInCol(candidate.GetComponent<Item>());

    public List<GameObject> GetMatchesInCol(Item candidate) => GetMatchesInCol(candidate.col, candidate.row, candidate.tag);

    public List<GameObject> GetMatchesInCol(int col, int row, string tag)
    {
        var matches = new List<GameObject>();
        
        var itemsInBottom = _items.AsList.Where(go => go != null && go.GetItemCol() == col && go.GetItemRow() < row)
                                         .Distinct()
                                         .OrderByDescending(it => it.GetItemRow());
        foreach (var it in itemsInBottom)
        {

            if (tag == it.tag)
            {
                matches.Add(it);
            }
            else
            {
                break;
            }
        }

        var itemsInTop = _items.AsList.Where(go => go != null && go.GetItemCol() == col && row < go.GetItemRow())
                                      .Distinct()
                                      .OrderBy(it => it.GetItemRow());
        foreach (var it in itemsInTop)
        {

            if (tag == it.tag)
            {
                matches.Add(it);
            }
            else
            {
                break;
            }
        }

        return matches;
    }

    public List<GameObject> GetMatchesInRow(GameObject candidate) => GetMatchesInRow(candidate.GetComponent<Item>());

    public List<GameObject> GetMatchesInRow(Item candidate) => GetMatchesInRow(candidate.col, candidate.row, candidate.tag);

    public List<GameObject> GetMatchesInRow(int col, int row, string tag)
    {
        var matches = new List<GameObject>();

        var itemsInLeft = _items.AsList.Where(go => go != null && go.GetItemRow() == row && go.GetItemCol() < col)
                                       .Distinct()
                                       .OrderByDescending(it => it.GetItemCol());

        foreach (var it in itemsInLeft)
        {

            if (tag == it.tag)
            {
                matches.Add(it);
            }
            else
            {
                break;
            }
        }

        var itemsInRight = _items.AsList.Where(go => go != null && go.GetItemRow() == row && col < go.GetItemCol())
                                        .Distinct()
                                        .OrderBy(it => it.GetItemCol());

        foreach (var it in itemsInRight)
        {
            if (tag == it.tag)
            {
                matches.Add(it);
            }
            else
            {
                break;
            }
        }

        return matches;
    }

    // return: selected(col, row), dragged(col, row), nMatches
    public IEnumerable<(ItemLocation, ItemLocation, int)> AllSwappableItems
    {
        get
        {
            var swappableItems = new Dictionary<(ItemLocation, ItemLocation), int>();

            Action<(GameObject, GameObject), int> updateDictionaryAction = (gos, value) =>
            {
                var key1 = new ItemLocation(gos.Item1.GetItemCol(), gos.Item1.GetItemRow());
                var key2 = new ItemLocation(gos.Item2.GetItemCol(), gos.Item2.GetItemRow());
                var key = (key1, key2);
                if (swappableItems.ContainsKey(key))
                {
                    swappableItems[key] += value;
                }
                else
                {
                    swappableItems[key] = value;
                }
            };

            Action<GameObject, GameObject> action = (candidate, neighbor) =>
            {
                SwapItems(candidate, neighbor);

                var candidateMatchesInCol = GetMatchesInCol(candidate);
                var candidateMatchesInRow = GetMatchesInRow(candidate);

                var neighborMatchesInCol = GetMatchesInCol(neighbor);
                var neighborMatchesInRow = GetMatchesInRow(neighbor);

                SwapItems(candidate, neighbor);

                if (candidateMatchesInCol.Count() + 1 >= _items.MinNumberOfMatches)
                {
                    updateDictionaryAction((candidate, neighbor), candidateMatchesInCol.Count());
                }
                if (candidateMatchesInRow.Count() + 1 >= _items.MinNumberOfMatches)
                {
                    updateDictionaryAction((candidate, neighbor), candidateMatchesInRow.Count());
                }

                if (candidateMatchesInCol.Count() + 1 >= _items.MinNumberOfMatches)
                {
                    updateDictionaryAction((candidate, neighbor), candidateMatchesInCol.Count());
                }
                if (candidateMatchesInRow.Count() + 1 >= _items.MinNumberOfMatches)
                {
                    updateDictionaryAction((candidate, neighbor), candidateMatchesInRow.Count());
                }
            };

            for (int iRow = 0; iRow < _items.RowLength; iRow++)
            {
                for (int iCol = 0; iCol < _items.ColLength; iCol++)
                {
                    //left
                    if (iCol - 1 > 0)
                    {
                        action(_items[iCol, iRow], _items[iCol - 1, iRow]);
                    }

                    //right
                    if (iCol + 1 < _items.ColLength)
                    {
                        action(_items[iCol, iRow], _items[iCol + 1, iRow]);
                    }

                    //bottom
                    if (iRow - 1 > 0)
                    {
                        action(_items[iCol, iRow], _items[iCol, iRow - 1]);
                    }

                    //top
                    if (iRow + 1 < _items.RowLength)
                    {
                        action(_items[iCol, iRow], _items[iCol, iRow + 1]);
                    }
                }
            }

            return swappableItems.Select(it => (it.Key.Item1, it.Key.Item2, it.Value));
        }
    }
}