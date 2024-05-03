using UnityEngine;
using System.IO;

public static class SaveManager
{
    private static readonly string _playerDataFilePath = Application.dataPath + "/Resources/Saves/PlayerData.json";

    public static void SavePlayerData(PlayerData playerData)
    {
        string jsonData = JsonUtility.ToJson(playerData);

        File.WriteAllText(_playerDataFilePath, jsonData);

        Debug.Log($"Player data saved to: {_playerDataFilePath}");
    }

    public static PlayerData LoadPlayerData()
    {
        if (File.Exists(_playerDataFilePath))
        {
            // Read JSON data from file
            string jsonData = File.ReadAllText(_playerDataFilePath);

            // Deserialize JSON data to PlayerData object
            PlayerData playerData = JsonUtility.FromJson<PlayerData>(jsonData);

            Debug.Log($"Player data loaded from: {_playerDataFilePath}");
            return playerData;
        }
        else
        {
            Debug.LogWarning($"No save file found at: {_playerDataFilePath}");
            return null;
        }
    }

    public static void TerminateSave()
    {
        PlayerData data = new()
        {
            CurrentRun = new()
            {
                IsOngoing = false,
            },
            UnitData = new()
            {
            }
        };

        string jsonData = JsonUtility.ToJson(data);

        File.WriteAllText(_playerDataFilePath, jsonData);

        Debug.Log($"Run completed, save file reset");
    }
}