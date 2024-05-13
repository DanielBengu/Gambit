namespace Assets.Resources.Scripts.Fight
{
    public static class CardsHandler
    {
        public static void HandleBasicCards(CardType cardType, FightUnit unit, GameCard card)
        {
            switch (cardType)
            {
                case CardType.Potion:
                    HandlePotions(card, unit);
                    break;
            }
        }

        public static void HandlePotions(GameCard card, FightUnit unit)
        {
            PotionType type = (PotionType)card.id;
            switch (type)
            {
                case PotionType.Health:
                    FightManager.HealCharacter(unit, card.value);
                    break;
                case PotionType.Damage:
                    FightManager.DamageCharacter(unit, card.value);
                    break;
            }
        }

        public enum PotionType
        {
            Health,
            Damage
        }
    }
}
