using Assets.Resources.Scripts.Fight;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.Timeline;
using static CardsManager;
using static FightManager;

public class FightUnit : UnitData
{
    #region Static data

    static readonly int BASE_CARD_DRAW_TURN = 3;

    #endregion

    #region Private Fields

    private readonly int _attacks;
    private readonly int _damageModifier;
    private readonly int _actionCardDrawnForTurn;

    #endregion

    #region Stats

    public int FightMaxHP { get { return GetUpdatedStat(Stats.MaxHP); } }
    public int FightHP { get; set; }
    public int FightArmor { get { return GetUpdatedStat(Stats.Armor); } }
    public int Attacks { get { return GetUpdatedStat(Stats.Attacks); } }
    public int CurrentMaxScore { get { return GetUpdatedStat(Stats.MaxScore); } }
    public int CardDrawnForTurn { get { return GetUpdatedStat(Stats.CardDrawnForTurn); } }
    public int Damage { get { return GetUpdatedStat(Stats.Damage); } }

    #endregion

    public IClass Class { get; set; }
    public bool IsPlayer { get; set; }

    #region Points deck

    public List<GameCard> FightCurrentDeck { get; set; }
    public List<GameCard> FightBaseDeck { get; set; }

    #endregion

    #region Action deck

    public List<ActionCard> FightActionCurrentDeck { get; set; }
    public List<ActionCard> FightActionBaseDeck { get; set; }

    public List<ActionCard> FightCurrentHand { get; set; }

    #endregion

    public int currentScore;
    
    public CharacterStatus status = CharacterStatus.Playing;

    //Value that forces stands
    public int Threshold { get; set; }

    public List<Reward> DefeatReward { get; set; }

    public List<Modifiers> CurrentModifiers { get; set; } = new();

    public Character Character { get; set; }

    public FightUnit(UnitData unit, bool isPlayer, Classes @class, List<GameCard> baseDeck, List<GameCard> currentDeck, List<ActionCard> baseActionDeck, List<ActionCard> currentActionDeck, Character character, FightManager manager, List<Reward> rewardList, int threshold = 0)
    {
        MaxHP = unit.MaxHP;
        FightHP = unit.CurrentHP;
        Armor = unit.Armor;
        MaxScore = unit.MaxScore;
        Name = unit.Name;

        _attacks = unit.NumberOfAttacks;
        if (_attacks <= 0)
            _attacks = 1;

        IsPlayer = isPlayer;

        FightCurrentDeck = currentDeck;
        FightBaseDeck = baseDeck;

        FightActionBaseDeck = baseActionDeck;
        FightActionCurrentDeck = currentActionDeck;

        FightCurrentHand = new();

        currentScore = 0;
        _damageModifier = 0;

        _actionCardDrawnForTurn = BASE_CARD_DRAW_TURN;

        Threshold = threshold;

        AssignClass(@class, manager);

        Character = character;

        DefeatReward = rewardList;
    }

    void AssignClass(Classes playerClass, FightManager manager)
    {
        switch (playerClass)
        {
            case Classes.Basic:
                Class = new Basic(manager);
                break;
            case Classes.Warrior:
                Class = new Warrior(manager);
                break;
            case Classes.Rogue:
                Class = new Rogue(manager);
                break;
            case Classes.Wizard:
                Class = new Wizard(manager);
                break;
            case Classes.Trickster:
                Class = new Trickster(manager);
                break;
            case Classes.Monk:
                Class = new Monk(manager);
                break;
            case Classes.Crystal:
                Class = new Crystal(manager);
                break;
            case Classes.Ranger:
                Class = new Ranger(manager);
                break;
        }
    }

    public int GetUpdatedStat(Stats stat)
    {
        int baseValue = 0;

        switch (stat)
        {
            case Stats.MaxHP:
                baseValue = MaxHP;
                break;
            case Stats.Armor:
                baseValue = Armor;
                break;
            case Stats.MaxScore:
                baseValue = MaxScore;
                break;
            case Stats.Attacks:
                baseValue = _attacks;
                break;
            case Stats.Damage:
                baseValue = _damageModifier;
                break;
            case Stats.CardDrawnForTurn: 
                baseValue = _actionCardDrawnForTurn; 
                break;
            default:
                break;
        }

        List<Modifiers> modifersList = CurrentModifiers.Where(m => m.statModified == stat).ToList();
        int modifiers = GetModifiersValue(modifersList);

        int finalValue = AdditionalChecks(stat, baseValue + modifiers);

        return finalValue;
    }

    int AdditionalChecks(Stats stat, int currentValue)
    {
        switch (stat)
        {
            case Stats.Armor:
                if(currentValue < 0)
                    currentValue = 0;
                break;
        }

        return currentValue;
    }

    int GetModifiersValue(List<Modifiers> modifiers)
    {
        if (modifiers.Count == 0) 
            return 0;

        return modifiers.Select(m => m.valueModifier).Sum();
    }

    public void TickModifiers()
    {
        for (int i = 0; i < CurrentModifiers.Count; i++)
        {
            Modifiers modifiers = CurrentModifiers[i];
            modifiers.TickModifier(out bool isFinished);
            if (isFinished)
            {
                CurrentModifiers.RemoveAt(i);
                i--;
            }
        }
    }

    public int ApplyDefenceModifiers(int incomingDamage, bool isPiercing)
    {
        int damageAmount = incomingDamage;

        if (!isPiercing)
            damageAmount -= FightArmor;

        if(damageAmount < 0)
            damageAmount = 0;

        return damageAmount;
    }

    public int ApplyDamageModifiers(int baseDamage)
    {
        int damageAmount = baseDamage + Damage;

        if (currentScore >= CRIT_SCORE)
            damageAmount *= 2;

        return damageAmount;
    }

    public struct Modifiers
    {
        public static int PERMANENT_EFFECT_COUNT = -1;
        public Stats statModified;
        public int turnCount;
        public int valueModifier;

        public Modifiers(Stats stat, int turnCount, int valueModifier)
        {
            statModified = stat;
            this.turnCount = turnCount; 
            this.valueModifier = valueModifier;
        }

        public void TickModifier(out bool IsFinished)
        {
            if (IsPermanentEffect())
                IsFinished = false;
            else
            {
                turnCount--;

                IsFinished = turnCount == 0;
            }
        }

        readonly bool IsPermanentEffect()
        {
            return turnCount == PERMANENT_EFFECT_COUNT;
        }
    }

    public enum Stats
    {
        MaxHP,
        Armor,
        MaxScore,
        Damage,
        Attacks,
        CardDrawnForTurn
    }
}