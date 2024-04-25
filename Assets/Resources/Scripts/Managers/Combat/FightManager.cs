using Assets.Resources.Scripts.Fight;
using System.Collections.Generic;
using UnityEngine;
using static EnemyManager;

public class FightManager
{
    readonly GameUIManager gameUIManager;
    readonly AnimationManager animationManager;
    readonly EnemyManager enemyManager;

    public int BUST_PENALITY = 2;

    #region Player data
    public List<GameCard> PlayerCurrentDeck { get; set; }
    public List<GameCard> PlayerBaseDeck { get; set; }

    public int PlayerScore { get; set; }
    public int PlayerMaxScore { get; set; }
    public CharacterStatus PlayerStatus { get; set; } = CharacterStatus.Playing;
    #endregion

    #region Enemy data

    public EnemyCurrent Enemy { get { return enemyManager.Enemy; } set { enemyManager.Enemy = value; } }
    #endregion

    public TurnStatus CurrentTurn { get; set; }

    public FightManager(EnemyData enemy, List<GameCard> playerStartingDeck, UnitData unit, GameUIManager gameUIManager, AnimationManager animationManager, EnemyManager enemyManager)
    {
        PlayerBaseDeck = playerStartingDeck;
        PlayerCurrentDeck = playerStartingDeck;
        PlayerScore = 0;
        PlayerMaxScore = unit.MaxScore;

        CurrentTurn = TurnStatus.PlayerTurn;

        this.gameUIManager = gameUIManager;
        this.animationManager = animationManager;

        this.enemyManager = enemyManager;
        this.enemyManager.fightManager = this;

        Enemy = new(enemy);
    }

    public GameCard DrawAndPlayRandomCard(Character character)
    {
        List<GameCard> deck = new();

        switch (character)
        {
            case Character.Player:
                deck = PlayerCurrentDeck;
                break;
            case Character.Enemy:
                deck = Enemy.CurrentDeck;
                break;
        }

        int cardIndex = Random.Range(0, deck.Count);
        GameCard card = deck[cardIndex];
        deck.Remove(card);

        PlayCard(card, character);

        return card;
    }

    void PlayCard(GameCard card, Character character)
    {
        switch (character)
        {
            case Character.Player:
                int playerCardValue = GetCardValue(card);

                if (IsBust(PlayerScore, playerCardValue, PlayerMaxScore))
                {
                    PlayerStatus = CharacterStatus.Bust;
                    HandleBust(character, PlayerStatus);
                }
                else
                {
                    PlayerScore += playerCardValue;
                    PlayerStatus = UpdateStatus(PlayerScore, PlayerMaxScore);
                }

                break;
            case Character.Enemy:
                int enemyCardValue = GetCardValue(card);

                if(IsBust(Enemy.CurrentScore, enemyCardValue, Enemy.MaxScore))
                {
                    Enemy.Status = CharacterStatus.Bust;
                    HandleBust(character, Enemy.Status);
                }
                else
                {
                    Enemy.CurrentScore += enemyCardValue;
                    Enemy.Status = UpdateStatus(Enemy.CurrentScore, Enemy.MaxScore);
                }

                break;
        }
    }

    public bool IsBust(int currentScore, int valueToAdd, int unitMaxScore)
    {
        return currentScore + valueToAdd > unitMaxScore;
    }

    CharacterStatus UpdateStatus(int score, int maximumScore)
    {
        if (score == maximumScore)
            return CharacterStatus.Standing;

        if (score > maximumScore)
            return CharacterStatus.Bust;

        return CharacterStatus.Playing;
    }

    public void HandleBust(Character character, CharacterStatus status)
    {
        if (status != CharacterStatus.Bust)
            return;

        switch(character)
        {
            case Character.Player:
                PlayerScore -= BUST_PENALITY;
                if(PlayerScore < 0) 
                    PlayerScore = 0;
                break;
            case Character.Enemy:
                Enemy.CurrentScore -= BUST_PENALITY;
                if (Enemy.CurrentScore < 0)
                    Enemy.CurrentScore = 0;
                break;
        }
    }

