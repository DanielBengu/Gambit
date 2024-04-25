using Assets.Resources.Scripts.Fight;
using System.Collections.Generic;
using UnityEngine;

public class FightManager
{
    #region Player data
    public List<GameCard> PlayerCurrentDeck { get; set; }
    public List<GameCard> PlayerBaseDeck { get; set; }

    public int PlayerScore { get; set; }
    public int PlayerMaxScore { get; set; }
    public CharacterStatus PlayerStatus { get; set; } = CharacterStatus.Playing;
    #endregion

    #region Enemy data
    public Enemy Enemy { get; set; }

    public List<GameCard> EnemyCurrentDeck { get; set; }
    public List<GameCard> EnemyBaseDeck { get; set; }

    public int EnemyScore { get; set; }
    public int EnemyMaxScore { get; set; }
    public int EnemyThreshold { get; set; }
    public CharacterStatus EnemyStatus { get; set; } = CharacterStatus.Playing;
    #endregion

    public TurnStatus CurrentTurn { get; set; }

    public FightManager(Enemy enemy, List<GameCard> playerStartingDeck)
    {
        Enemy = enemy;

        PlayerBaseDeck = playerStartingDeck;
        PlayerCurrentDeck = playerStartingDeck;
        PlayerScore = 0;
        PlayerMaxScore = 21;

        EnemyBaseDeck = enemy.BaseDecklist;
        EnemyCurrentDeck = enemy.BaseDecklist;
        EnemyScore = 0;
        EnemyMaxScore = 21;
        EnemyThreshold = enemy.BaseStandThreshold;

        CurrentTurn = TurnStatus.PlayerTurn;
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
                deck = EnemyCurrentDeck;
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
                PlayerScore += GetCardValue(card, PlayerScore, PlayerMaxScore);
                PlayerStatus = UpdateStatus(PlayerScore, PlayerMaxScore);
                break;
            case Character.Enemy:
                EnemyScore += GetCardValue(card, EnemyScore, EnemyMaxScore);
                EnemyStatus = UpdateStatus(EnemyScore, EnemyMaxScore);
                break;
        }
    }

    CharacterStatus UpdateStatus(int score, int maximumScore)
    {
        if (score == maximumScore)
            return CharacterStatus.Standing;

        if (score > maximumScore)
            return CharacterStatus.Bust;

        return CharacterStatus.Playing;
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
                bustAmount = CalculateBustAmount(EnemyCurrentDeck, EnemyScore, EnemyMaxScore);
                break;
        }

        return bustAmount;
    }

    int CalculateBustAmount(List<GameCard> deck, int charCurrentScore, int charMaxScore)
    {
        int cardsBusting = 0;

        foreach (var card in deck)
        {
            int cardValue = GetCardValue(card, charCurrentScore, charMaxScore);
            if (charCurrentScore + cardValue > charMaxScore)
                cardsBusting++;
        }

        return cardsBusting;
    }

    int GetCardValue(GameCard card, int characterCurrentScore, int characterMaximumScore)
    {
        int valueToAdd = 0;

        switch (card.cardType)
        {
            case CardType.Ace:
                if (characterCurrentScore + 11 <= characterMaximumScore)
                    valueToAdd = 11;
                else
                    valueToAdd = 1;
                break;
            case CardType.Number:
                valueToAdd = card.value;
                break;
            case CardType.Figure:
                valueToAdd = 10;
                break;
            case CardType.Special:
                break;
        }

        return valueToAdd;
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