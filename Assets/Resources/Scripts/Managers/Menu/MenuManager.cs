using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using static PlayerPrefsManager;

public class MenuManager : MonoBehaviour
{
    public bool forceTutorial = false;
    public MenuOptions menuOptions;

    void Start()
    {
        FirstGameStart();

        menuOptions.StartCardAnimation();

        HandleSaves();
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
        if (GetPref<int>(PlayerPrefsEnum.HasWonAnyRun) != 1)
            menuOptions.CoverCard(MenuOptions.CardPosition.Right);

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
        int classId = -1;
        if (!DoesPrefExists(PlayerPrefsEnum.AlreadyLaunchedGame) || forceTutorial)
        {
            mapToPlay = mapList.Maps.Find(m => m.Id == 0); //Tutorial world
            classId = 0; //Tutorial starts with warrior
            SetPref(PlayerPrefsEnum.AlreadyLaunchedGame, 1);
        }
        else
        {
            mapToPlay = StartRandomMap(mapList);
            classId = 0; //To change with class selected
        }

        SaveManager.SavePlayerData(new()
        {
            UnitData = new()
            {
                Name = "TEMP",
                Armor = 2,
                MaxHP = 10,
                CurrentHP = 10,
                MaxScore = 12
            },
            CurrentRun = new()
            {
                IsOngoing = true,
                MapId = mapToPlay.Id,
                ClassId = classId,
                CurrentFloor = 1,
                CardList = GameManager.GetStartingDeck(classId)
            }
        });

        Debug.Log($"Launching world {mapToPlay.Name} (id: {mapToPlay.Id})");
        SceneManager.LoadScene("Game");
    }

    Map StartRandomMap(MapData mapData)
    {
        List<Map> mapListWithoutTutorial = mapData.Maps.Where(m => m.Id != 0).ToList();

        int mapIndex = Random.Range(0, mapListWithoutTutorial.Count);

        return mapListWithoutTutorial[mapIndex];
    }
}