using System;
using UnityEngine;

public static class PlayerPrefsManager
{
    public static int MISSING_PREF_INT = -1;

    public static bool DoesPrefExists(PlayerPrefsEnum pref)
    {
        return PlayerPrefs.HasKey(pref.ToString());
    }

    public static T GetPref<T>(PlayerPrefsEnum pref)
    {
        string prefKey = pref.ToString();

        switch (typeof(T).Name)
        {
            case "Int32":
                return (T)(object)PlayerPrefs.GetInt(prefKey);
            case "Single":
                return (T)(object)PlayerPrefs.GetFloat(prefKey);
            case "String":
                return (T)(object)PlayerPrefs.GetString(prefKey);
            default:
                Debug.LogError($"Unsupported PlayerPrefs type: {typeof(T).Name}.");
                return default; // Return default value for unsupported types
        }
    }

    public static void SetPref<T>(PlayerPrefsEnum pref, T value)
    {
        string prefKey = pref.ToString();
        Action<string, T> setMethod;

        switch (typeof(T).Name)
        {
            case "Int32":
                setMethod = (key, val) => PlayerPrefs.SetInt(key, (int)(object)val);
                break;
            case "Single":
                setMethod = (key, val) => PlayerPrefs.SetFloat(key, (float)(object)val);
                break;
            case "String":
                setMethod = (key, val) => PlayerPrefs.SetString(key, (string)(object)val);
                break;
            default:
                Debug.LogError($"Unsupported PlayerPrefs type: {typeof(T).Name}.");
                return;
        }

        setMethod(prefKey, value);
        PlayerPrefs.Save();
    }

    public static void RemovePref(PlayerPrefsEnum pref)
    {
        PlayerPrefs.DeleteKey(pref.ToString());
    }

    public enum PlayerPrefsEnum
    {
        AlreadyLaunchedGame,
        HasWonAnyRun,
        Language,
        WarriorUnlocked,
        RogueUnlocked,
        WizardUnlocked,
        BerserkUnlocked,
        ArchmageUnlocked,
        PoisonerUnlocked,
        TricksterUnlocked,
    }
}