using System;

internal class Basic : IClass
{
    public CardsManager.Classes Class { get; set; } = CardsManager.Classes.Basic;

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
}
