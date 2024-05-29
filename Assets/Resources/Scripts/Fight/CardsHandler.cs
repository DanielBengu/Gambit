using UnityEngine;

public class CardsHandler
{
    readonly FightManager manager;

    public CardsHandler(FightManager manager)
    {
        this.manager = manager;
    }

    public void HandleBasicCards(CardType cardType, FightUnit unit, GameObject unitObj, FightUnit enemy, GameObject enemyObj, GameCard card)
    {
        switch (cardType)
        {
            case CardType.Potion:
                HandlePotions(card, unit, unitObj, enemy, enemyObj);
                break;
        }
    }

    public void HandlePotions(GameCard card, FightUnit unit, GameObject unitObj, FightUnit enemy, GameObject enemyObj)
    {
        PotionType type = (PotionType)card.id;
        switch (type)
        {
            case PotionType.Health:
                manager.HealCharacter(unit, card.value);
                break;
            case PotionType.Damage:
                manager.DamageCharacter(enemy, enemyObj, card.value);
                break;
        }
    }

    public Sprite HandleIcons(GameCard card)
    {
        switch (card.cardType)
        {
            default:
                return null;
            case CardType.Potion:
                PotionType type = (PotionType)card.id;
                return GetPotionIcon(type);
        }
    }

    public Sprite GetPotionIcon(PotionType type)
    {
        return type switch
        {
            PotionType.Health => Resources.Load<Sprite>($"Sprites/Cards/Potion_{type}"),
            PotionType.Damage => Resources.Load<Sprite>($"Sprites/Cards/Potion_{type}"),
            _ => null
        };
    }

    public enum PotionType
    {
        Health,
        Damage
    }
}
