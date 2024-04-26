using Assets.Resources.Scripts.Fight;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static FightManager;

public class GameManager : MonoBehaviour
{
    public GameUIManager gameUIManager;
    public FightManager fightManager;
    public EnemyManager enemyManager;
    public EffectsManager effectsManager;

    public GameObject player;
    public GameObject enemyObj;

    PlayerData playerData;
    Map currentMap;

    // Start is called before the first frame update
    void Start()
    {
        playerData = SaveManager.LoadPlayerData();
        currentMap = JSONManager.GetFileFromJSON<MapData>(JSONManager.MAPS_PATH).Maps.Find(m => m.Id == playerData.CurrentRun.MapId);
        EnemyData enemy = new()
        {
            Name = "Skeleton",
            BaseDecklist = GetStartingDeck(0),
            Rewards = new()
            {
                new()
                {
                    reward = TypeOfReward.Gold,
                    rewardId = 0,
                    amount = 500
                }
            },
            HP = 1,
            Armor = 1,
            BaseMaxScore = 12,
            BaseStandThreshold = 8
        };
        fightManager = new(enemy, playerData.CurrentRun.CardList, playerData.UnitData, gameUIManager, effectsManager, enemyManager, player, enemyObj, this);

        int bustAmount = fightManager.GetCardsBustAmount(Character.Player);
        gameUIManager.SetupUI(fightManager.Enemy, playerData.UnitData, playerData.CurrentRun.CardList.Count, bustAmount);

        SetupBlackScreen();
    }

    void SetupBlackScreen()
    {
        gameUIManager.SetupBlackScreen(true, effectsManager);
    }

    private void Update()
    {
        if (fightManager.PlayerStatus != CharacterStatus.Playing && fightManager.Enemy.Status != CharacterStatus.Playing)
            fightManager.HandleEndTurn();
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
        if (fightManager.IsGameOnStandby())
            return;

        fightManager.PlayUnitCard(Character.Player);
    }

    //Called by stand icon click in game
    public void Stand()
    {
        fightManager.HandlePlayerStand();
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
}
