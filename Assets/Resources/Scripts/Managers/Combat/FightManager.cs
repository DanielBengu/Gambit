using Assets.Resources.Scripts.Fight;
using System.Collections.Generic;
using UnityEngine;
using static AnimationManager;
using System;
using static CardsManager;
using System.Collections;
using UnityEditor.Experimental.GraphView;
using System.Linq;
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
    public Queue<AttackStruct> attacks = new();

    public FightUnit Player { get; set; }
    public FightUnit Enemy { get { return enemyManager.Enemy; } set { enemyManager.Enemy = value; } }

    public TurnStatus CurrentTurn { get; set; }

    public FightManager(EnemyData enemy, List<GameCard> playerStartingDeck, List<ActionCard> playerStartingActionDeck, UnitData unit, Classes playerClass, GameUIManager gameUIManager, VisualEffectsManager effectsManager, EnemyManager enemyManager, GameObject player, GameObject enemyObj, GameManager gameManager)
    {
        this.enemyManager = enemyManager;
        this.enemyManager.fightManager = this;

        Enemy = ConvertEnemyIntoUnit(enemy);
        Player = new(unit, true, playerClass, GameManager.CopyDeck(playerStartingDeck), GameManager.CopyDeck(playerStartingDeck), GameManager.CopyDeck(playerStartingActionDeck), GameManager.CopyDeck(playerStartingActionDeck), Character.Player);

        CurrentTurn = TurnStatus.IntermediaryEffects;

        this.gameUIManager = gameUIManager;
        this.effectsManager = effectsManager;

        playerObj = player;
        this.enemyObj = enemyObj;

        this.gameManager = gameManager;
    }

    public void SetupFightUIAndStartGame()
    {
        int bustAmount = GetCardsBustAmount(Player.FightCurrentDeck, Player.currentScore, Player.MaxScore);
        gameUIManager.SetupFightUI(Enemy, Player, Player.FightCurrentDeck.Count, bustAmount);

        StartPlayerTurn();
    }

    public void StartPlayerTurn()
    {
        DrawTurnHand();
    }

    public void DrawTurnHand()
    {
        for (int i = 0; i < Player.CardDrawnForTurn; i++)
        {
            Player.FightCurrentHand.Add(DrawActionCardFromDeck(Player));
        }

        gameUIManager.UpdateHand(Player.FightCurrentHand, this);
    }

    public ActionCard DrawActionCardFromDeck(FightUnit unit)
    {
        if (unit.FightActionCurrentDeck.Count == 0)
            ResetActionDeck(unit.Character);

        int cardIndex = UnityEngine.Random.Range(0, unit.FightActionCurrentDeck.Count);

        ActionCard cardDrawn = unit.FightActionCurrentDeck[cardIndex];
        
        unit.FightActionCurrentDeck.Remove(cardDrawn);

        return cardDrawn;
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

        return new(unit, false, enemy.UnitClass, GameManager.CopyDeck(enemy.BaseDecklist), GameManager.CopyDeck(enemy.BaseDecklist), new(), new(),  Character.Enemy, enemy.BaseStandThreshold);
    }

    public void HandleEndTurn()
    {
        int pointDifference = Mathf.Abs(Player.currentScore - Enemy.currentScore);

        if (pointDifference != 0)
        {
            FightUnit attacker = Player.currentScore > Enemy.currentScore ? Player : Enemy;
            FightUnit defender = Player.currentScore > Enemy.currentScore ? Enemy : Player;

            int damage = CalculateDamage(attacker, defender, pointDifference, false);

            SetAttacks(attacker, defender, damage);

            GameObject obj = attacker.Character == Character.Player ? playerObj : enemyObj;

            PlayAnimation(obj, SpriteAnimation.UnitDealingDamage, DealDamage);
        }

        ResetTurn();
    }

    public void SetAttacks(FightUnit attacker, FightUnit defender, int damage)
    {
        for (int i = 0; i < attacker.Attacks; i++)
            attacks.Enqueue(new(attacker, defender, damage, DealDamage));

        AddBonusAttacks(attacker, defender);
    }

    void AddBonusAttacks(FightUnit attacker, FightUnit defender)
    {
        switch (attacker.Class.Class)
        {
            case Classes.Basic:
            case Classes.Warrior:
            case Classes.Wizard:
            case Classes.Berserk:
            case Classes.Poisoner:
            case Classes.Archmage:
            case Classes.Trickster:
            default:
                break;
            case Classes.Rogue:
                Rogue rogueClass = attacker.Class as Rogue;
                foreach (var attack in rogueClass.GetBonusAttacks(attacker, defender, DealDamage))
                {
                    attacks.Enqueue(attack);
                }
                break;
        }
    }

    int CalculateDamage(FightUnit attacker, FightUnit defender, int baseDamage, bool isPiercing)
    {
        int attackerDamage = attacker.ApplyDamageModifiers(baseDamage);
        int finalDamageAmount = defender.ApplyDefenceModifiers(attackerDamage, isPiercing);

        return finalDamageAmount;
    }

    public void StartAttackEffect(string effectName)
    {
        if (attacks.Count == 0)
            return;

        GameObject obj = null;
        Transform defenderTransform = null;
        Transform attackerTransform = null;

        switch (effectName)
        {
            case "wizard_earth_attack":
                obj = Resources.Load<GameObject>($"Prefabs/Characters/Wizard/Effects/Earth Attack/Earth Attack Prefab");
                defenderTransform = attacks.First().defender.Character == Character.Player ? playerObj.transform : enemyObj.transform;
                attackerTransform = attacks.First().attacker.Character == Character.Player ? playerObj.transform : enemyObj.transform;
                break;
            case "wizard_fire_attack":
                obj = Resources.Load<GameObject>($"Prefabs/Characters/Wizard/Effects/Fire Attack/Fire Attack Prefab");
                defenderTransform = attacks.First().defender.Character == Character.Player ? playerObj.transform : enemyObj.transform;
                attackerTransform = attacks.First().attacker.Character == Character.Player ? playerObj.transform : enemyObj.transform;
                break;
        }

        InstantiateEffect(obj, defenderTransform, attackerTransform.GetComponent<UnitAnimationManager>().dictionaryCallback);
    }

    void InstantiateEffect(GameObject obj, Transform parent, Dictionary<string, Action> callbacks)
    {
        if(obj == null || parent == null || callbacks == null) 
            return;

        GameObject attackInstance = UnityEngine.Object.Instantiate(obj, parent.position, parent.rotation, playerObj.transform.parent);

        Animator anim = attackInstance.GetComponent<Animator>();
        anim.Play("Attack");

        var script = attackInstance.GetComponent<UnitAnimationManager>();
        script.dictionaryCallback = callbacks;
    }

    void DealDamage()
    {
        if (attacks.Count == 0)
            return;

        AttackStruct attack = attacks.Dequeue();

        FightUnit unitTakingDamage = attack.defender;
        unitTakingDamage.FightHP -= attack.damageAmount;

        GameObject defender = attack.defender.Character == Character.Player ? playerObj : enemyObj;

        SpriteAnimation anim = GetDamageAnimation(attack.defender.Character);
        Action callbackDefender = GetAnimationCallback(anim, attack.defender.Character);

        gameUIManager.UpdateUI(attack.defender.Character, attack.defender);

        PlayAnimation(defender, anim, callbackDefender);

        if(attacks.Count > 0)
        {
            GameObject newAttacker = attacks.First().attacker.Character == Character.Player ? playerObj: enemyObj;
            PlayAnimation(newAttacker, SpriteAnimation.UnitDealingDamage, attacks.First().callback);
        }
    }

    public Action GetAnimationCallback(SpriteAnimation animation, Character character)
    {
        return animation switch
        {
            SpriteAnimation.UnitTakingDamage => () => { },
            SpriteAnimation.UnitDealingDamage => () => { },
            SpriteAnimation.UnitIdle => () => { },
            SpriteAnimation.UnitDeath => character == Character.Player ? HandlePlayerDeath : HandleEnemyDeath,
            _ => () => { },
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
        Action callback = GetAnimationCallback(animation, character);

        switch (character)
        {
            case Character.Player:
                PlayAnimation(playerObj, animation, callback);
                break;
            case Character.Enemy:
                PlayAnimation(enemyObj, animation, callback);
                break;
        }
    }

    public void HandlePlayerDeath()
    {
        gameManager.HandleFightDefeat();
    }

    public void HandleEnemyDeath()
    {
        CharacterManager.ResetCharacter(enemyObj, effectsManager);
        gameUIManager.TurnOfFightUI();
        gameManager.HandleFightVictory();
    }

    void ResetTurn()
    {
        ResetUnitTurn(Player);
        ResetUnitTurn(Enemy);

        DrawTurnHand();

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

        unit.Class.ResetTurn();

        unit.FightCurrentHand.Clear();
    }

    /*
    public void RemoveObjFromAnimationList(GameObject obj)
    {
        effectsManager.RemoveFromLists(obj);
    }
    */

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

                HandleSideEffects(character, Player.Class, card);
                HandlePoints(ref Player.status, character, ref Player.currentScore, Player.CurrentMaxScore, playerCardValue);

                Player.FightCurrentDeck.Remove(card);
                break;
            case Character.Enemy:
                int enemyCardValue = GetCardPointValue(card);

                HandleSideEffects(character, Enemy.Class, card);
                HandlePoints(ref Enemy.status, character, ref Enemy.currentScore, Enemy.CurrentMaxScore, enemyCardValue);

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
            return CharacterStatus.StandingOnCrit;

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

    public void ResetActionDeck(Character character)
    {
        switch (character)
        {
            case Character.Player:
                Player.FightActionCurrentDeck = GameManager.CopyDeck(Player.FightActionBaseDeck);
                break;
            case Character.Enemy:
                Enemy.FightActionCurrentDeck = GameManager.CopyDeck(Enemy.FightActionBaseDeck);
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
                gameUIManager.UpdateStandUI(character, Player.status, Player.currentScore, Player.CurrentMaxScore, Player.Attacks);
                gameUIManager.UpdatePlayerInfo(Player.FightCurrentDeck.Count, GetCardsBustAmount(Player.FightCurrentDeck, Player.currentScore, Player.MaxScore));
                break;
            case Character.Enemy:
                GameCard enemyCardDrawn = DrawAndPlayRandomCard(Character.Enemy);
                gameUIManager.ShowCardDrawn(Character.Enemy, enemyCardDrawn, Enemy.Class, effectsManager, EnemyCardAnimationCallback);
                gameUIManager.UpdateStandUI(character, Enemy.status, Enemy.currentScore,Enemy.CurrentMaxScore, Enemy.Attacks);
                break;
        }

        gameUIManager.UpdateUI(Character.Enemy, Enemy);
        gameUIManager.UpdateUI(Character.Player, Player);
    }

    public void PlayActionCard(ActionCard card, Character character)
    {
        FightUnit unit = null;
        switch (character)
        {
            case Character.Player:
                unit = Player;
                break;
            case Character.Enemy:
                unit = Enemy;
                break;
        }
        ActionCardArchive.ApplyEffect(card.Id, unit);
        unit.FightCurrentHand.Remove(card);
        gameUIManager.UpdateHand(unit.FightCurrentHand, this);

        HandlePoints(ref unit.status, unit.Character, ref unit.currentScore, unit.CurrentMaxScore, 0);
        gameUIManager.UpdateUI(unit.Character, unit);
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
        StandingOnCrit,
        Bust
    }

    public enum TurnStatus
    {
        PlayerTurn,
        EnemyTurn,
        IntermediaryEffects
    }

    public struct AttackStruct
    {
        public FightUnit attacker;
        public FightUnit defender;
        public int damageAmount;
        public Action callback;

        public AttackStruct(FightUnit attacker, FightUnit defender, int damageAmount, Action callback)
        {
            this.attacker = attacker;
            this.defender = defender;
            
            this.damageAmount = damageAmount;

            this.callback = callback;
        }
    }
}