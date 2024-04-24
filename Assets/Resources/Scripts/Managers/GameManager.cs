using Assets.Resources.Scripts.Fight;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static FightManager;

public class GameManager : MonoBehaviour
{
    public GameUIManager gameUIManager;
    public FightManager fightManager;

    PlayerData playerData;
    Map currentMap;

    // Start is called before the first frame update
    void Start()
    {
        playerData = SaveManager.LoadPlayerData();
        currentMap = JSONManager.GetFileFromJSON<MapData>(JSONManager.MAPS_PATH).Maps.Find(m => m.Id == playerData.CurrentRun.MapId);

        fightManager = new(new()
        {
            Name = "Medusa",
            BaseDecklist = playerData.CurrentRun.CardList,
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
            Armor = 2
        },playerData.CurrentRun.CardList);

        int bustAmount = fightManager.GetCardsBustAmount(Character.Player);
        gameUIManager.SetupUI(fightManager.Enemy, playerData.CurrentRun.CardList.Count, bustAmount);
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
        if (fightManager.PlayerStatus != CharacterStatus.Playing)
            return;

        GameCard cardDrawn = fightManager.DrawAndPlayRandomCard(Character.Player);

        if (fightManager.PlayerStatus != CharacterStatus.Playing)
            gameUIManager.DisableStandClick();

        int updatedBustAmount = fightManager.GetCardsBustAmount(Character.Player);
        gameUIManager.ShowCardDrawn(cardDrawn);
        gameUIManager.UpdateUI(Character.Player, fightManager.PlayerScore, fightManager.PlayerMaxScore, fightManager.PlayerCurrentDeck.Count, updatedBustAmount, fightManager.PlayerStatus);
    }

    //Called by stand icon click in game
    public void Stand()
    {
        if (fightManager.PlayerStatus != CharacterStatus.Playing)
            return;

        fightManager.PlayerStatus = CharacterStatus.Standing;
        gameUIManager.DisableStandClick();
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
