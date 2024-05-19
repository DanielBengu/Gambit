using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using static CardsManager;
using static PlayerPrefsManager;

public class MenuManager : MonoBehaviour
{
    public static readonly int TUTORIAL_WORLD_ID = 0;

    public bool forceFirstStart = false;
    public bool unlockAllClasses = false;

    public MenuOptions menuOptions;
    public VisualEffectsManager effectsManager;
    public LanguageManager languageManager;

    public GameObject blackScreen;

    void Start()
    {
        if (forceFirstStart)
            RemovePref(PlayerPrefsEnum.AlreadyLaunchedGame);

        FirstGameStart();

        if (unlockAllClasses)
            UnlockAllClasses();

        menuOptions.StartMenuAnimation();

        HandleSaves();
        SetStrings();
    }

    void UnlockAllClasses()
    {
        SetPref(PlayerPrefsEnum.WarriorUnlocked, 1);
        SetPref(PlayerPrefsEnum.RogueUnlocked, 1);
        SetPref(PlayerPrefsEnum.WizardUnlocked, 1);
        SetPref(PlayerPrefsEnum.BerserkUnlocked, 1);
        SetPref(PlayerPrefsEnum.ArchmageUnlocked, 1);
        SetPref(PlayerPrefsEnum.RangerUnlocked, 1);
        SetPref(PlayerPrefsEnum.TricksterUnlocked, 1);
    }

    void SetStrings()
    {
        Transform cardLeftContents = GameObject.Find("Card_left").transform.GetChild(0);
        Transform cardCenterContents = GameObject.Find("Card_center").transform.GetChild(0);
        //Transform cardRightContents = GameObject.Find("Card_right").transform.GetChild(0);

        TextMeshProUGUI newGameText = cardCenterContents.Find("New Game").GetComponent<TextMeshProUGUI>();
        TextMeshProUGUI continueText = cardLeftContents.Find("Continue").GetComponent<TextMeshProUGUI>();

        languageManager.SetLanguageValues(new()
        {
            new(0, newGameText, new object[0]),
            new(1, continueText, new object[0]),
        });
    }

    void HandleSaves()
    {
        PlayerData playerData = SaveManager.LoadPlayerData();

        if (playerData == null || playerData.CurrentRun == null)
        {
            menuOptions.CoverCard(MenuOptions.CardPosition.Left);
            return;
        }

        HandleCards(playerData);
    }

    void HandleCards(PlayerData data)
    {
        if (!data.CurrentRun.IsOngoing)
            menuOptions.CoverCard(MenuOptions.CardPosition.Left);
        else
            menuOptions.LoadRunInfo(data);
    }

    void FirstGameStart()
    {
        if (DoesPrefExists(PlayerPrefsEnum.AlreadyLaunchedGame))
            return;

        SetPref(PlayerPrefsEnum.HasWonAnyRun, 0);
        SetPref(PlayerPrefsEnum.Language, (int)LanguageManager.Language.English);

        SetPref(PlayerPrefsEnum.WarriorUnlocked, 1);
        SetPref(PlayerPrefsEnum.RogueUnlocked, 0);
        SetPref(PlayerPrefsEnum.WizardUnlocked, 0);
        SetPref(PlayerPrefsEnum.BerserkUnlocked, 0);
        SetPref(PlayerPrefsEnum.ArchmageUnlocked, 0);
        SetPref(PlayerPrefsEnum.RangerUnlocked, 0);
        SetPref(PlayerPrefsEnum.TricksterUnlocked, 0);

        SaveManager.SavePlayerData(new()
        {
            CurrentRun = new()
            {
                IsOngoing = false
            }
        });
    }

    public void StartGame(Classes playerClass)
    {
        MapData mapList = JSONManager.GetFileFromJSON<MapData>(JSONManager.MAPS_PATH);
        Map mapToPlay = StartRandomMap(mapList);

        SaveNewRunData(mapToPlay.Id, playerClass);
        SwitchToRun(mapToPlay);
    }

    public void StartTutorial()
    {
        MapData mapList = JSONManager.GetFileFromJSON<MapData>(JSONManager.MAPS_PATH);
        Map mapToPlay = mapList.Maps.Find(m => m.Id == TUTORIAL_WORLD_ID);

        Classes playerClass = Classes.Warrior;
        SetPref(PlayerPrefsEnum.AlreadyLaunchedGame, 1);

        SaveNewRunData(mapToPlay.Id, playerClass);
        SwitchToRun(mapToPlay);
    }

    void SaveNewRunData(int mapId, Classes classId)
    {
        SaveManager.SavePlayerData(new()
        {
            UnitData = new()
            {
                Name = "TEMP",
                Armor = 2,
                MaxHP = 100,
                CurrentHP = 100,
                MaxScore = 12,
                NumberOfAttacks = 1
            },
            CurrentRun = new()
            {
                IsOngoing = true,
                MapId = mapId,
                ClassId = classId,
                CurrentFloor = -1,
                CardList = GameManager.GetStartingDeck(classId),
                ActionDeck = GameManager.GetStartingActionDeck(classId),
                GoldAmount = 0
            }
        });
    }

    void SwitchToRun(Map mapToPlay)
    {
        Debug.Log($"Launching world {mapToPlay.Name} (id: {mapToPlay.Id})");
        effectsManager.effects.Add(new()
        {
            obj = blackScreen,
            callback = new() { LoadSceneGame },
            effect = VisualEffectsManager.Effects.MenuStartGame
        });
        blackScreen.SetActive(true);
    }

    public void StartNewGameButtonClick()
    {
        menuOptions.StartMenuClearAnimation();

        if (DoesPrefExists(PlayerPrefsEnum.AlreadyLaunchedGame))
        {
            menuOptions.StartChooseCharacterAnimation();
            menuOptions.ChangeTitleVisibility(false);
        }
        else
        {
            StartTutorial();
        }
    }

    public void LoadSceneGame()
    {
        SceneManager.LoadScene("Game");
    }

    Map StartRandomMap(MapData mapData)
    {
        List<Map> mapListWithoutTutorial = mapData.Maps.Where(m => m.Id != 0).ToList();

        int mapIndex = Random.Range(0, mapListWithoutTutorial.Count);

        return mapListWithoutTutorial[mapIndex];
    }
}