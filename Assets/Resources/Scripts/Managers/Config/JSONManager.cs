using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public static class JSONManager
{
    public static string GAMEDATA_PATH = "Assets\\Resources\\GameData";
    public static string MAPS_PATH = $"{GAMEDATA_PATH}\\Maps.json";
    public static string CLASSES_PATH = $"{GAMEDATA_PATH}\\Classes.json";
    public static string ENEMIES_PATH = $"{GAMEDATA_PATH}\\Enemies.json";
    public static string EVENTS_PATH = $"{GAMEDATA_PATH}\\Events.json";
    public static string CARDS_PATH = $"{GAMEDATA_PATH}\\Cards.json";
    public static T GetFileFromJSON<T>(string path)
    {
        string data = File.ReadAllText(path);
        T obj = JsonUtility.FromJson<T>(data);
        return obj;
    }

    public static void SaveFileToJSON(object data, string path)
    {
        string textData = JsonUtility.ToJson(data);
        File.WriteAllText(path, textData);
    }
}
