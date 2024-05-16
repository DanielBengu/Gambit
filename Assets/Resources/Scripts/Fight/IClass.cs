using Assets.Resources.Scripts.Fight;
using Unity.VisualScripting;
using UnityEngine;
using static CardsManager;

public abstract class IClass
{
    public Classes Class { get; set; }
    public Color CardColor { get; set; }
    public CardsHandler CardsHandler { get; set; }

    public IClass(FightManager manager, Classes unitClass, Color cardColor)
    {
        CardsHandler = new(manager);   
        Class = unitClass;
        CardColor = cardColor;
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
            case CardType.Potion:
                CardsHandler.HandleBasicCards(cardType, unit, unitObj, enemy, enemyObj, card);
                break;
        }
    }

    public static Color GetCardBackgroundColor(Classes classOfUnit)
    {
        return classOfUnit switch
        {
            Classes.Warrior => Color.red,
            Classes.Rogue => Color.yellow,
            Classes.Wizard => Color.blue,
            Classes.Ranger => Color.green,
            Classes.Berserk => Color.magenta,
            Classes.Archmage => Color.white,
            Classes.Trickster => Color.cyan,
            _ => Color.grey,
        };
    }

    public abstract void PlayJack(FightUnit unit, FightUnit enemy);
    public abstract void PlayQueen(FightUnit unit, FightUnit enemy);
    public abstract void PlayKing(FightUnit unit, FightUnit enemy);
    public abstract void PlayAce(FightUnit unit, FightUnit enemy);

    public abstract string GetCardText(CardType cardType);
    public abstract Sprite GetCardIcon(CardType cardType);

    public abstract void ResetTurn();
}
