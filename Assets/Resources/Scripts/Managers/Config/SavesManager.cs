using UnityEngine;
using System.IO;
using static CardsManager;
using System.Collections.Generic;

public static class SaveManager
{
    private static readonly string _playerDataFilePath = Path.Combine(Application.persistentDataPath, "PlayerData.json");

    public static void SavePlayerData(PlayerData playerData)
    {
        string directoryPath = Path.GetDirectoryName(_playerDataFilePath);

        // Check if the directory exists, if not, create it
        if (!Directory.Exists(directoryPath))
        {
            Directory.CreateDirectory(directoryPath);
        }

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

    public static List<Classes> GetUnlockedClasses()
    {
        List<Classes> classes = new()
        {
            Classes.Warrior,
            Classes.Rogue,
            Classes.Wizard
        };

        if(PlayerPrefsManager.GetPref<int>(PlayerPrefsManager.PlayerPrefsEnum.MonkUnlocked) == 1)
            classes.Add(Classes.Monk);

        if (PlayerPrefsManager.GetPref<int>(PlayerPrefsManager.PlayerPrefsEnum.CrystalUnlocked) == 1)
            classes.Add(Classes.Crystal);

        if (PlayerPrefsManager.GetPref<int>(PlayerPrefsManager.PlayerPrefsEnum.TricksterUnlocked) == 1)
            classes.Add(Classes.Trickster);

        if (PlayerPrefsManager.GetPref<int>(PlayerPrefsManager.PlayerPrefsEnum.RangerUnlocked) == 1)
            classes.Add(Classes.Ranger);

        return classes;
    }
}