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
    public UnlockManager UnlockManager { get; set; }

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

    #region Unlock section

    public GameObject unlockParent;

    #endregion

    public GameStatus Status { get; set; }
    public int CurrentEncounterCount { get; set; } = -1;

    void Start()
    {
        LoadData();

        if (!playerData.CurrentRun.IsOngoing)
        {
            Debug.LogError("START FIGHT - NO RUN FOUND, BACK TO INTRO");
            SceneManager.LoadScene(0);
            return;
        }

        LoadGameSetup();
    }

    void LoadData()
    {
        playerData = SaveManager.LoadPlayerData();
        currentMap = JSONManager.GetFileFromJSON<MapData>(JSONManager.MAPS_PATH).Maps.Find(m => m.Id == playerData.CurrentRun.MapId);
        UnlockManager = new(unlockParent);
    }

    void LoadGameSetup()
    {
        string className = playerData.CurrentRun.ClassId.ToString();
        player = LoadCharacter(className, playerParent);

        gameUIManager.SetPlayerSection(playerData.UnitData.Name, playerData.CurrentRun.ClassId.ToString(), playerData.UnitData.MaxHP, playerData.UnitData.CurrentHP, playerData.UnitData.Armor);
        gameUIManager.UpdateGoldAmount(playerData.CurrentRun.GoldAmount);

        CurrentEncounterCount = playerData.CurrentRun.CurrentFloor;
        HandleNextEncounter();
        SetupBlackScreen(TurnOffBlackScreen, VisualEffectsManager.Effects.LightenBlackScreen);
    }

    private void Update()
    {
        if (FightManager != null && FightManager.Player.status != CharacterStatus.Playing && FightManager.Enemy.status != CharacterStatus.Playing &&
            effectsManager.movingObjects.Count == 0)
            FightManager.HandleEndTurn();

        EventManager?.Update();
    }

    public void LoadEnemy(GameObject enemy)
    {
        this.enemy = enemy;
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
                PlayAnimation(enemy, SpriteAnimation.UnitIntro, FightManager.SetupFightUIAndStartGame);
                break;

            case TypeOfEncounter.Event:
                PlayEvent(encounter.Id);
                break;
        }
    }

    public void PlayCombat(EnemyData enemyData, Action callback)
    {
        callbackFightVictory = callback;

        enemyData.BaseDecklist = enemyData.IsCustomDecklist ? GetStartingDeck(0) : GetStartingDeck(0);
        FightManager = new(enemyData, playerData.CurrentRun.CardList, playerData.CurrentRun.ActionDeck, playerData.UnitData, playerData.CurrentRun.ClassId, gameUIManager, effectsManager, enemyManager, player, enemy, this);

        Status = GameStatus.Fight;
    }

    void PlayEvent(int eventData){

        EventManager = new(eventData, enemyParent, enemy, effectsManager, textBubble, gameUIManager, this, choicesObj, languageManager, choicesPositionObj);

        gameUIManager.SetupEventUI();

        Status = GameStatus.Event;
    }

    void SetupBlackScreen(Action callback, VisualEffectsManager.Effects screen)
    {
        gameUIManager.SetupBlackScreen(true, screen, effectsManager, callback);
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
                    cardType = GetCardType<CardType>(card.Id),
                    classId = classOfDeck,
                    value = card.Id
                });
            }
        }

        return startingDeck;
    }

    public static List<ActionCard> GetStartingActionDeck(Classes classOfDeck)
    {
        List<ActionCard> startingDeck = new();

        CardListData data = JSONManager.GetFileFromJSON<CardListData>(JSONManager.CARDS_PATH);
        var cardData = GetActionCardList(classOfDeck, data);
        foreach (var card in cardData)
        {
            for (int i = 0; i < card.Quantities; i++)
            {
                ActionCard cardToAdd = ConvertIdIntoCard(card.Id);
                if(cardToAdd != null)
                    startingDeck.Add(cardToAdd);
            }
        }

        return startingDeck;
    }

    public static List<Card> GetActionCardList(Classes classOfDeck, CardListData cardData)
    {
        return classOfDeck switch
        {
            Classes.Warrior => cardData.WarriorActionCardList,
            _ => cardData.WarriorActionCardList,
        };
    }

    public static ActionCard ConvertIdIntoCard(int id)
    {
        return id switch
        {
            0 => new()
            {
                Id = 0,
                NameIdValue = 32,
                DescriptionIdValue = 38,
                ActionId = ActionType.Equip,
                ClassId = Classes.Warrior
            },
            1 => new()
            {
                Id = 1,
                NameIdValue = 33,
                DescriptionIdValue = 39,
                ActionId = ActionType.Attack,
                ClassId = Classes.Warrior
            },
            2 => new()
            {
                Id = 2,
                NameIdValue = 34,
                DescriptionIdValue = 40,
                ActionId = ActionType.Skill,
                ClassId = Classes.Warrior
            },
            3 => new()
            {
                Id = 3,
                NameIdValue = 36,
                DescriptionIdValue = 41,
                ActionId = ActionType.Modifier,
                ClassId = Classes.Basic
            },
            4 => new()
            {
                  Id = 4,
                  NameIdValue = 37,
                  DescriptionIdValue = 42,
                  ActionId = ActionType.Modifier,
                  ClassId = Classes.Basic
            },
            _ => null
        };
    }

    public static List<GameCard> CopyDeck(List<GameCard> deck)
    {
        return deck.Select(c => CopyCard(c)).ToList();
    }

    public static List<ActionCard> CopyDeck(List<ActionCard> deck)
    {
        return deck.Select(c => CopyCard(c)).ToList();
    }

    public void StartMessage()
    {
        SetupBlackScreen(() => { }, VisualEffectsManager.Effects.DarkenBlackScreen);
    }

    public static GameCard CopyCard(GameCard card)
    {
        return new GameCard()
        {
            cardType = card.cardType,
            classId = card.classId,
            id = card.id,
            value = card.value,
            destroyOnPlay = true
        };
    }

    public static ActionCard CopyCard(ActionCard card)
    {
        return new ActionCard()
        {
            ActionId = card.ActionId,
            ClassId = card.ClassId,
            Id = card.Id,
            NameIdValue = card.NameIdValue,
            DescriptionIdValue = card.DescriptionIdValue,
        };
    }

    void TurnOffBlackScreen()
    {
        gameUIManager.blackScreen.gameObject.SetActive(false);
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

    public void HandleFightVictory(List<Reward> rewards)
    {
        foreach(Reward reward in rewards)
            if (reward.reward == TypeOfReward.Gold)
                playerData.CurrentRun.GoldAmount += reward.amount;

        gameUIManager.UpdateGoldAmount(playerData.CurrentRun.GoldAmount);

        HandleEncounterVictory();

        callbackFightVictory();
    }

    public void HandleEncounterVictory()
    {
        playerData.CurrentRun.CurrentFloor = CurrentEncounterCount;
        SaveManager.SavePlayerData(playerData);
    }

    public void HandleFightDefeat()
    {
        Debug.Log("Fight lost");
        HandleGameDefeat();
    }

    public void HandleEventVictory()
    {
        HandleEncounterVictory();
        SetNextSectionButtonClick();
    }

    public void HandleEventDefeat()
    {
        Debug.Log("Event lost");
        HandleGameDefeat();
    }

    public void HandleGameVictory()
    {
        SetupBlackScreen(UnlockCard, VisualEffectsManager.Effects.DarkenBlackScreen);
    }

    void UnlockCard()
    {
        UnlockManager.ManageUnlocksByMapCompletion(playerData, currentMap, TerminateAndReturnToMenu);
    }

    void TerminateAndReturnToMenu()
    {
        SaveManager.TerminateSave();
        SceneManager.LoadScene(0);
    }

    public void HandleGameDefeat()
    {
        TerminateAndReturnToMenu();
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

    static T GetCardType<T>(int cardId)
    {
        return (T)Enum.Parse(typeof(T), cardId.ToString());
    }

    public enum GameStatus
    {
        Default,
        Fight,
        Event
    }
}