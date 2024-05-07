using System;
using System.Collections.Generic;
using UnityEngine;
using static FightManager;

public class Rogue : IClass
{
    public static int _aceAttackAmount = 3;


    public CardsManager.Classes Class { get; set; } = CardsManager.Classes.Rogue;
    public Color CardColor { get; set; } = Color.green;

    public bool IsAce { get; set; }
    public int AceAttackAmount { get { return GetAceAttackAmount(); } }

    int GetAceAttackAmount()
    {
        return _aceAttackAmount;
    }

    public void ResetTurn()
    {
        IsAce = false;
    }

    public List<AttackStruct> GetBonusAttacks(FightUnit attacker, FightUnit defender, Action callback, FightManager manager)
    {
        if (!IsAce)
            return new();

        List<AttackStruct> atk = new()
        {
            new AttackStruct(attacker, defender, AceAttackAmount, true, callback, manager)
        };

        return atk;
    }

    public void PlayCardEffect(CardType cardType, FightUnit unit, FightUnit enemy)
    {
        switch (cardType)
        {
            case CardType.Jack:
                PlayJack(unit, enemy);
                break;
            case CardType.Queen: 
                PlayQueen(unit, enemy);
                break;
            case CardType.King: 
                PlayKing(unit, enemy);
                break;
            case CardType.Ace: 
                PlayAce(unit, enemy);
                break;
        }
    }

    public void PlayJack(FightUnit unit, FightUnit enemy)
    {
        AddScoreWithoutBusting(unit, 1);
    }

    public void PlayQueen(FightUnit unit, FightUnit enemy)
    {
        AddScoreWithoutBusting(unit, 2);
    }

    public void PlayKing(FightUnit unit, FightUnit enemy)
    {
        AddScoreWithoutBusting(unit, 3);
    }

    public void PlayAce(FightUnit unit, FightUnit enemy)
    {
        IsAce = true;
        SetScoreToCrit(unit);
    }

    void SetScoreToCrit(FightUnit unit)
    {
        unit.currentScore = unit.CurrentMaxScore;
    }

    void AddScoreWithoutBusting(FightUnit unit, int value)
    {
        unit.currentScore += value;

        if(unit.currentScore > unit.CurrentMaxScore)
            unit.currentScore = unit.CurrentMaxScore;
    }

    public string GetCardText(CardType cardType)
    {
        switch (cardType)
        {
            case CardType.Default:
                return string.Empty;
            case CardType.Ace:
                return "GAIN 2 EXTRA ATTACKS";
            case CardType.One:
                return "1";
            case CardType.Two:
                return "2";
            case CardType.Three:
                return "3";
            case CardType.Four:
                return "4";
            case CardType.Five:
                return "5";
            case CardType.Six:
                return "6";
            case CardType.Jack:
                return "GAIN 1 ARMOR";
            case CardType.Queen:
                return "GAIN 2 ARMOR";
            case CardType.King:
                return "GAIN 3 ARMOR";
            default:
                Debug.LogError($"Card {cardType} not implemented for {Class}");
                return string.Empty;
        }
    }

    public Sprite GetCardIcon(CardType cardType)
    {
        switch (cardType)
        {
            case CardType.Default:
            case CardType.One:
            case CardType.Two:
            case CardType.Three:
            case CardType.Four:
            case CardType.Five:
            case CardType.Six:
                return null;
            case CardType.Ace:
            case CardType.Jack:
            case CardType.Queen:
            case CardType.King:
                return Resources.Load<Sprite>($"Sprites/Icons/Cards/{cardType}");
            default:
                Debug.LogError($"Card {cardType} not implemented for {Class}");
                return null;
        }
    }
}
