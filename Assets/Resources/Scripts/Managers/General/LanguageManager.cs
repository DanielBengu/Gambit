using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class LanguageManager : MonoBehaviour
{
    LanguageClass _languageList;
    Language _selectedLanguage;

    public Language SelectedLanguage { get { return GetCurrentLanguage(); } }
    public LanguageClass LanguageList { get { return GetLanguageList(); } }

    Language GetCurrentLanguage()
    {
        if(_selectedLanguage == Language.Default)
        {
            _selectedLanguage = (Language)PlayerPrefsManager.GetPref<int>(PlayerPrefsManager.PlayerPrefsEnum.Language);
        }

        return _selectedLanguage;
    }

    LanguageClass GetLanguageList()
    {
        if(_languageList == null)
        {
            string language = GetLanguagePath();
            _languageList = JSONManager.GetFileFromJSON<LanguageClass>(language);
        }

        return _languageList;
    }

    string GetLanguagePath()
    {
        return SelectedLanguage switch
        {
            Language.Italian => JSONManager.LANGUAGES_PATH_ITA,
            _ => JSONManager.LANGUAGES_PATH_ENG,
        };
    }

    string GetString(int stringId)
    {
        return LanguageList.Strings.Find(s => s.Id == stringId).Value;
    }

    public void SetLanguageValues(List<LanguageStruct> values)
    {
        foreach (var item in values)
        {
            string newText = GetString(item.id);
            item.obj.text = string.Format(newText, item.parameters);
        }  
    }

    public string GetText(int textId)
    {
        return GetString(textId);
    }

    public struct LanguageStruct
    {
        public int id;
        public TextMeshProUGUI obj;
        public object[] parameters;

        public LanguageStruct(int id, TextMeshProUGUI obj, object[] parameters)
        {
            this.id = id;
            this.obj = obj;
            this.parameters = parameters;
        }
    }

    public enum Language
    {
        Default,
        English,
        Italian
    }
}