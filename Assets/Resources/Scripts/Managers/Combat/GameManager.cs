using Assets.Resources.Scripts.Fight;
using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEditor;
using UnityEngine;
using static FightManager;

public class GameManager : MonoBehaviour
{
    public FightManager FightManager { get; set; }
    public EventManager EventManager { get; set; }
    public DialogueManager DialogueManager { get; set; } = new();

    public GameUIManager gameUIManager;
    public EnemyManager enemyManager;
    public EffectsManager effectsManager;

    public GameObject player;
    public GameObject enemyObj;

    public TextMeshProUGUI textBubble;

    PlayerData playerData;
    Map currentMap;

    public GameStatus Status { get; set; }
    public int CurrentEncounterCount { get; set; } = 0;

    // Start is called before the first frame update
    void Start()
    {
        playerData = SaveManager.LoadPlayerData();
        currentMap = JSONManager.GetFileFromJSON<MapData>(JSONManager.MAPS_PATH).Maps.Find(m => m.Id == playerData.CurrentRun.MapId);

        EncounterData encounter = GetEncounter(CurrentEncounterCount);
        PlayEncounter(encounter);
    }

    private void Update()
    {
        if (FightManager != null && (FightManager.PlayerStatus != CharacterStatus.Playing && FightManager.Enemy.Status != CharacterStatus.Playing))
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
        switch (encounter.Type)
        {

            case Map.TypeOfEncounter.Combat:
                EnemyList enemyList = JSONManager.GetFileFromJSON<EnemyList>(JSONManager.ENEMIES_PATH);
                EnemyData enemy = enemyList.Enemies.Find(e => e.Id == encounter.Id);
                PlayCombat(enemy);

                SetupBlackScreen(() => { });
                break;

            case Map.TypeOfEncounter.Event:
                EventList eventList = JSONManager.GetFileFromJSON<EventList>(JSONManager.EVENTS_PATH);
                EventData eventData = eventList.Events.Find(e => e.Id == encounter.Id);
                PlayEvent(eventData);

                SetupBlackScreen(EventManager.CharacterTalk);
                break;
        }
    }

    void PlayCombat(EnemyData enemy)
    {
        enemy.BaseDecklist = enemy.IsCustomDecklist ? GetStartingDeck(0) : GetStartingDeck(0);
        FightManager = new(enemy, playerData.CurrentRun.CardList, playerData.UnitData, gameUIManager, effectsManager, enemyManager, player, enemyObj, this);

        int bustAmount = FightManager.GetCardsBustAmount(Character.Player);
        gameUIManager.SetupFightUI(FightManager.Enemy, playerData.UnitData, playerData.CurrentRun.CardList.Count, bustAmount);

        Status = GameStatus.Fight;
    }

    void PlayEvent(EventData eventData){

        EventManager = new(eventData, enemyObj, effectsManager, textBubble);

        gameUIManager.SetupEventUI();

        Status = GameStatus.Event;
    }

    EnemyData GetRandomEnemy(Map currentMap, EnemyList enemyList)
    {
        List<int> listOfEncounters = currentMap.EncounterList.Where(w => w.Type == Map.TypeOfEncounter.Combat).Select(e => e.Id).ToList();
        int enemyIndexSelected = UnityEngine.Random.Range(0, listOfEncounters.Count);
        int enemyId = listOfEncounters[enemyIndexSelected];

        return enemyList.Enemies.Find(e => e.Id == enemyId);
    }

    void SetupBlackScreen(Action callback)
    {
        gameUIManager.SetupBlackScreen(true, effectsManager, callback);
    }

    public static List<GameCard> GetStartingDeck(int classOfDeck)
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

    public void HandleFightDefeat()
    {

    }

    public void HandleFightVictory()
    {

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
        if (cardId == 0)
            return CardType.Ace;

        if (cardId > 0 && cardId < 11)
            return CardType.Number;

        if (cardId >= 11 && cardId <= 13)
            return CardType.Figure;

        if (cardId > 13)
            return CardType.Special;

        Debug.LogError($"Incorrect card id: {cardId}");
        return CardType.Default;
    }

    public enum GameStatus
    {
        Default,
        Fight,
        Event
    }
}
