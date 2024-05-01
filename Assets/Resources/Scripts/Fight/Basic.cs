using System;

internal class Basic : IClass
{
    public CardsManager.Classes Class { get; set; } = CardsManager.Classes.Basic;

    public void PlayCardEffect(CardType cardType, FightUnit player, FightUnit enemy)
    {
        switch (cardType)
        {
            case CardType.Jack:
                PlayJack(player);
                break;
            case CardType.Queen: 
                PlayQueen(player);
                break;
            case CardType.King: 
                PlayKing(player);
                break;
        }
    }

    void PlayJack(FightUnit player)
    {
        player.CurrentModifiers.Add(new(FightUnit.Stats.Armor, 1, 1));
    }

    void PlayQueen(FightUnit player)
    {
        player.CurrentModifiers.Add(new(FightUnit.Stats.Armor, 1, 2));
    }

    void PlayKing(FightUnit player)
    {
        player.CurrentModifiers.Add(new(FightUnit.Stats.Armor, 1, 3));
    }
}
