using UnityEngine;

namespace Assets.Resources.Scripts.Fight
{
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

        public enum PotionType
        {
            Health,
            Damage
        }
    }
}
