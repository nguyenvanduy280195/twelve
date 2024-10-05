using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyData : MonoBehaviour
{
    [Header("For Observing")]
    [SerializeField] private int _id = -1;


    public int ID => _id;
    public ScriptableEnemyStat ScriptableEnemyStat;
}
