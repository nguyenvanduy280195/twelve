using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

public static class SaveSystem
{
    private static readonly string PLAYER_SAVE_FILENAME = "player.data";
    private static readonly string PLAYER_SAVE_FILEPATH = Application.persistentDataPath + PLAYER_SAVE_FILENAME;
    public static void SavePlayerStat(PlayerData stat)
    {
        using var stream = new FileStream(PLAYER_SAVE_FILEPATH, FileMode.OpenOrCreate);
        var formatter = new BinaryFormatter();
        formatter.Serialize(stream, stat);
    }

    public static PlayerData LoadPlayerStat()
    {
        if (!File.Exists(PLAYER_SAVE_FILEPATH))
        {
            Debug.LogWarning("Save file not found");
            return null;
        }

        using var stream = new FileStream(PLAYER_SAVE_FILEPATH, FileMode.Open);
        var formatter = new BinaryFormatter();
        var playerStat = formatter.Deserialize(stream) as PlayerData;
        return playerStat;
    }

    public static bool PlayerDataExists => File.Exists(PLAYER_SAVE_FILEPATH);


}
