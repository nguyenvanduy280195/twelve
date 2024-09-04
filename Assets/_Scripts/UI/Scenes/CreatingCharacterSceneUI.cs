using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreatingCharacterSceneUI : MonoBehaviour
{

    [SerializeField] private ScriptablePlayerStat _soldier;

    public void OnEnjoyButtonClicked()
    {
        _CreateCharacter();
        MySceneManager.Instance?.LoadMazeScene();
    }

    public void OnNopeButtonClicked()
    {
        MySceneManager.Instance?.LoadMainMenuScene();
    }

    private void _CreateCharacter()
    {
        var playerStat = _GeneratePlayerStat(_soldier.PlayerStat.Clone());
        SaveSystem.SavePlayerStat(playerStat);
    }

    private static PlayerStat _GeneratePlayerStat(PlayerStat playerStat)
    {
        playerStat.Attack = 0.5f * playerStat.Strength;
        playerStat.HPMax = 10f * playerStat.Vitality;
        playerStat.HP = 10f * playerStat.Vitality;
        playerStat.HPRegen = 0.1f * playerStat.Vitality;
        playerStat.ManaMax = 10f * playerStat.Intelligent;
        playerStat.Mana = 0f;
        playerStat.ManaRegen = 0.1f * playerStat.Intelligent;
        playerStat.StaminaMax = 10f * playerStat.Endurance;
        playerStat.StaminaRegen = 0.1f * playerStat.Endurance;
        playerStat.StaminaConsumeWeight = 0.1f * playerStat.Level;
        playerStat.Stamina = 10f * playerStat.Endurance;
        return playerStat;
    }
}
