using Assets.Resources.Scripts.Fight;
using System.Collections.Generic;
using UnityEngine;
using static AnimationManager;
using System;
using static CardsManager;
using System.Linq;
using TMPro;
using static GameUIManager;
public class FightManager
{
    public static int CRIT_SCORE = 12;

    #region Managers

    public readonly GameManager gameManager;
    public readonly GameUIManager gameUIManager;
    readonly VisualEffectsManager effectsManager;
    readonly EnemyManager enemyManager;
    public readonly TurnManager turnManager;

    #endregion

    public int BUST_PENALITY = 2;

    public GameObject playerObj;
    public GameObject enemyObj;

    public GameObject goldRewardObj;

    public List<int> spritesAnimating = new();

    public FightUnit Player { get; set; }
    public FightUnit Enemy { get { return enemyManager.Enemy; } set { enemyManager.Enemy = value; } }

    public TurnStatus CurrentTurn { get { return turnManager.CurrentTurn; } set { turnManager.CurrentTurn = value; } }

    public FightStatusEnum FightStatus { get; set; }

    public FightManager(EnemyData enemy, List<GameCard> playerStartingDeck, List<ActionCard> playerStartingActionDeck, UnitData unit, Classes playerClass, GameUIManager gameUIManager, VisualEffectsManager effectsManager, EnemyManager enemyManager, GameObject player, GameObject enemyObj, GameManager gameManager, GameObject goldRewardObj)
    {
        this.enemyManager = enemyManager;
        this.enemyManager.fightManager = this;

        Enemy = ConvertEnemyIntoUnit(enemy);
        Player = new(unit, true, playerClass, GameManager.CopyDeck(playerStartingDeck), GameManager.CopyDeck(playerStartingDeck), GameManager.CopyDeck(playerStartingActionDeck), GameManager.CopyDeck(playerStartingActionDeck), Character.Player, this, enemy.Rewards);

        turnManager = new(Player, Enemy, this, gameUIManager);

        FightStatus = FightStatusEnum.Ongoing;

        this.gameUIManager = gameUIManager;
        this.effectsManager = effectsManager;

        playerObj = player;
        this.enemyObj = enemyObj;

        this.gameManager = gameManager;
        this.goldRewardObj = goldRewardObj;
    }

    public void Update()
    {
        switch (FightStatus)
        {
            case FightStatusEnum.Ongoing:
            default:
                return;
            case FightStatusEnum.WaitingForRewards:
                HandleAwaitingForRewards();
                break;
            case FightStatusEnum.ScrollingRewards:
                //GiveReward();
                break;
            case FightStatusEnum.End:
                break;
        }
    }

