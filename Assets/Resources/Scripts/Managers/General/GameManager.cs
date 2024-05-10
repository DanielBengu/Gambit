using Assets.Resources.Scripts.Fight;
using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using static AnimationManager;
using static CardsManager;
using static FightManager;
using static Map;

public class GameManager : MonoBehaviour
{
    public FightManager FightManager { get; set; }
    public EventManager EventManager { get; set; }

    public GameUIManager gameUIManager;
    public EnemyManager enemyManager;
    public VisualEffectsManager effectsManager;
    public LanguageManager languageManager;

    public Transform playerParent;
    public GameObject player;
    public Transform enemyParent;
    public GameObject enemy;

    public TextMeshProUGUI textBubble;
    public Button nextSectionButton;

    public GameObject choicesObj;
    public Transform choicesPositionObj;

    public PlayerData playerData;
    public EncounterData currentEncounter;
    Map currentMap;

    Action callbackFightVictory;

    public GameStatus Status { get; set; }
    public int CurrentEncounterCount { get; set; } = -1;

    void Start()
    {
        playerData = SaveManager.LoadPlayerData();
        currentMap = JSONManager.GetFileFromJSON<MapData>(JSONManager.MAPS_PATH).Maps.Find(m => m.Id == playerData.CurrentRun.MapId);

        if (!playerData.CurrentRun.IsOngoing)
        {
            Debug.LogError("START FIGHT - NO RUN FOUND, BACK TO INTRO");
            SceneManager.LoadScene(0);
            return;
        }
            

        string className = playerData.CurrentRun.ClassId.ToString();
        player = LoadCharacter(className, playerParent);

        gameUIManager.SetPlayerSection(playerData.UnitData.Name, playerData.CurrentRun.ClassId.ToString(), playerData.UnitData.MaxHP, playerData.UnitData.CurrentHP, playerData.UnitData.Armor);

        HandleNextEncounter();
    }

    private void Update()
    {
        if (FightManager != null && FightManager.Player.status != CharacterStatus.Playing && FightManager.Enemy.status != CharacterStatus.Playing &&
            effectsManager.movingObjects.Count == 0)
            FightManager.HandleEndTurn();

        EventManager?.Update();
    }

    EncounterData GetEncounter(int encounterCount, EncounterData previousEncounter)
    {
        EncounterData encounter = currentMap.CustomEncounters.Find(e => e.PositionOnMap == encounterCount) ?? DrawRandomEncounter(previousEncounter);
        return encounter;
    }

    EncounterData DrawRandomEncounter(EncounterData previousEncounter)
    {
        TypeOfEncounter encounterTypeToAvoid = previousEncounter != null ? previousEncounter.Type : TypeOfEncounter.Default;
        List<EncounterData> possibleEncounters = currentMap.EncounterList.Where(e => e.Type != encounterTypeToAvoid).ToList();
        int index = UnityEngine.Random.Range(0, possibleEncounters.Count);
        return possibleEncounters[index];
    }

    public static GameObject LoadCharacter(string charName, Transform character)
    {
        GameObject characterPrefab = CharacterManager.LoadCharacter(charName);
        return Instantiate(characterPrefab, characterPrefab.transform.position, character.rotation, character);
    }

    void PlayEncounter(EncounterData encounter)
    {
        nextSectionButton.gameObject.SetActive(false);

        switch (encounter.Type)
        {

            case TypeOfEncounter.Combat:
                EnemyList enemyList = JSONManager.GetFileFromJSON<EnemyList>(JSONManager.ENEMIES_PATH);
                EnemyData enemyData = enemyList.Enemies.Find(e => e.Id == encounter.Id);

                enemy = LoadCharacter(enemyData.Name, enemyParent);

                PlayCombat(enemyData, SetNextSectionButtonClick);
                PlayAnimation(enemy, SpriteAnimation.UnitIntro, FightManager.SetupFightUI);

                SetupBlackScreen(() => { });
                break;

            case TypeOfEncounter.Event:
                PlayEvent(encounter.Id);

                SetupBlackScreen(() => { });
                break;
        }
    }

    public void PlayCombat(EnemyData enemyData, Action callback)
    {
        callbackFightVictory = callback;

        enemyData.BaseDecklist = enemyData.IsCustomDecklist ? GetStartingDeck(0) : GetStartingDeck(0);
        FightManager = new(enemyData, playerData.CurrentRun.CardList, playerData.UnitData, playerData.CurrentRun.ClassId, gameUIManager, effectsManager, enemyManager, player, enemy, this);

        Status = GameStatus.Fight;
    }

    void PlayEvent(int eventData){

        EventManager = new(eventData, enemy, enemyParent, effectsManager, textBubble, gameUIManager, this, choicesObj, languageManager, choicesPositionObj);

        gameUIManager.SetupEventUI();

        Status = GameStatus.Event;
    }

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

    void HandleNextEncounter()
    {
        CurrentEncounterCount++;

        if(currentMap.NumberOfEncounters ==  CurrentEncounterCount)
        {
            HandleGameVictory();
        }
        else
        {
            currentEncounter = GetEncounter(CurrentEncounterCount, currentEncounter);
            PlayEncounter(currentEncounter);
        }
    }

    public void MakePlayerDie()
    {
        PlayAnimation(playerParent.gameObject, SpriteAnimation.UnitDeath, HandleGameDefeat);
    }

    public void SetNextSectionButtonClick()
    {
        nextSectionButton.gameObject.SetActive(true);
    }

    #region Handle Game Status

    public void HandleFightVictory()
    {
        callbackFightVictory();
    }

    public void HandleFightDefeat()
    {
        Debug.Log("Fight lost");
        HandleGameDefeat();
    }

    public void HandleEventVictory()
    {
        SetNextSectionButtonClick();
    }

    public void HandleEventDefeat()
    {
        Debug.Log("Event lost");
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

    #endregion

    #region Button clicks

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

    //Next section button click
    public void NextSectionButtonClick()
    {
        HandleNextEncounter();
    }

    #endregion

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