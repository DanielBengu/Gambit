using System;

public class Warrior : IClass
{
    public CardsManager.Classes Class { get; set; } = CardsManager.Classes.Warrior;

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
}