    public void SetupFightUIAndStartGame()
    {
        int bustAmount = GetCardsBustAmount(Player.FightCurrentDeck, Player.currentScore, Player.MaxScore);
        gameUIManager.SetupFightUI(Enemy, Player, Player.FightCurrentDeck.Count, bustAmount);

        string damagePrevision = GetDamagePrevision(Player, Enemy, out PrevisionEnum prev);
        gameUIManager.UpdatePrevision(prev, damagePrevision);

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

    public void AddCardToHand(ActionCard card)
    {
        Player.FightCurrentHand.Add(card);

        gameUIManager.UpdateHand(Player.FightCurrentHand, this);
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

        return new(unit, false, enemy.UnitClass, GameManager.CopyDeck(enemy.BaseDecklist), GameManager.CopyDeck(enemy.BaseDecklist), new(), new(),  Character.Enemy, this, enemy.Rewards, enemy.BaseStandThreshold);
    }

    public Action GetAnimationCallback(SpriteAnimation animation, Character character)
    {
        return animation switch
        {
            SpriteAnimation.UnitTakingDamage => () => { },
            SpriteAnimation.UnitDealDamage => () => { },
            SpriteAnimation.Idle => () => { },
            SpriteAnimation.UnitDeath => character == Character.Player ? HandlePlayerDeath : HandleEnemyDeath,
            _ => () => { },
        };
    }

    public void HandleCharacterHPVariation(FightUnit unit, GameObject unitObj)
    {
        SpriteAnimation anim = GetDamageAnimation(unit.Character);
        Action callbackDefender = GetAnimationCallback(anim, unit.Character);

        PlayAnimation(unitObj, anim, callbackDefender);

        gameUIManager.UpdateUI(unit);
    }

    public SpriteAnimation GetDamageAnimation(Character character)
    {
        int unitHp = character == Character.Player ? Player.FightHP : Enemy.FightHP;

        if (unitHp <= 0)
            return SpriteAnimation.UnitDeath;
        else
            return SpriteAnimation.UnitTakingDamage;
    }

    #region Method Forwarding

    public void HandleEndTurn()
    {
        turnManager.HandleEndTurn();
    }

    public void HandlePlayerDeath()
    {
        gameManager.HandleFightDefeat();
    }

    public void StartAttackEffect(string effectName)
    {
        turnManager.StartAttackEffect(effectName);
    }

    #endregion


    public void HandleEnemyDeath()
    {
        FightStatus = FightStatusEnum.WaitingForRewards;

        CharacterManager.ResetCharacter(enemyObj, effectsManager);
        gameUIManager.TurnOfFightUI();

        enemyObj = GameManager.LoadCharacter("Chest", enemyObj.transform.parent);
    }

    public void HandleFightVictory()
    {
        gameManager.HandleFightVictory(Enemy.DefeatReward);
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

        if (deck.Count == 0)
            return null;

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
        int cardValue = ActionCardArchive.GetCardPointValue(card);
        FightUnit unit = character == Character.Player ? Player : Enemy;

        HandleSideEffects(character, unit.Class, card);
        HandlePoints(ref unit.status, character, ref unit.currentScore, unit.CurrentMaxScore, cardValue);

        string damagePrevision = GetDamagePrevision(Player, Enemy, out PrevisionEnum prev);
        gameUIManager.UpdatePrevision(prev, damagePrevision);

        unit.FightCurrentDeck.Remove(card);

        if (card.destroyOnPlay)
        {
            var cardOnBaseDeck = unit.FightBaseDeck.Find(c => c.cardType == card.cardType && c.id == card.id && c.value == card.value && c.classId == card.classId && c.destroyOnPlay == card.destroyOnPlay);
            unit.FightBaseDeck.Remove(cardOnBaseDeck);
        }
    }

    public string GetDamagePrevision(FightUnit player, FightUnit enemy, out PrevisionEnum previsionEnum)
    {
        string damagePrevision;
        int baseDamageAmount = Math.Abs(player.currentScore - enemy.currentScore);

        if (player.currentScore > enemy.currentScore)
        {
            previsionEnum = PrevisionEnum.PlayerAdvantage;
            baseDamageAmount = player.ApplyDamageModifiers(baseDamageAmount);

            if (baseDamageAmount < 0)
                baseDamageAmount = 0;

            damagePrevision = baseDamageAmount.ToString();

            if (player.Attacks > 1)
                damagePrevision += $"x{player.Attacks}";
        }
        else if (enemy.currentScore > player.currentScore)
        {
            previsionEnum = PrevisionEnum.EnemyAdvantage;

            baseDamageAmount = enemy.ApplyDamageModifiers(baseDamageAmount);

            if (baseDamageAmount < 0)
                baseDamageAmount = 0;

            damagePrevision = baseDamageAmount.ToString();

            if (enemy.Attacks > 1)
                damagePrevision += $"x{enemy.Attacks}";
        }
        else
        {
            previsionEnum = PrevisionEnum.Tie;
            damagePrevision = "0";
        }

        return damagePrevision;
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
                unitClass.PlayCardEffect(card.cardType, Player, playerObj, Enemy, enemyObj, card);
                break;
            case Character.Enemy:
                unitClass.PlayCardEffect(card.cardType, Enemy, enemyObj, Player, playerObj, card);
                break;
        }
    }

    public void HealCharacter(FightUnit unit, int healAmount)
    {
        unit.FightHP += healAmount;

        if(unit.FightHP > unit.FightMaxHP)
            unit.FightHP = unit.FightMaxHP;
    }

    public void DamageCharacter(FightUnit unit, GameObject unitObj, int damage)
    {
        unit.FightHP -= damage;

        HandleCharacterHPVariation(unit, unitObj);
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
            int cardValue = ActionCardArchive.GetCardPointValue(card);
            if (charCurrentScore + cardValue > charMaxScore)
                cardsBusting++;
        }

