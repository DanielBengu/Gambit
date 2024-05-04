using Assets.Resources.Scripts.Fight;
using System.Collections.Generic;
using UnityEngine;
using static EnemyManager;
using static AnimationManager;
using System;
using static CardsManager;
public class FightManager
{
    public static int CRIT_SCORE = 12;
    readonly GameManager gameManager;
    readonly GameUIManager gameUIManager;
    readonly VisualEffectsManager effectsManager;
    readonly EnemyManager enemyManager;

    public int BUST_PENALITY = 2;

    public GameObject playerObj;
    public GameObject enemyObj;

    public List<int> spritesAnimating = new();

    public FightUnit Player { get; set; }
    public FightUnit Enemy { get { return enemyManager.Enemy; } set { enemyManager.Enemy = value; } }

    public TurnStatus CurrentTurn { get; set; }

    public FightManager(EnemyData enemy, List<GameCard> playerStartingDeck, UnitData unit, Classes playerClass, GameUIManager gameUIManager, VisualEffectsManager effectsManager, EnemyManager enemyManager, GameObject player, GameObject enemyObj, GameManager gameManager)
    {
        this.enemyManager = enemyManager;
        this.enemyManager.fightManager = this;

        Enemy = ConvertEnemyIntoUnit(enemy);

        Player = new(unit, true, playerClass, GameManager.CopyDeck(playerStartingDeck), GameManager.CopyDeck(playerStartingDeck));

        CurrentTurn = TurnStatus.IntermediaryEffects;

        this.gameUIManager = gameUIManager;
        this.effectsManager = effectsManager;

        playerObj = player;
        this.enemyObj = enemyObj;

        this.gameManager = gameManager;

        CharacterManager.LoadCharacter(enemy.Name, enemyObj);

        PlayAnimation(enemyObj, SpriteAnimation.UnitIntro, SetupFightUI, effectsManager);
    }

    void SetupFightUI()
    {
        int bustAmount = GetCardsBustAmount(Player.FightCurrentDeck, Player.currentScore, Player.MaxScore);
        gameUIManager.SetupFightUI(Enemy, Player, Player.FightCurrentDeck.Count, bustAmount);
    }

    FightUnit ConvertEnemyIntoUnit(EnemyData enemy)
    {
        UnitData unit = new()
        {
            Name = enemy.Name,
            Armor = enemy.Armor,
            CurrentHP = enemy.HP,
            MaxHP = enemy.HP,
            MaxScore = enemy.BaseMaxScore
        };

        return new(unit, false, enemy.UnitClass, GameManager.CopyDeck(enemy.BaseDecklist), GameManager.CopyDeck(enemy.BaseDecklist), enemy.BaseStandThreshold);
    }

    public void HandleEndTurn()
    {
        int pointDifference = Mathf.Abs(Player.currentScore - Enemy.currentScore);

        if (Player.currentScore > Enemy.currentScore)
        {

            int playerDamage = CalculateDamage(Player, Enemy, pointDifference);
            DealDamage(Enemy, playerDamage);
            MakeUnitPerformAnimation(Character.Player, SpriteAnimation.UnitDealingDamage);

        } else if(Player.currentScore < Enemy.currentScore)
        {
            int enemyDamage = CalculateDamage(Enemy, Player, pointDifference);
            DealDamage(Player, enemyDamage);
            MakeUnitPerformAnimation(Character.Enemy, SpriteAnimation.UnitDealingDamage);
        }      

        ResetTurn();
    }

    int CalculateDamage(FightUnit attacker, FightUnit defender, int baseDamage)
    {
        int attackerDamage = attacker.ApplyDamageModifiers(baseDamage);
        int finalDamageAmount = defender.ApplyDefenceModifiers(attackerDamage);

        return finalDamageAmount;
    }

    public Character GetCharacterFromGameObjectID(int gameObjectID)
    {
        if (gameObjectID == playerObj.GetInstanceID())
            return Character.Player;

        if(gameObjectID == enemyObj.GetInstanceID())
            return Character.Enemy;

        return Character.Default;
    }

    void DealDamage(FightUnit unitTakingDamage, int amount)
    {
        unitTakingDamage.FightHP -= amount;
    }

