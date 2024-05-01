public class Rogue : IClass
{
    public CardsManager.Classes Class { get; set; } = CardsManager.Classes.Rogue;

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
}
