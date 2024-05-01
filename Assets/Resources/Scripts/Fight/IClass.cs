using Unity.VisualScripting;
using static CardsManager;

public interface IClass
{
    public Classes Class { get; set; }

    public void PlayCardEffect(CardType cardType, FightUnit player, FightUnit enemy);
}