    public Action GetAnimationCallback(SpriteAnimation animation)
    {
        return animation switch
        {
            SpriteAnimation.UnitTakingDamage => EmptyMethod,
            SpriteAnimation.UnitDealingDamage => EmptyMethod,
            SpriteAnimation.UnitIdle => EmptyMethod,
            SpriteAnimation.UnitDeath => EmptyMethod,
            _ => EmptyMethod,
        };
    }

    public SpriteAnimation GetDamageAnimation(Character character)
    {
        int unitHp = character == Character.Player ? Player.FightHP : Enemy.FightHP;

        if (unitHp <= 0)
            return SpriteAnimation.UnitDeath;
        else
            return SpriteAnimation.UnitTakingDamage;
    }

    public void MakeUnitPerformAnimation(Character character, SpriteAnimation animation)
    {
        Action callback = GetAnimationCallback(animation);

        switch (character)
        {
            case Character.Player:
                PlayAnimation(playerObj, animation, callback, effectsManager);
                break;
            case Character.Enemy:
                PlayAnimation(enemyObj, animation, callback, effectsManager);
                break;
        }
    }

    public void HandleUnitDeath(Character character)
    {
        switch (character)
        {
            case Character.Enemy:
                CharacterManager.ResetCharacter(enemyObj, effectsManager);
                gameUIManager.TurnOfFightUI();
                gameManager.HandleFightVictory();
                break;
            case Character.Player:
                gameManager.HandleFightDefeat();
                break;
        }
    }

    void EmptyMethod()
    {

    }

    void ResetTurn()
    {
        ResetUnitTurn(Player);
        ResetUnitTurn(Enemy);

        CurrentTurn = TurnStatus.PlayerTurn;

        gameUIManager.UpdateUI(Character.Enemy, Enemy);
        gameUIManager.UpdateUI(Character.Player, Player);

        gameUIManager.SetStandButtonInteractable(true);
    }

    void ResetUnitTurn(FightUnit unit)
    {
        unit.TickModifiers();
        unit.currentScore = 0;
        unit.status = CharacterStatus.Playing;
    }

    public void RemoveObjFromAnimationList(GameObject obj)
    {
        effectsManager.RemoveFromLists(obj);
    }

    public GameCard DrawAndPlayRandomCard(Character character)
    {
        List<GameCard> deck = new();
        switch (character)
        {
            case Character.Player:
                if (Player.FightCurrentDeck.Count == 0)
                    ResetDeck(character);
                deck = Player.FightCurrentDeck;
                break;
            case Character.Enemy:
                if (Enemy.FightCurrentDeck.Count == 0)
                    ResetDeck(character);
                deck = Enemy.FightCurrentDeck;
                break;
        }

        GameCard cardPlayed = GetRandomCard(deck);
        PlayCard(cardPlayed, character);

        return cardPlayed;
    }

    public GameCard GetRandomCard(List<GameCard> deck)
    {
        int cardIndex = UnityEngine.Random.Range(0, deck.Count);
        return deck[cardIndex];
    }

    void PlayCard(GameCard card, Character character)
    {
        switch (character)
        {
            case Character.Player:
                int playerCardValue = GetCardPointValue(card);

                HandlePoints(ref Player.status, character, ref Player.currentScore, Player.CurrentMaxScore, playerCardValue);
                HandleSideEffects(character, Player.Class, card);
                Player.FightCurrentDeck.Remove(card);
                break;
            case Character.Enemy:
                int enemyCardValue = GetCardPointValue(card);

                HandlePoints(ref Enemy.status, character, ref Enemy.currentScore, Enemy.CurrentMaxScore, enemyCardValue);
                HandleSideEffects(character, Enemy.Class, card);

                Enemy.FightCurrentDeck.Remove(card);
                break;
        }
    }

    public void HandlePoints(ref CharacterStatus status, Character character, ref int unitScore, int unitMaxScore, int playerCardValue)
    {
        if (IsBust(unitScore, playerCardValue, unitMaxScore))
        {
            status = CharacterStatus.Bust;
            HandleBust(character, status);
        }
        else
        {
            unitScore += playerCardValue;
            status = UpdateStatus(unitScore, unitMaxScore);
        }
    }

    public void HandleSideEffects(Character character, IClass unitClass, GameCard card)
    {
        switch (character)
        {
            case Character.Player:
                unitClass.PlayCardEffect(card.cardType, Player, Enemy);
                break;
            case Character.Enemy:
                unitClass.PlayCardEffect(card.cardType, Enemy, Player);
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
                Player.currentScore -= BUST_PENALITY;
                if(Player.currentScore < 0)
                    Player.currentScore = 0;
                break;
            case Character.Enemy:
                Enemy.currentScore -= BUST_PENALITY;
                if (Enemy.currentScore < 0)
                    Enemy.currentScore = 0;
                break;
        }
    }

