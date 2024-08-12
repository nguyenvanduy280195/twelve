using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

public static class SaveSystem
{
    public static void SavePlayerStat(PlayerStat stat)
    {
        BinaryFormatter formatter = new BinaryFormatter();
        string path = Application.persistentDataPath + "player.data";
        using (FileStream stream = new FileStream(path, FileMode.Create))
        {
            formatter.Serialize(stream, stat);
        }
    }

    public static PlayerStat LoadPlayerStat()
    {
        string path = Application.persistentDataPath + "player.data";
        if (!File.Exists(path))
        {
            Debug.LogWarning("Save file not found");
            return null;
        }

        BinaryFormatter formatter = new BinaryFormatter();
        using (FileStream stream = new FileStream(path, FileMode.Open))
        {
            return formatter.Deserialize(stream) as PlayerStat;
        }
    }
}
