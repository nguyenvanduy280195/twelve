using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

public static class SaveSystem
{
    public static void SavePlayerStat(PlayerStat stat)
    {
        var path = Application.persistentDataPath + "player.data";
        using (FileStream stream = new FileStream(path, FileMode.Create))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            formatter.Serialize(stream, stat);
        }
    }

    public static PlayerStat LoadPlayerStat()
    {
        var path = Application.persistentDataPath + "player.data";
        if (!File.Exists(path))
        {
            Debug.LogWarning("Save file not found");
            return null;
        }

        using (FileStream stream = new FileStream(path, FileMode.Open))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            var playerStat = formatter.Deserialize(stream) as PlayerStat;
            var playerAfterGeneratingPartLack = _GeneratePlayerStatPartWithoutSaving(playerStat);
            return playerStat;
        }
    }

    private static PlayerStat _GeneratePlayerStatPartWithoutSaving(PlayerStat playerStat)
    {
        playerStat.Attack = playerStat.Strength;
        playerStat.HPMax = 10f * playerStat.Vitality;
        playerStat.HPRegen = 0.1f * playerStat.Vitality;
        playerStat.ManaMax = 10f * playerStat.Intelligent;
        playerStat.ManaRegen = 0.1f * playerStat.Intelligent;
        playerStat.StaminaMax = 10f * playerStat.Endurance;
        return playerStat;
    }
}