    public static int GetCardsBustAmount(List<GameCard> deck, int currentScore, int maxScore)
    {
        int bustAmount = CalculateBustAmount(deck, currentScore, maxScore);

        return bustAmount;
    }

    public void ResetDeck(Character character)
    {
        switch (character)
        {
            case Character.Player:
                Player.FightCurrentDeck = GameManager.CopyDeck(Player.FightBaseDeck);
                break;
            case Character.Enemy:
                Enemy.FightCurrentDeck = GameManager.CopyDeck(Enemy.FightBaseDeck);
                break;
        }
    }

    static int CalculateBustAmount(List<GameCard> deck, int charCurrentScore, int charMaxScore)
    {
        int cardsBusting = 0;

        foreach (var card in deck)
        {
            int cardValue = GetCardPointValue(card);
            if (charCurrentScore + cardValue > charMaxScore)
                cardsBusting++;
        }

        return cardsBusting;
    }

    static int GetCardPointValue(GameCard card)
    {
        int valueToAdd = 0;

        switch (card.cardType)
        {
            case CardType.Ace:
                valueToAdd = 0;
                break;
            case CardType.One:
            case CardType.Two:
            case CardType.Three:
            case CardType.Four:
            case CardType.Five:
            case CardType.Six:
                valueToAdd = card.value;
                break;
            case CardType.Jack:
                valueToAdd = 0;
                break;
            case CardType.Queen:
                valueToAdd = 0;
                break;
            case CardType.King:
                valueToAdd = 0;
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
                if (Player.status != CharacterStatus.Playing)
                    gameUIManager.SetStandButtonInteractable(false);
                gameUIManager.ShowCardDrawn(Character.Player, cardDrawn, Player.Class, effectsManager, PlayerCardAnimationCallback);
                gameUIManager.UpdateStandUI(character, Player.status, Player.currentScore, Player.CurrentMaxScore);
                gameUIManager.UpdatePlayerInfo(Player.FightCurrentDeck.Count, GetCardsBustAmount(Player.FightCurrentDeck, Player.currentScore, Player.MaxScore));
                break;
            case Character.Enemy:
                GameCard enemyCardDrawn = DrawAndPlayRandomCard(Character.Enemy);
                gameUIManager.ShowCardDrawn(Character.Enemy, enemyCardDrawn, Enemy.Class, effectsManager, EnemyCardAnimationCallback);
                gameUIManager.UpdateStandUI(character, Enemy.status, Enemy.currentScore,Enemy.CurrentMaxScore);
                break;
        }

        gameUIManager.UpdateUI(Character.Enemy, Enemy);
        gameUIManager.UpdateUI(Character.Player, Player);
    }

    public bool IsGameOnStandby()
    {
        bool isUserOnStandby = Player.status != CharacterStatus.Playing;
        bool isPlayerCardAnimating = effectsManager.movingObjects.Exists(a => a.type == VisualEffectsManager.MovingObject.TypeOfObject.CardDrawnPlayer);

        return isUserOnStandby || isPlayerCardAnimating || CurrentTurn == TurnStatus.EnemyTurn;
    }

    public void HandlePlayerStand()
    {
        if (IsGameOnStandby())
            return;

        Player.status = CharacterStatus.Standing;
        gameUIManager.SetStandButtonInteractable(false);

        if (Enemy.status == CharacterStatus.Playing)
            enemyManager.PlayEnemyTurn();
    }

    //Called after player drew their card
    void PlayerCardAnimationCallback()
    {
        if (Enemy.status != CharacterStatus.Playing)
            return;

        CurrentTurn = TurnStatus.EnemyTurn;
        enemyManager.HandleEnemyTurn();
    }

    //Called after player drew their card
    void EnemyCardAnimationCallback()
    {
        if (Player.status == CharacterStatus.Playing)
        {
            CurrentTurn = TurnStatus.PlayerTurn;
            return;
        }

        enemyManager.HandleEnemyTurn();
    }

    public enum Character
    {
        Player,
        Enemy,
        Default
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
        EnemyTurn,
        IntermediaryEffects
    }
}