using Assets.Resources.Scripts.Fight;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static CardsManager;
using static FightManager;

public abstract class IClass
{
    public Classes Class { get; set; }
    public CardsHandler CardsHandler { get; set; }

    internal FightManager FightManager { get; }

    public IClass(FightManager manager, Classes unitClass)
    {
        CardsHandler = new(manager);   
        Class = unitClass;
        FightManager = manager;
    }


    public void PlayCardEffect(CardType cardType, FightUnit unit, GameObject unitObj, FightUnit enemy, GameObject enemyObj, GameCard card)
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
            case CardType.Potion:
                CardsHandler.HandleBasicCards(cardType, unit, unitObj, enemy, enemyObj, card);
                break;
        }
    }

    public static Color GetCardBackgroundColor(Classes classOfUnit)
    {
        return classOfUnit switch
        {
            Classes.Warrior => new(0.6f, 0.09f, 0.09f),
            Classes.Rogue => new(0.6f, 0.6f, 0.6f),
            Classes.Wizard => new(0.2f, 0.4f, 0.7f),
            Classes.Ranger => new(0.05f, 0.5f, 0.05f),
            Classes.Monk => new(0.62f, 0.3f, 0.14f),
            Classes.Crystal => new(0.62f, 0.98f, 0.94f),
            Classes.Trickster => new(0.7f, 0.5f, 0.055f),
            _ => Color.grey,
        };
    }

    public abstract void PlayJack(FightUnit unit, FightUnit enemy);
    public abstract void PlayQueen(FightUnit unit, FightUnit enemy);
    public abstract void PlayKing(FightUnit unit, FightUnit enemy);
    public abstract void PlayAce(FightUnit unit, FightUnit enemy);

    public abstract string GetAttackAnimation(FightUnit unit, Queue<AttackStruct> attacks, GameObject obj);

    public abstract string GetCardText(CardType cardType);
    public abstract Sprite GetCardIcon(CardType cardType);

    public abstract void ResetTurn();
}
