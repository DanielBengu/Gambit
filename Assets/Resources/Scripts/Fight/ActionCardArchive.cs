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
                    ApplyModifier(unit, Stats.Attacks, 1, 1);
                    break;
                case 1: 
                    ApplyModifier(unit, Stats.Damage, 1, 2); 
                    break;
                case 2:
                    ApplyModifier(unit, Stats.Armor, 1, 2);
                    break;
                case 3:
                    AddScore(unit, 2);
                    break;
                case 4:
                    AddScore(unit, 6);
                    break;
                case 5:
                    ApplyModifier(unit, Stats.Attacks, 1, 1);
                    break;
                case 6:
                    ApplyModifier(unit, Stats.Damage, 1, 2);
                    break;
                case 7:
                    ApplyModifier(unit, Stats.Armor, 1, 2);
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
                _ => cardData.WarriorActionCardList,
            };
        }

        public static ActionCard ConvertIdIntoCard(int id)
        {
            return id switch
            {
                0 => new()
                {
                    Id = 0,
                    NameIdValue = 32,
                    DescriptionIdValue = 38,
                    ActionId = ActionType.Equip,
                    ClassId = Classes.Warrior
                },
                1 => new()
                {
                    Id = 1,
                    NameIdValue = 33,
                    DescriptionIdValue = 39,
                    ActionId = ActionType.Attack,
                    ClassId = Classes.Warrior
                },
                2 => new()
                {
                    Id = 2,
                    NameIdValue = 35,
                    DescriptionIdValue = 40,
                    ActionId = ActionType.Skill,
                    ClassId = Classes.Warrior
                },
                3 => new()
                {
                    Id = 3,
                    NameIdValue = 36,
                    DescriptionIdValue = 41,
                    ActionId = ActionType.Modifier,
                    ClassId = Classes.Basic
                },
                4 => new()
                {
                    Id = 4,
                    NameIdValue = 37,
                    DescriptionIdValue = 42,
                    ActionId = ActionType.Modifier,
                    ClassId = Classes.Basic
                },
                5 => new()
                {
                    Id = 5,
                    NameIdValue = 46,
                    DescriptionIdValue = 38,
                    ActionId = ActionType.Equip,
                    ClassId = Classes.Wizard
                },
                6 => new()
                {
                    Id = 6,
                    NameIdValue = 47,
                    DescriptionIdValue = 39,
                    ActionId = ActionType.Attack,
                    ClassId = Classes.Wizard
                },
                7 => new()
                {
                    Id = 7,
                    NameIdValue = 48,
                    DescriptionIdValue = 40,
                    ActionId = ActionType.Skill,
                    ClassId = Classes.Wizard
                },
                8 => new()
                {
                    Id = 8,
                    NameIdValue = 34,
                    DescriptionIdValue = 38,
                    ActionId = ActionType.Equip,
                    ClassId = Classes.Ranger
                },
                9 => new()
                {
                    Id = 9,
                    NameIdValue = 49,
                    DescriptionIdValue = 39,
                    ActionId = ActionType.Attack,
                    ClassId = Classes.Ranger
                },
                10 => new()
                {
                    Id = 10,
                    NameIdValue = 50,
                    DescriptionIdValue = 40,
                    ActionId = ActionType.Skill,
                    ClassId = Classes.Ranger
                },
                11 => new()
                {
                    Id = 11,
                    NameIdValue = 51,
                    DescriptionIdValue = 38,
                    ActionId = ActionType.Equip,
                    ClassId = Classes.Rogue
                },
                12 => new()
                {
                    Id = 12,
                    NameIdValue = 52,
                    DescriptionIdValue = 39,
                    ActionId = ActionType.Attack,
                    ClassId = Classes.Rogue
                },
                13 => new()
                {
                    Id = 13,
                    NameIdValue = 53,
                    DescriptionIdValue = 40,
                    ActionId = ActionType.Skill,
                    ClassId = Classes.Rogue
                },
                _ => null
            };
        }

        public static string GetAnimation(int animationId)
        {
            return animationId switch
            {
                2 or 7 or 10 or 13 => "Defend",
                _ => string.Empty,
            };
        }
    }
}
