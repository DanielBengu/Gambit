using Assets.Resources.Scripts.Fight;
using UnityEngine;

internal class Wizard : IClass
{
    public CardsManager.Classes Class { get; set; } = CardsManager.Classes.Wizard;
    public Color CardColor { get; set; } = Color.cyan;
    public CardsHandler CardsHandler { get; set; }

    public Wizard(FightManager manager)
    {
        CardsHandler = new(manager);
    }


    public void ResetTurn()
    {
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
