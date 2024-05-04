using Assets.Resources.Scripts.Fight;
using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using static CardsManager;
using static FightManager;

public class GameManager : MonoBehaviour
{
    public FightManager FightManager { get; set; }
    public EventManager EventManager { get; set; }

    public GameUIManager gameUIManager;
    public EnemyManager enemyManager;
    public VisualEffectsManager effectsManager;

    public GameObject player;
    public GameObject enemyObj;

    public TextMeshProUGUI textBubble;
    public Button nextSectionButton;

    public GameObject choicesObj;

    PlayerData playerData;
    Map currentMap;

    public GameStatus Status { get; set; }
    public int CurrentEncounterCount { get; set; } = -1;

    // Start is called before the first frame update
    void Start()
    {
        playerData = SaveManager.LoadPlayerData();
        currentMap = JSONManager.GetFileFromJSON<MapData>(JSONManager.MAPS_PATH).Maps.Find(m => m.Id == playerData.CurrentRun.MapId);

        HandleNextEncounter();

        CharacterManager.LoadCharacter(playerData.CurrentRun.ClassId.ToString(), player);

        gameUIManager.SetPlayerSection(playerData.UnitData.Name, playerData.CurrentRun.ClassId.ToString(), playerData.UnitData.MaxHP, playerData.UnitData.CurrentHP, playerData.UnitData.Armor);
    }

    private void Update()
    {
        if (FightManager != null && FightManager.Player.status != CharacterStatus.Playing && FightManager.Enemy.status != CharacterStatus.Playing &&
            effectsManager.movingObjects.Count == 0)
            FightManager.HandleEndTurn();

        EventManager?.Update();
    }

    EncounterData GetEncounter(int encounterCount)
    {
        EncounterData encounter = currentMap.CustomEncounters.Find(e => e.PositionOnMap == encounterCount) ?? DrawRandomEncounter();
        return encounter;
    }

    EncounterData DrawRandomEncounter()
    {
        int index = UnityEngine.Random.Range(0, currentMap.EncounterList.Count);
        return currentMap.EncounterList[index];
    }

    void PlayEncounter(EncounterData encounter)
    {
        nextSectionButton.gameObject.SetActive(false);

        switch (encounter.Type)
        {

            case Map.TypeOfEncounter.Combat:
                EnemyList enemyList = JSONManager.GetFileFromJSON<EnemyList>(JSONManager.ENEMIES_PATH);
                EnemyData enemy = enemyList.Enemies.Find(e => e.Id == encounter.Id);
                PlayCombat(enemy);

                SetupBlackScreen(() => { });
                break;

            case Map.TypeOfEncounter.Event:
                PlayEvent(encounter.Id);

                SetupBlackScreen(() => { });
                break;
        }
    }

    void PlayCombat(EnemyData enemy)
    {
        enemy.BaseDecklist = enemy.IsCustomDecklist ? GetStartingDeck(0) : GetStartingDeck(0);
        FightManager = new(enemy, playerData.CurrentRun.CardList, playerData.UnitData, playerData.CurrentRun.ClassId, gameUIManager, effectsManager, enemyManager, player, enemyObj, this);

        Status = GameStatus.Fight;
    }

    void PlayEvent(int eventData){

        EventManager = new(eventData, enemyObj, effectsManager, textBubble, gameUIManager, this, choicesObj);

        gameUIManager.SetupEventUI();

        Status = GameStatus.Event;
    }

    /*EnemyData GetRandomEnemy(Map currentMap, EnemyList enemyList)
    {
        List<int> listOfEncounters = currentMap.EncounterList.Where(w => w.Type == Map.TypeOfEncounter.Combat).Select(e => e.Id).ToList();
        int enemyIndexSelected = UnityEngine.Random.Range(0, listOfEncounters.Count);
        int enemyId = listOfEncounters[enemyIndexSelected];

        return enemyList.Enemies.Find(e => e.Id == enemyId);
    }*/

    void SetupBlackScreen(Action callback)
    {
        gameUIManager.SetupBlackScreen(true, effectsManager, callback);
    }

    public static List<GameCard> GetStartingDeck(Classes classOfDeck)
    {
        List<GameCard> startingDeck = new();

        CardListData data = JSONManager.GetFileFromJSON<CardListData>(JSONManager.CARDS_PATH);
        foreach (var card in data.StartingCardList)
        {
            for (int i = 0; i < card.Quantities; i++)
            {
                startingDeck.Add(new()
                {
                    id = card.Id,
                    cardType = GetCardType(card.Id),
                    classId = classOfDeck,
                    value = card.Id
                });
            }
        }

        return startingDeck;
    }

    public static List<GameCard> CopyDeck(List<GameCard> deck)
    {
        return deck.Select(c => CopyCard(c)).ToList();
    }

    public static GameCard CopyCard(GameCard card)
    {
        return new GameCard()
        {
            cardType = card.cardType,
            classId = card.classId,
            id = card.id,
            value = card.value
        };
    }

    //Next section button click
    public void NextSectionButtonClick()
    {
        HandleNextEncounter();
    }

    void HandleNextEncounter()
    {
        CurrentEncounterCount++;

        if(currentMap.NumberOfEncounters ==  CurrentEncounterCount)
        {
            HandleGameVictory();
        }
        else
        {
            EncounterData nextEncounter = GetEncounter(CurrentEncounterCount);
            PlayEncounter(nextEncounter);
        }
    }

    public void HandleFightVictory()
    {
        nextSectionButton.gameObject.SetActive(true);
    }

    public void HandleFightDefeat()
    {
        Debug.Log("Fight lost");
        HandleGameDefeat();
    }

    public void HandleGameVictory()
    {
        SaveManager.TerminateSave();
        SceneManager.LoadScene(0);
    }

    public void HandleGameDefeat()
    {
        SaveManager.TerminateSave();
        SceneManager.LoadScene(0);
    }

    //Called by deck click in game
    public void PlayCard()
    {
        if (FightManager.IsGameOnStandby())
            return;

        FightManager.PlayUnitCard(Character.Player);
    }

    //Called by stand icon click in game
    public void Stand()
    {
        FightManager.HandlePlayerStand();
    }

    static CardType GetCardType(int cardId)
    {
        return (CardType)Enum.Parse(typeof(CardType), cardId.ToString());
    }

    public enum GameStatus
    {
        Default,
        Fight,
        Event
    }
}