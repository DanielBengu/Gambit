using Assets.Resources.Scripts.Fight;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static FightManager;

public class GameManager : MonoBehaviour
{
    public GameUIManager gameUIManager;
    public FightManager fightManager;
    public AnimationManager animationManager;

    PlayerData playerData;
    Map currentMap;

    bool isEnemyTurn;

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
            Armor = 2,
            BaseMaxScore = 21,
            BaseStandThreshold = 16
        },playerData.CurrentRun.CardList);

        int bustAmount = fightManager.GetCardsBustAmount(Character.Player);
        gameUIManager.SetupUI(fightManager.Enemy, playerData.UnitData, playerData.CurrentRun.CardList.Count, bustAmount);

        isEnemyTurn = false;
    }

    public bool IsGameOnStandby()
    {
        bool isUserOnStandby = fightManager.PlayerStatus != CharacterStatus.Playing;
        bool isPlayerCardAnimating = animationManager.movingObjects.Exists(a => a.type == AnimationManager.MovingObject.TypeOfObject.CardDrawnPlayer);

        return isUserOnStandby || isPlayerCardAnimating || isEnemyTurn;
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
        if (IsGameOnStandby())
            return;

        PlayUnitCard(Character.Player);
    }

    public void PlayEnemyTurn()
    {
        PlayUnitCard(Character.Enemy);
    }

    public void PlayUnitCard(Character character)
    {
        switch (character)
        {
            case Character.Player:
                GameCard cardDrawn = fightManager.DrawAndPlayRandomCard(Character.Player);
                if (fightManager.PlayerStatus != CharacterStatus.Playing)
                    gameUIManager.DisableStandClick();
                gameUIManager.ShowCardDrawn(Character.Player, cardDrawn, animationManager, PlayerCardAnimationCallback);
                gameUIManager.UpdateStandUI(character, fightManager.PlayerScore, fightManager.PlayerMaxScore);
                gameUIManager.UpdatePlayerInfo(fightManager.PlayerCurrentDeck.Count, fightManager.GetCardsBustAmount(character));
                break;
            case Character.Enemy:
                GameCard enemyCardDrawn = fightManager.DrawAndPlayRandomCard(Character.Enemy);
                gameUIManager.ShowCardDrawn(Character.Enemy, enemyCardDrawn, animationManager, EnemyCardAnimationCallback);
                gameUIManager.UpdateStandUI(character, fightManager.EnemyScore, fightManager.EnemyMaxScore);
                break;
        }
    }

    //Called after player drew their card
    void PlayerCardAnimationCallback()
    {
        if (fightManager.EnemyStatus != CharacterStatus.Playing)
            return;

        isEnemyTurn = true;
        HandleEnemyTurn();
    }

    //Called after player drew their card
    void EnemyCardAnimationCallback()
    {
        if(fightManager.PlayerStatus == CharacterStatus.Playing)
        {
            isEnemyTurn = false;
            return;
        }

        HandleEnemyTurn();
    }

    void HandleEnemyTurn()
    {
        if (fightManager.EnemyStatus != CharacterStatus.Playing)
            return;

        //Player is bust or is standing, enemy keeps playing the turn until end
        PlayEnemyTurn();

        if (fightManager.EnemyStatus == CharacterStatus.Playing && fightManager.EnemyScore >= fightManager.EnemyThreshold)
        {
            fightManager.EnemyStatus = CharacterStatus.Standing;
        }

        if (fightManager.EnemyStatus != CharacterStatus.Playing)
            isEnemyTurn = false;
    }

    //Called by stand icon click in game
    public void Stand()
    {
        if (IsGameOnStandby())
            return;

        fightManager.PlayerStatus = CharacterStatus.Standing;
        gameUIManager.DisableStandClick();

        if (fightManager.EnemyStatus == CharacterStatus.Playing)
            PlayEnemyTurn();
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
