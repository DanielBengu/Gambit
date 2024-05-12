using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using static CardsManager;
using static PlayerPrefsManager;

public class MenuManager : MonoBehaviour
{
    public bool forceFirstStart = false;
    public Classes forceClass = Classes.Basic;

    public MenuOptions menuOptions;
    public VisualEffectsManager effectsManager;
    public LanguageManager languageManager;

    public GameObject blackScreen;

    void Start()
    {
        FirstGameStart();

        menuOptions.StartCardAnimation();

        HandleSaves();
        SetStrings();
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
        /*if (GetPref<int>(PlayerPrefsEnum.HasWonAnyRun) != 1)
            menuOptions.CoverCard(MenuOptions.CardPosition.Right);*/

        if (!data.CurrentRun.IsOngoing)
            menuOptions.CoverCard(MenuOptions.CardPosition.Left);
        else
            menuOptions.LoadRunInfo(data);
    }

    void FirstGameStart()
    {
        if (DoesPrefExists(PlayerPrefsEnum.AlreadyLaunchedGame) && !forceFirstStart)
            return;

        SetPref(PlayerPrefsEnum.HasWonAnyRun, 0);
        SetPref(PlayerPrefsEnum.Language, (int)LanguageManager.Language.English);

        SaveManager.SavePlayerData(new()
        {
            CurrentRun = new()
            {
                IsOngoing = false
            }
        });
    }

    public void StartNewGameButtonClick()
    {
        MapData mapList = JSONManager.GetFileFromJSON<MapData>(JSONManager.MAPS_PATH);
        Map mapToPlay;
        Classes playerClass = Classes.Basic;
        if (!DoesPrefExists(PlayerPrefsEnum.AlreadyLaunchedGame) || forceFirstStart)
        {
            mapToPlay = mapList.Maps.Find(m => m.Id == 0); //Tutorial world
            playerClass = forceClass != Classes.Basic ? forceClass : Classes.Warrior; //Tutorial starts with warrior
            SetPref(PlayerPrefsEnum.AlreadyLaunchedGame, 1);
        }
        else
        {
            mapToPlay = StartRandomMap(mapList);
            playerClass = Classes.Basic; //To change with class selected
        }

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
                MapId = mapToPlay.Id,
                ClassId = playerClass,
                CurrentFloor = 1,
                CardList = GameManager.GetStartingDeck(playerClass),
                ActionDeck = GameManager.GetStartingActionDeck(playerClass)
            }
        });

        Debug.Log($"Launching world {mapToPlay.Name} (id: {mapToPlay.Id})");
        effectsManager.effects.Add(new()
        {
            obj = blackScreen,
            callback = LoadSceneGame,
            effect = VisualEffectsManager.Effects.MenuStartGame
        });
        blackScreen.SetActive(true);
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