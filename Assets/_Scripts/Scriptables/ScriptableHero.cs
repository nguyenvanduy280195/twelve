using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "New Scriptable Hero")]
public class ScriptableHero : ScriptableUnitBase
{
    public HeroType Type;


}


public enum HeroType
{
    Soldier,
    Wizard
}