        return cardsBusting;
    }

    public void PlayUnitCard(Character character)
    {
        switch (character)
        {
            case Character.Player:
                if (Player.FightCurrentDeck.Count == 0 && Player.FightBaseDeck.Count == 0)
                    return;
                HandleCardDrawn(character, Player, PlayerCardAnimationCallback);
                if (Player.status != CharacterStatus.Playing)
                {
                    gameUIManager.SetStandButtonInteractable(false);
                    gameUIManager.ChangeScoreDeckVFX(false);
                }
                break;
            case Character.Enemy:
                HandleCardDrawn(character, Enemy, EnemyCardAnimationCallback);
                break;
        }
        gameUIManager.UpdateUI(Enemy);
        gameUIManager.UpdateUI(Player);
    }

    public void HandleCardDrawn(Character character, FightUnit unit, Action callback)
    {
        GameCard cardDrawn = DrawAndPlayRandomCard(character);

        gameUIManager.ShowCardDrawn(character, cardDrawn, unit.Class, effectsManager, callback);
        gameUIManager.UpdateStandUI(character, unit.status, unit.currentScore, unit.CurrentMaxScore, unit.Attacks);
    }

    public void PlayActionCard(ActionCard card, Character character)
    {
        FightUnit unit = null;
        FightUnit enemy = null;
        GameObject obj = playerObj;
        switch (character)
        {
            case Character.Player:
                unit = Player;
                enemy = Enemy;
                obj = playerObj;
                break;
            case Character.Enemy:
                unit = Enemy;
                enemy = Player;
                obj = enemyObj;
                break;
        }

        unit.Class.HandleActionCardPlayed(card);

        ActionCardArchive.ApplyEffect(card.Id, unit, enemy, this);
        string animation = ActionCardArchive.GetAnimation(card.Id);

        unit.FightCurrentHand.Remove(card);
        gameUIManager.UpdateHand(unit.FightCurrentHand, this);

        HandlePoints(ref unit.status, unit.Character, ref unit.currentScore, unit.CurrentMaxScore, 0);

        gameUIManager.UpdateUI(unit);
        gameUIManager.UpdateUI(enemy);

        string damagePrevision = GetDamagePrevision(Player, Enemy, out PrevisionEnum prev);
        gameUIManager.UpdatePrevision(prev, damagePrevision);

        PlayCustomAnimation(obj, animation, () => { });
    }

    public bool IsGameOnStandby()
    {
        bool isUserOnStandby = Player.status != CharacterStatus.Playing;
        bool isPlayerCardAnimating = effectsManager.movingObjects.Exists(a => a.type == VisualEffectsManager.MovingObject.TypeOfObject.CardDrawnPlayer);

        return isUserOnStandby || isPlayerCardAnimating || turnManager.CurrentTurn == TurnStatus.EnemyTurn || gameManager.IsInfoPanelOpen;
    }

    public void HandleAwaitingForRewards()
    {
        if (InputManager.IsClickDown())
        {
            string name = GetAnimationOfObject(enemyObj);

            if (name != "Chest_Idle")
                return;

            PlayAnimation(enemyObj, SpriteAnimation.ChestOpen, GiveReward);
            FightStatus = FightStatusEnum.ScrollingRewards;
        }
    }


    public void GiveReward()
    {
        if (Enemy.DefeatReward.Count == 0)
        {
            CharacterManager.ResetCharacter(enemyObj, effectsManager);
            HandleFightVictory();
            return;
        }
            
        Reward rewardToGive = Enemy.DefeatReward.First();
        Enemy.DefeatReward.Remove(rewardToGive);

        gameManager.playerData.CurrentRun.GoldAmount += rewardToGive.amount;

        HandleReward(rewardToGive);
    }

    void ResetGoldUI()
    {
        goldRewardObj.SetActive(false);
    }

    void HandleReward(Reward reward)
    {
        switch (reward.reward)
        {
            case TypeOfReward.Gold:
                StartGoldRewardAnimation(reward.amount);
                break;
            case TypeOfReward.Equipment:
                break;
            default:
                break;
        }
    }

    void StartGoldRewardAnimation(int amount)
    {
        goldRewardObj.transform.position = enemyObj.transform.position;

        TextMeshProUGUI text = goldRewardObj.GetComponent<TextMeshProUGUI>();
        text.text = $"+{amount}";

        Vector3 targetPos = goldRewardObj.transform.position + new Vector3(0, 2, 0);

        goldRewardObj.SetActive(true);

        effectsManager.effects.Add(new(VisualEffectsManager.Effects.AddGoldReward, goldRewardObj, new() { StartGoldUIUpdate, GiveReward, ResetGoldUI }, new object[4] {2, goldRewardObj.GetComponent<TextMeshProUGUI>(), targetPos, goldRewardObj.transform.position }));
    }

    void StartGoldUIUpdate()
    {
        effectsManager.effects.Add(new(VisualEffectsManager.Effects.UpdateGoldUI, gameUIManager.goldAmountText.gameObject, new()
        {
            () => { }
        }, new object[3] { gameUIManager.goldAmountText, gameUIManager, gameManager.playerData.CurrentRun.GoldAmount }));
    }

    public void PlayEnemyTurn()
    {
        if(Enemy.status == CharacterStatus.Playing)
            enemyManager.PlayEnemyTurn();
    }

    public void HandlePlayerStand()
    {
        if (IsGameOnStandby())
            return;

        Player.status = CharacterStatus.Standing;
        gameUIManager.SetStandButtonInteractable(false);

        gameUIManager.ChangeScoreDeckVFX(false);

        PlayEnemyTurn();
    }

    public void LoadActionDeckOnInfoPanel()
    {
        gameUIManager.LoadActionDeckInfo(Player.FightActionCurrentDeck, this);
    }

    public void ClearActionDeckInfoPanel()
    {
        gameUIManager.ClearInfoPanel();
    }

    //Called after player drew their card
    void PlayerCardAnimationCallback()
    {
        if (Enemy.status != CharacterStatus.Playing)
            return;

        enemyManager.HandleEnemyTurn();
    }

    //Called after player drew their card
    void EnemyCardAnimationCallback()
    {
        if (Player.status == CharacterStatus.Playing)
        {
            turnManager.CurrentTurn = TurnStatus.PlayerTurn;
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

    public enum FightStatusEnum
    {
        Ongoing,
        WaitingForRewards,
        ScrollingRewards,
        End
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