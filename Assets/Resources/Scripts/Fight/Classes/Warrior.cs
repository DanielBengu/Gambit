using System;
using UnityEngine;

public class Warrior : IClass
{
    public CardsManager.Classes Class { get; set; } = CardsManager.Classes.Warrior;
    public Color CardColor { get; set; } = Color.red;

    public void ResetTurn()
    {
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
        }
    }

    public void PlayJack(FightUnit unit, FightUnit enemy)
    {
        unit.CurrentModifiers.Add(new(FightUnit.Stats.Armor, 1, 1));
    }

    public void PlayQueen(FightUnit unit, FightUnit enemy)
    {
        unit.CurrentModifiers.Add(new(FightUnit.Stats.Armor, 1, 2));
    }

    public void PlayKing(FightUnit unit, FightUnit enemy)
    {
        unit.CurrentModifiers.Add(new(FightUnit.Stats.Armor, 1, 3));
    }

    public void PlayAce(FightUnit unit, FightUnit enemy)
    {
        unit.CurrentModifiers.Add(new(FightUnit.Stats.Attacks, 1, 2));
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