    public int GetCardsBustAmount(Character character)
    {
        int bustAmount = 0;

        switch(character)
        {
            case Character.Player:
                bustAmount = CalculateBustAmount(PlayerCurrentDeck, PlayerScore, PlayerMaxScore);
                break;
            case Character.Enemy:
                bustAmount = CalculateBustAmount(Enemy.CurrentDeck, Enemy.CurrentScore, Enemy.MaxScore);
                break;
        }

        return bustAmount;
    }

    int CalculateBustAmount(List<GameCard> deck, int charCurrentScore, int charMaxScore)
    {
        int cardsBusting = 0;

        foreach (var card in deck)
        {
            int cardValue = GetCardValue(card);
            if (charCurrentScore + cardValue > charMaxScore)
                cardsBusting++;
        }

        return cardsBusting;
    }

    int GetCardValue(GameCard card)
    {
        int valueToAdd = 0;

        switch (card.cardType)
        {
            case CardType.Ace:
                valueToAdd = 0;
                break;
            case CardType.Number:
                valueToAdd = card.value;
                break;
            case CardType.Figure:
                valueToAdd = 0;
                break;
            case CardType.Special:
                break;
        }

        return valueToAdd;
    }

    public void PlayUnitCard(Character character)
    {
        switch (character)
        {
            case Character.Player:
                GameCard cardDrawn = DrawAndPlayRandomCard(Character.Player);
                if (PlayerStatus != CharacterStatus.Playing)
                    gameUIManager.DisableStandClick();
                gameUIManager.ShowCardDrawn(Character.Player, cardDrawn, animationManager, PlayerCardAnimationCallback);
                gameUIManager.UpdateStandUI(character, PlayerStatus, PlayerScore, PlayerMaxScore);
                gameUIManager.UpdatePlayerInfo(PlayerCurrentDeck.Count, GetCardsBustAmount(character));
                break;
            case Character.Enemy:
                GameCard enemyCardDrawn = DrawAndPlayRandomCard(Character.Enemy);
                gameUIManager.ShowCardDrawn(Character.Enemy, enemyCardDrawn, animationManager, EnemyCardAnimationCallback);
                gameUIManager.UpdateStandUI(character, Enemy.Status, Enemy.CurrentScore,Enemy.MaxScore);
                break;
        }
    }

    public bool IsGameOnStandby()
    {
        bool isUserOnStandby = PlayerStatus != CharacterStatus.Playing;
        bool isPlayerCardAnimating = animationManager.movingObjects.Exists(a => a.type == AnimationManager.MovingObject.TypeOfObject.CardDrawnPlayer);

        return isUserOnStandby || isPlayerCardAnimating || CurrentTurn == TurnStatus.EnemyTurn;
    }

    public void HandlePlayerStand()
    {
        if (IsGameOnStandby())
            return;

        PlayerStatus = CharacterStatus.Standing;
        gameUIManager.DisableStandClick();

        if (Enemy.Status == CharacterStatus.Playing)
            enemyManager.PlayEnemyTurn();
    }

    //Called after player drew their card
    void PlayerCardAnimationCallback()
    {
        if (Enemy.Status != CharacterStatus.Playing)
            return;

        CurrentTurn = TurnStatus.EnemyTurn;
        enemyManager.HandleEnemyTurn();
    }

    //Called after player drew their card
    void EnemyCardAnimationCallback()
    {
        if (PlayerStatus == CharacterStatus.Playing)
        {
            CurrentTurn = TurnStatus.PlayerTurn;
            return;
        }

        enemyManager.HandleEnemyTurn();
    }

    public enum Character
    {
        Player,
        Enemy
    }

    public enum CharacterStatus
    {
        Playing,
        Standing,
        Bust
    }

    public enum TurnStatus
    {
        PlayerTurn,
        EnemyTurn
    }
}