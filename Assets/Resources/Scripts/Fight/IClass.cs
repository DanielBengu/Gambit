using Unity.VisualScripting;
using static CardsManager;

public interface IClass
{
    public Classes Class { get; set; }

    public void PlayCardEffect(CardType cardType, FightUnit unit, FightUnit enemy);

    public void PlayJack(FightUnit unit, FightUnit enemy);
    public void PlayQueen(FightUnit unit, FightUnit enemy);
    public void PlayKing(FightUnit unit, FightUnit enemy);
    public void PlayAce(FightUnit unit, FightUnit enemy);
}
