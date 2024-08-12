using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[Serializable]
public struct UIUnit
{
    public Bar HP;
    public Bar Mana;
    public Bar Stamina;
}

public class BattleUIManager : Singleton<BattleUIManager>
{
    public UIUnit Player;
    public UIUnit Enemy;
}
