﻿using System;
using UnityEngine;

internal class Basic : IClass
{
    public CardsManager.Classes Class { get; set; } = CardsManager.Classes.Basic;
    public Color CardColor { get; set; } = Color.white;

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
    }

    public void PlayQueen(FightUnit unit, FightUnit enemy)
    {
    }

    public void PlayKing(FightUnit unit, FightUnit enemy)
    {
    }

    public void PlayAce(FightUnit unit, FightUnit enemy)
    {

    }

    public string GetCardText(CardType cardType)
    {
        switch (cardType)
        {
            case CardType.Default:
                return string.Empty;
            case CardType.Ace:
                return "ACE";
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
                return "JACK";
            case CardType.Queen:
                return "QUEEN";
            case CardType.King:
                return "KING";
            default:
                Debug.LogError($"Card {cardType} not implemented for {Class}");
                return string.Empty;
        }
    }

    public Sprite GetCardIcon(CardType cardType)
    {
        return null;
    }
}
