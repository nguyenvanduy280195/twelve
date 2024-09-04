using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

// TODO We don't need checkpoint. The game will be saved after each battle ends
public static class SaveSystem
{
    private static readonly string PLAYER_SAVE_FILENAME = "player.data";
    private static readonly string PLAYER_SAVE_FILEPATH = Application.persistentDataPath + PLAYER_SAVE_FILENAME;
    public static void SavePlayerStat(PlayerStat stat)
    {
        using var stream = new FileStream(PLAYER_SAVE_FILEPATH, FileMode.OpenOrCreate);
        var formatter = new BinaryFormatter();
        formatter.Serialize(stream, stat);
    }

    public static PlayerStat LoadPlayerStat()
    {
        if (!File.Exists(PLAYER_SAVE_FILEPATH))
        {
            Debug.LogWarning("Save file not found");
            return null;
        }

        using var stream = new FileStream(PLAYER_SAVE_FILEPATH, FileMode.Open);
        var formatter = new BinaryFormatter();
        var playerStat = formatter.Deserialize(stream) as PlayerStat;
        return playerStat;
    }

    public static bool PlayerDataExists => File.Exists(PLAYER_SAVE_FILEPATH);


}
