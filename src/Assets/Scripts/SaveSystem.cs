using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public static class SaveSystem
{
    public static void SaveData(Player player, TilemapGen tilemap)
    {
        var formatter = new BinaryFormatter();
        string path = Application.persistentDataPath + "/savedata.fgs";
        var stream = new FileStream(path, FileMode.Create);
        var data = new SaveData(player, tilemap);

        formatter.Serialize(stream, data);

        stream.Close();
    }

    public static bool SaveDataExists() => File.Exists(Application.persistentDataPath + "/savedata.fgs");

    public static SaveData LoadData()
    {
        string path = Application.persistentDataPath + "/savedata.fgs";
        if (File.Exists(path))
        {
            var formatter = new BinaryFormatter();
            var stream = new FileStream(path, FileMode.Open);
            SaveData data = (SaveData)formatter.Deserialize(stream);

            stream.Close();
            return data;
        }
        else
        {
            Debug.LogError("Save data not found in " + path);
            return null;
        }
    }
}
