using static CardsManager;
using System.Collections.Generic;
using static FightUnit;

namespace Assets.Resources.Scripts.Fight
{
    public static class ActionCardArchive
    {
        public static void ApplyEffect(int cardId, FightUnit unit)
        {
            switch (cardId)
            {
                case 0:
                case 5:
                case 8:
                case 11:
                case 14:
                case 17:
                case 20:
                    ApplyModifier(unit, Stats.Attacks, 1, 1);
                    break;
                case 1:
                case 6:
                case 9:
                case 12:
                case 15:
                case 18:
                case 21:
                    ApplyModifier(unit, Stats.Damage, 1, 2); 
                    break;
                case 2:
                case 7:
                case 10:
                case 13:
                case 16:
                case 19:
                case 22:
                    ApplyModifier(unit, Stats.Armor, 1, 2);
                    break;
                case 3:
                    AddScore(unit, 2);
                    break;
                case 4:
                    AddScore(unit, 6);
                    break;
            }
        }

        static void ApplyModifier(FightUnit unit, Stats stat, int turnCount, int modifier)
        {
            unit.CurrentModifiers.Add(new(stat, turnCount, modifier));
        }
        static void AddScore(FightUnit unit, int modifier)
        {
            unit.currentScore += modifier;
        }

        public static List<Card> GetActionCardList(Classes classOfDeck, CardListData cardData)
        {
            return classOfDeck switch
            {
                Classes.Warrior => cardData.WarriorActionCardList,
                Classes.Wizard => cardData.WizardActionCardList,
                Classes.Ranger => cardData.RangerActionCardList,
                Classes.Rogue => cardData.RogueActionCardList,
                Classes.Monk => cardData.MonkActionCardList,
                Classes.Crystal => cardData.CrystalActionCardList,
                Classes.Trickster => cardData.TricksterActionCardList,
                _ => cardData.WarriorActionCardList,
            };
        }

        public static ActionCard ConvertIdIntoCard(int id)
        {
            return id switch
            {
                0 => new()
                {
                    Id = id,
                    NameIdValue = 32,
                    DescriptionIdValue = 38,
                    ActionId = ActionType.Equip,
                    ClassId = Classes.Warrior
                },
                1 => new()
                {
                    Id = id,
                    NameIdValue = 33,
                    DescriptionIdValue = 39,
                    ActionId = ActionType.Attack,
                    ClassId = Classes.Warrior
                },
                2 => new()
                {
                    Id = id,
                    NameIdValue = 35,
                    DescriptionIdValue = 40,
                    ActionId = ActionType.Skill,
                    ClassId = Classes.Warrior
                },
                3 => new()
                {
                    Id = id,
                    NameIdValue = 36,
                    DescriptionIdValue = 41,
                    ActionId = ActionType.Modifier,
                    ClassId = Classes.Basic
                },
                4 => new()
                {
                    Id = id,
                    NameIdValue = 37,
                    DescriptionIdValue = 42,
                    ActionId = ActionType.Modifier,
                    ClassId = Classes.Basic
                },
                5 => new()
                {
                    Id = id,
                    NameIdValue = 46,
                    DescriptionIdValue = 38,
                    ActionId = ActionType.Equip,
                    ClassId = Classes.Wizard
                },
                6 => new()
                {
                    Id = id,
                    NameIdValue = 47,
                    DescriptionIdValue = 39,
                    ActionId = ActionType.Attack,
                    ClassId = Classes.Wizard
                },
                7 => new()
                {
                    Id = id,
                    NameIdValue = 48,
                    DescriptionIdValue = 40,
                    ActionId = ActionType.Skill,
                    ClassId = Classes.Wizard
                },
                8 => new()
                {
                    Id = id,
                    NameIdValue = 34,
                    DescriptionIdValue = 38,
                    ActionId = ActionType.Equip,
                    ClassId = Classes.Ranger
                },
                9 => new()
                {
                    Id = id,
                    NameIdValue = 49,
                    DescriptionIdValue = 39,
                    ActionId = ActionType.Attack,
                    ClassId = Classes.Ranger
                },
                10 => new()
                {
                    Id = id,
                    NameIdValue = 50,
                    DescriptionIdValue = 40,
                    ActionId = ActionType.Skill,
                    ClassId = Classes.Ranger
                },
                11 => new()
                {
                    Id = id,
                    NameIdValue = 51,
                    DescriptionIdValue = 38,
                    ActionId = ActionType.Equip,
                    ClassId = Classes.Rogue
                },
                12 => new()
                {
                    Id = id,
                    NameIdValue = 52,
                    DescriptionIdValue = 39,
                    ActionId = ActionType.Attack,
                    ClassId = Classes.Rogue
                },
                13 => new()
                {
                    Id = id,
                    NameIdValue = 53,
                    DescriptionIdValue = 40,
                    ActionId = ActionType.Skill,
                    ClassId = Classes.Rogue
                },
                14 => new()
                {
                    Id = id,
                    NameIdValue = 55,
                    DescriptionIdValue = 38,
                    ActionId = ActionType.Equip,
                    ClassId = Classes.Monk
                },
                15 => new()
                {
                    Id = id,
                    NameIdValue = 56,
                    DescriptionIdValue = 39,
                    ActionId = ActionType.Attack,
                    ClassId = Classes.Monk
                },
                16 => new()
                {
                    Id = id,
                    NameIdValue = 57,
                    DescriptionIdValue = 40,
                    ActionId = ActionType.Skill,
                    ClassId = Classes.Monk
                },
                17 => new()
                {
                    Id = id,
                    NameIdValue = 58,
                    DescriptionIdValue = 38,
                    ActionId = ActionType.Equip,
                    ClassId = Classes.Crystal
                },
                18 => new()
                {
                    Id = id,
                    NameIdValue = 59,
                    DescriptionIdValue = 39,
                    ActionId = ActionType.Attack,
                    ClassId = Classes.Crystal
                },
                19 => new()
                {
                    Id = id,
                    NameIdValue = 60,
                    DescriptionIdValue = 40,
                    ActionId = ActionType.Skill,
                    ClassId = Classes.Crystal
                },
                20 => new()
                {
                    Id = id,
                    NameIdValue = 61,
                    DescriptionIdValue = 38,
                    ActionId = ActionType.Equip,
                    ClassId = Classes.Trickster
                },
                21 => new()
                {
                    Id = id,
                    NameIdValue = 62,
                    DescriptionIdValue = 39,
                    ActionId = ActionType.Attack,
                    ClassId = Classes.Trickster
                },
                22 => new()
                {
                    Id = id,
                    NameIdValue = 63,
                    DescriptionIdValue = 40,
                    ActionId = ActionType.Skill,
                    ClassId = Classes.Trickster
                },
                _ => null
            };
        }

        public static string GetAnimation(int animationId)
        {
            return animationId switch
            {
                2 or 7 or 10 or 13 or 16 or 19 or 22 => "Defend",
                _ => string.Empty,
            };
        }

        public static int GetCardPointValue(GameCard card)
        {
            int valueToAdd = 0;

            switch (card.cardType)
            {
                case CardType.One:
                case CardType.Two:
                case CardType.Three:
                case CardType.Four:
                case CardType.Five:
                case CardType.Six:
                    valueToAdd = card.value;
                    break;
                case CardType.Ace:
                case CardType.Jack:
                case CardType.Queen:
                case CardType.King:
                case CardType.Potion:
                    valueToAdd = 0;
                    break;
            }

            return valueToAdd;
        }
    }
}
