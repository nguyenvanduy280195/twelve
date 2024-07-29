using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WinResult : MonoBehaviour
{
    [SerializeField] private UIStat _expCurrent;
    [SerializeField] private UIStat _expInBattle;
    [SerializeField] private UIStat _expFromEnemy;
    [SerializeField] private UIStat _expTotal;
    [SerializeField] private UIStat _goldCurrent;
    [SerializeField] private UIStat _goldInBattle;
    [SerializeField] private UIStat _goldBonus;
    [SerializeField] private UIStat _goldTotal;

    public void OnNextButtonClicked()
    {

    }

    private Queue<Action> actions = new();

    private void Update()
    {
        if (actions.Count > 0)
        {
            var action = actions.Dequeue();
            action?.Invoke();
        }
    }
}
