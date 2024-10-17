using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MultiAttack : PowerStrike
{

    // skill_level  | 0 1 2 3 4 5 6 7 8 9 10
    // -------------|------------------------
    // nRowsInBattle| 1 1 1 2 2 2 3 3 3 4 4 
    protected override IEnumerator _RunHavestingItems()
    {
        yield return new WaitUntil(() => BattleGameManager.Instance != null);
        
        var battleData = BattleGameManager.Instance.MyData;
        var items = battleData.Items;
        var nRow = (_SkillData.Level % 3) + 1;
        var itemsWillBeHarvested = new List<GameObject>();

        while (nRow > 0)
        {
            var iRandomRow = Random.Range(0, battleData.NumberOfRow - 1);
            for (int iCol = 0; iCol < battleData.NumberOfColumn; iCol++)
            {
                itemsWillBeHarvested.Add(items[iCol, iRandomRow]);
            }
            nRow--;
        }

        BattleGameManager.Instance.HarvestItems(itemsWillBeHarvested);

        _harvestingItemsRunning = false;
    }
}
