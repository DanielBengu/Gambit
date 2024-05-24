using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
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

    public Button closeSectionIcon;

    private bool isFirstStart;

    void Start()
    {
        if (forceFirstStart)
            RemovePref(PlayerPrefsEnum.AlreadyCompletedTutorial);

        isFirstStart = !DoesPrefExists(PlayerPrefsEnum.AlreadyCompletedTutorial);
        HandleFirstGameStart();

        if (unlockAllClasses)
            UnlockAllClasses();

        menuOptions.StartMenuAnimation();
        HandleSaves();
        SetStrings();
    }

    private void Update()
    {
        if (InputManager.IsExit())
            Application.Quit();
    }

    void UnlockAllClasses()
    {
        SetPref(PlayerPrefsEnum.WarriorUnlocked, 1);
        SetPref(PlayerPrefsEnum.RogueUnlocked, 1);
        SetPref(PlayerPrefsEnum.WizardUnlocked, 1);
        SetPref(PlayerPrefsEnum.MonkUnlocked, 1);
        SetPref(PlayerPrefsEnum.CrystalUnlocked, 1);
        SetPref(PlayerPrefsEnum.RangerUnlocked, 1);
        SetPref(PlayerPrefsEnum.TricksterUnlocked, 1);
    }

    void SetStrings()
    {
        Transform cardLeftContents = GameObject.Find("Card_left").transform.GetChild(0);
        Transform cardCenterContents = GameObject.Find("Card_center").transform.GetChild(0);

        TextMeshProUGUI newGameText = cardCenterContents.Find("New Game").GetComponent<TextMeshProUGUI>();
        TextMeshProUGUI continueText = cardLeftContents.Find("Continue").GetComponent<TextMeshProUGUI>();

        languageManager.SetLanguageValues(new()
        {
            new(0, newGameText, new object[0]),
            new(1, continueText, new object[0])
        });
    }

    void HandleSaves()
    {
        PlayerData playerData = SaveManager.LoadPlayerData();

        if (playerData == null || playerData.CurrentRun == null || !playerData.CurrentRun.IsOngoing)
        {
            menuOptions.CoverCard(MenuOptions.CardPosition.Left);
            return;
        }

        menuOptions.LoadRunInfo(playerData);
    }

    void HandleFirstGameStart()
    {
        if (isFirstStart)
            InitializeDefaultPreferences();
    }

    void InitializeDefaultPreferences()
    {
        SetPref(PlayerPrefsEnum.HasWonAnyRun, 0);
        SetPref(PlayerPrefsEnum.Language, (int)LanguageManager.Language.English);

        SetPref(PlayerPrefsEnum.WarriorUnlocked, 1);
        SetPref(PlayerPrefsEnum.RogueUnlocked, 0);
        SetPref(PlayerPrefsEnum.WizardUnlocked, 0);
        SetPref(PlayerPrefsEnum.MonkUnlocked, 0);
        SetPref(PlayerPrefsEnum.CrystalUnlocked, 0);
        SetPref(PlayerPrefsEnum.RangerUnlocked, 0);
        SetPref(PlayerPrefsEnum.TricksterUnlocked, 0);

        SaveManager.SavePlayerData(new PlayerData
        {
            CurrentRun = new() { IsOngoing = false }
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

        SaveNewRunData(mapToPlay.Id, Classes.Warrior);
        SwitchToRun(mapToPlay);
    }

    void SaveNewRunData(int mapId, Classes classId)
    {
        SaveManager.SavePlayerData(new PlayerData
        {
            UnitData = new UnitData
            {
                Name = "TEMP",
                Armor = 0,
                MaxHP = 10,
                CurrentHP = 10,
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
            callback = new List<System.Action> { LoadSceneGame },
            effect = VisualEffectsManager.Effects.MenuStartGame
        });
        blackScreen.SetActive(true);
    }

    public void CloseCharacterSelectionAndReturnToMM()
    {
        menuOptions.ChangeTitleVisibility(true);

        StartCoroutine(menuOptions.ResetCharacterCards());
        menuOptions.StartMenuAnimation();

        closeSectionIcon.gameObject.SetActive(false);
        closeSectionIcon.onClick.RemoveAllListeners();
    }

    #region Button Click

    public void ContinueButtonClick()
    {
        effectsManager.effects.Add(new()
        {
            obj = blackScreen,
            callback = new List<System.Action> { LoadSceneGame },
            effect = VisualEffectsManager.Effects.MenuStartGame
        });
        blackScreen.SetActive(true);
    }

    public void StartNewGameButtonClick()
    {
        menuOptions.StartMenuClearAnimation();

        if (isFirstStart)
        {
            StartTutorial();
        }
        else
        {
            closeSectionIcon.gameObject.SetActive(true);
            closeSectionIcon.onClick.AddListener(() => CloseCharacterSelectionAndReturnToMM());

            StartCoroutine(menuOptions.StartChooseCharacterAnimation());
            menuOptions.ChangeTitleVisibility(false);
        }
    }

    #endregion

    public void LoadSceneGame()
    {
        SceneManager.LoadScene("Game");
    }

    Map StartRandomMap(MapData mapData)
    {
        List<Map> mapListWithoutTutorial = mapData.Maps.Where(m => m.Id != TUTORIAL_WORLD_ID).ToList();
        int mapIndex = Random.Range(0, mapListWithoutTutorial.Count);
        return mapListWithoutTutorial[mapIndex];
    }
}