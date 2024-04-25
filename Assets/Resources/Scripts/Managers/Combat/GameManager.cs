using Assets.Resources.Scripts.Fight;
using System.Collections.Generic;
using UnityEngine;
using static FightManager;

public class GameManager : MonoBehaviour
{
    public GameUIManager gameUIManager;
    public FightManager fightManager;
    public EnemyManager enemyManager;
    public EffectsManager animationManager;

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
            Name = "Medusa",
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
            HP = 10,
            Armor = 2,
            BaseMaxScore = 12,
            BaseStandThreshold = 8
        };
        fightManager = new(enemy, playerData.CurrentRun.CardList, playerData.UnitData, gameUIManager, animationManager, enemyManager, player, enemyObj);

        int bustAmount = fightManager.GetCardsBustAmount(Character.Player);
        gameUIManager.SetupUI(fightManager.Enemy, playerData.UnitData, playerData.CurrentRun.CardList.Count, bustAmount);
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
