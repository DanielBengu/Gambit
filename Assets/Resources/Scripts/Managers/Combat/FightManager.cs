using Assets.Resources.Scripts.Fight;
using System.Collections.Generic;
using UnityEngine;
using static EnemyManager;
using static AnimationManager;
using System;
public class FightManager
{
    readonly GameManager gameManager;
    readonly GameUIManager gameUIManager;
    readonly EffectsManager effectsManager;
    readonly EnemyManager enemyManager;

    public int BUST_PENALITY = 2;

    public GameObject playerObj;
    public GameObject enemyObj;

    public List<int> spritesAnimating = new();

    #region Player data
    public UnitData Unit { get; set; }
    public int PlayerMaxHP { get; set; }
    public int PlayerHP { get; set; }
    public int PlayerArmor { get; set; }
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

    public FightManager(EnemyData enemy, List<GameCard> playerStartingDeck, UnitData unit, GameUIManager gameUIManager, EffectsManager effectsManager, EnemyManager enemyManager, GameObject player, GameObject enemyObj, GameManager gameManager)
    {
        this.enemyManager = enemyManager;
        this.enemyManager.fightManager = this;

        Unit = unit;
        Enemy = new(enemy);

        PlayerMaxHP = unit.MaxHP;
        PlayerHP = unit.CurrentHP;
        PlayerArmor = unit.Armor;
        PlayerBaseDeck = GameManager.CopyDeck(playerStartingDeck);
        PlayerCurrentDeck = GameManager.CopyDeck(playerStartingDeck);
        PlayerScore = 0;
        PlayerMaxScore = unit.MaxScore;

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
        int bustAmount = GetCardsBustAmount(Character.Player);
        gameUIManager.SetupFightUI(Enemy, Unit, PlayerCurrentDeck.Count, bustAmount);
    }

    public void HandleEndTurn()
    {
        int damageAmount = Mathf.Abs(PlayerScore - Enemy.CurrentScore);

        if (PlayerScore > Enemy.CurrentScore)
        {
            DealDamage(Character.Enemy, damageAmount);
            MakeUnitPerformAnimation(Character.Player, SpriteAnimation.UnitDealingDamage);
        } else if(PlayerScore < Enemy.CurrentScore)
        {
            DealDamage(Character.Player, damageAmount);
            MakeUnitPerformAnimation(Character.Enemy, SpriteAnimation.UnitDealingDamage);
        }      

        ResetTurn();
    }

    public Character GetCharacterFromGameObjectID(int gameObjectID)
    {
        if (gameObjectID == playerObj.GetInstanceID())
            return Character.Player;

        if(gameObjectID == enemyObj.GetInstanceID())
            return Character.Enemy;

        return Character.Default;
    }

    void DealDamage(Character unitTakingDamage, int amount)
    {
        switch (unitTakingDamage)
        {
            case Character.Player:
                int playerDamage = amount - PlayerArmor;
                if(playerDamage > 0)
                    PlayerHP -= amount;
                break;
            case Character.Enemy:
                int enemyDamage = amount - Enemy.Armor;
                if (enemyDamage > 0)
                    Enemy.CurrentHP -= amount;
                break;
        }
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
        int unitHp = character == Character.Player ? PlayerHP : Enemy.CurrentHP;

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
                CharacterManager.ResetCharacter(enemyObj);
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
        PlayerScore = 0;
        PlayerStatus = CharacterStatus.Playing;

        Enemy.Status = CharacterStatus.Playing;
        Enemy.CurrentScore = 0;

        CurrentTurn = TurnStatus.PlayerTurn;

        gameUIManager.UpdateStandUI(Character.Enemy, Enemy.Status, 0, Enemy.MaxScore);
        gameUIManager.UpdateStandUI(Character.Player, PlayerStatus, 0, PlayerMaxScore);

        gameUIManager.UpdatePlayerInfo(PlayerCurrentDeck.Count, GetCardsBustAmount(Character.Player));

        gameUIManager.SetUnitHP(Character.Player, PlayerHP, PlayerMaxHP);
        gameUIManager.SetUnitHP(Character.Enemy, Enemy.CurrentHP, Enemy.MaxHP);

        gameUIManager.SetStandButtonInteractable(true);
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
                if (PlayerCurrentDeck.Count == 0)
                    ResetDeck(character);
                deck = PlayerCurrentDeck;
                break;
            case Character.Enemy:
                if (Enemy.CurrentDeck.Count == 0)
                    ResetDeck(character);
                deck = Enemy.CurrentDeck;
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

                PlayerCurrentDeck.Remove(card);

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

                Enemy.CurrentDeck.Remove(card);
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

    public void ResetDeck(Character character)
    {
        switch (character)
        {
            case Character.Player:
                PlayerCurrentDeck = GameManager.CopyDeck(PlayerBaseDeck);
                break;
            case Character.Enemy:
                Enemy.CurrentDeck = GameManager.CopyDeck(Enemy.BaseDeck);
                break;
        }
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
                    gameUIManager.SetStandButtonInteractable(false);
                gameUIManager.ShowCardDrawn(Character.Player, cardDrawn, effectsManager, PlayerCardAnimationCallback);
                gameUIManager.UpdateStandUI(character, PlayerStatus, PlayerScore, PlayerMaxScore);
                gameUIManager.UpdatePlayerInfo(PlayerCurrentDeck.Count, GetCardsBustAmount(character));
                break;
            case Character.Enemy:
                GameCard enemyCardDrawn = DrawAndPlayRandomCard(Character.Enemy);
                gameUIManager.ShowCardDrawn(Character.Enemy, enemyCardDrawn, effectsManager, EnemyCardAnimationCallback);
                gameUIManager.UpdateStandUI(character, Enemy.Status, Enemy.CurrentScore,Enemy.MaxScore);
                break;
        }
    }

    public bool IsGameOnStandby()
    {
        bool isUserOnStandby = PlayerStatus != CharacterStatus.Playing;
        bool isPlayerCardAnimating = effectsManager.movingObjects.Exists(a => a.type == EffectsManager.MovingObject.TypeOfObject.CardDrawnPlayer);

        return isUserOnStandby || isPlayerCardAnimating || CurrentTurn == TurnStatus.EnemyTurn;
    }

    public void HandlePlayerStand()
    {
        if (IsGameOnStandby())
            return;

        PlayerStatus = CharacterStatus.Standing;
        gameUIManager.SetStandButtonInteractable(false);

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