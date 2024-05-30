using static CardsManager;
using System.Collections.Generic;
using static FightUnit;
using System;

namespace Assets.Resources.Scripts.Fight
{
    public static class ActionCardArchive
    {
        public static List<ActionCard> CARD_ARCHIVE = new()
        {
            new()
            {
                Id = 0,
                NameIdValue = 32,
                DescriptionIdValue = 38,
                ActionId = ActionType.Equip,
                ClassId = Classes.Warrior,
                SpecialCard = false,
                Rarity = CardRarity.Basic
            },
            new()
            {
                Id = 1,
                NameIdValue = 33,
                DescriptionIdValue = 39,
                ActionId = ActionType.Attack,
                ClassId = Classes.Warrior,
                SpecialCard = false,
                Rarity = CardRarity.Basic
            },
            new()
            {
                Id = 2,
                NameIdValue = 35,
                DescriptionIdValue = 40,
                ActionId = ActionType.Skill,
                ClassId = Classes.Warrior,
                SpecialCard = false,
                Rarity = CardRarity.Basic
            },
            new()
            {
                Id = 3,
                NameIdValue = 36,
                DescriptionIdValue = 41,
                ActionId = ActionType.Modifier,
                ClassId = Classes.Basic,
                SpecialCard = false,
                Rarity = CardRarity.Basic
            },
            new()
            {
                Id = 4,
                NameIdValue = 37,
                DescriptionIdValue = 42,
                ActionId = ActionType.Modifier,
                ClassId = Classes.Basic,
                SpecialCard = false,
                Rarity = CardRarity.Basic
            },
            new()
            {
                Id = 5,
                NameIdValue = 46,
                DescriptionIdValue = 38,
                ActionId = ActionType.Equip,
                ClassId = Classes.Wizard,
                SpecialCard = false,
                Rarity = CardRarity.Basic
            },
            new()
            {
                Id = 6,
                NameIdValue = 47,
                DescriptionIdValue = 39,
                ActionId = ActionType.Attack,
                ClassId = Classes.Wizard,
                SpecialCard = false,
                Rarity = CardRarity.Basic
            },
            new()
            {
                Id = 7,
                NameIdValue = 48,
                DescriptionIdValue = 40,
                ActionId = ActionType.Skill,
                ClassId = Classes.Wizard,
                SpecialCard = false,
                Rarity = CardRarity.Basic
            },
            new()
            {
                Id = 8,
                NameIdValue = 34,
                DescriptionIdValue = 38,
                ActionId = ActionType.Equip,
                ClassId = Classes.Ranger,
                SpecialCard = false,
                Rarity = CardRarity.Basic
            },
            new()
            {
                Id = 9,
                NameIdValue = 49,
                DescriptionIdValue = 39,
                ActionId = ActionType.Attack,
                ClassId = Classes.Ranger,
                SpecialCard = false,
                Rarity = CardRarity.Basic
            },
            new()
            {
                Id = 10,
                NameIdValue = 50,
                DescriptionIdValue = 40,
                ActionId = ActionType.Skill,
                ClassId = Classes.Ranger,
                SpecialCard = false,
                Rarity = CardRarity.Basic
            },
            new()
            {
                Id = 11,
                NameIdValue = 51,
                DescriptionIdValue = 38,
                ActionId = ActionType.Equip,
                ClassId = Classes.Rogue,
                SpecialCard = false,
                Rarity = CardRarity.Basic
            },
            new()
            {
                Id = 12,
                NameIdValue = 52,
                DescriptionIdValue = 39,
                ActionId = ActionType.Attack,
                ClassId = Classes.Rogue,
                SpecialCard = false,
                Rarity = CardRarity.Basic
            },
            new()
            {
                Id = 13,
                NameIdValue = 53,
                DescriptionIdValue = 40,
                ActionId = ActionType.Skill,
                ClassId = Classes.Rogue,
                SpecialCard = false,
                Rarity = CardRarity.Basic
            },
            new()
            {
                Id = 14,
                NameIdValue = 55,
                DescriptionIdValue = 38,
                ActionId = ActionType.Equip,
                ClassId = Classes.Monk,
                SpecialCard = false,
                Rarity = CardRarity.Basic
            },
            new()
            {
                Id = 15,
                NameIdValue = 56,
                DescriptionIdValue = 39,
                ActionId = ActionType.Attack,
                ClassId = Classes.Monk,
                SpecialCard = false,
                Rarity = CardRarity.Basic
            },
            new()
            {
                Id = 16,
                NameIdValue = 57,
                DescriptionIdValue = 40,
                ActionId = ActionType.Skill,
                ClassId = Classes.Monk,
                SpecialCard = false,
                Rarity = CardRarity.Basic
            },
            new()
            {
                Id = 17,
                NameIdValue = 58,
                DescriptionIdValue = 38,
                ActionId = ActionType.Equip,
                ClassId = Classes.Crystal,
                SpecialCard = false,
                Rarity = CardRarity.Basic
            },
            new()
            {
                Id = 18,
                NameIdValue = 59,
                DescriptionIdValue = 39,
                ActionId = ActionType.Attack,
                ClassId = Classes.Crystal,
                SpecialCard = false,
                Rarity = CardRarity.Basic
            },
            new()
            {
                Id = 19,
                NameIdValue = 60,
                DescriptionIdValue = 40,
                ActionId = ActionType.Skill,
                ClassId = Classes.Crystal,
                SpecialCard = false,
                Rarity = CardRarity.Basic
            },
            new()
            {
                Id = 20,
                NameIdValue = 61,
                DescriptionIdValue = 38,
                ActionId = ActionType.Equip,
                ClassId = Classes.Trickster,
                SpecialCard = false,
                Rarity = CardRarity.Basic
            },
            new()
            {
                Id = 21,
                NameIdValue = 62,
                DescriptionIdValue = 39,
                ActionId = ActionType.Attack,
                ClassId = Classes.Trickster,
                SpecialCard = false,
                Rarity = CardRarity.Basic
            },
            new()
            {
                Id = 22,
                NameIdValue = 63,
                DescriptionIdValue = 40,
                ActionId = ActionType.Skill,
                ClassId = Classes.Trickster,
                SpecialCard = false,
                Rarity = CardRarity.Basic
            },
            new()
            {
                Id = 23,
                NameIdValue = 64,
                DescriptionIdValue = 65,
                ActionId = ActionType.Attack,
                ClassId = Classes.Ranger,
                SpecialCard = false,
                Rarity = CardRarity.Basic
            },
            new()
            {
                Id = 24,
                NameIdValue = 66,
                DescriptionIdValue = 67,
                ActionId = ActionType.Modifier,
                ClassId = Classes.Basic,
                SpecialCard = false,
                Rarity = CardRarity.Basic
            },
            new()
            {
                Id = 25,
                NameIdValue = 68,
                DescriptionIdValue = 69,
                ActionId = ActionType.Attack,
                ClassId = Classes.Wizard,
                SpecialCard = true,
                Rarity = CardRarity.Basic
            },
            new()
            {
                Id = 26,
                NameIdValue = 70,
                DescriptionIdValue = 71,
                ActionId = ActionType.Skill,
                ClassId = Classes.Wizard,
                SpecialCard = false,
                Rarity = CardRarity.Basic
            },            
            new()
            {
                Id = 27,
                NameIdValue = 72,
                DescriptionIdValue = 73,
                ActionId = ActionType.Attack,
                ClassId = Classes.Wizard,
                SpecialCard = false,
                Rarity = CardRarity.Basic
            },
            new()
            {
                Id = 28,
                NameIdValue = 72,
                DescriptionIdValue = 73,
                ActionId = ActionType.Attack,
                ClassId = Classes.Warrior,
                SpecialCard = false,
                Rarity = CardRarity.Rare
            },
            new()
            {
                Id = 29,
                NameIdValue = 76,
                DescriptionIdValue = 77,
                ActionId = ActionType.Attack,
                ClassId = Classes.Crystal,
                SpecialCard = true,
                Rarity = CardRarity.Gambit
            },
        };

        public static void ApplyEffect(int cardId, FightUnit unit, FightUnit enemy, FightManager manager)
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
                case 23:
                    ApplyModifier(enemy, Stats.Armor, 1, -1);
                    manager.DamageCharacter(enemy, manager.enemyObj, 1);
                    break;
                case 3:
                    AddScore(unit, 2);
                    break;
                case 4:
                    AddScore(unit, 6);
                    break;
                case 24:
                    AddScore(unit, 12);
                    break;
                case 25:
                    var unitManager = manager.playerObj.GetComponent<UnitAnimationManager>();
                    Wizard wizClass = unit.Class as Wizard;

                    unitManager.dictionaryCallback.Add("RiverDance1", wizClass.HandleFirstRiverDanceDamage);
                    unitManager.dictionaryCallback.Add("RiverDance2", wizClass.HandleSecondRiverDanceDamage);
                    unitManager.dictionaryCallback.Add("RiverDance3", wizClass.HandleThirdRiverDanceDamage);

                    manager.turnManager.SetAttacks(unit, enemy, wizClass.GetFirstRiverDanceDamage(), false);
                    break;
                case 26:
                    manager.HealCharacter(unit, 2);
                    break;
                case 27:
                    ApplyModifier(unit, Stats.Damage, 1, 1);
                    ApplyModifier(enemy, Stats.Armor, 2, -1);
                    break;
                case 28:
                    int armor = unit.FightArmor;
                    ApplyModifier(unit, Stats.Damage, 1, armor);
                    break;
                case 29:
                    var unitManagerCrystal = manager.playerObj.GetComponent<UnitAnimationManager>();
                    Crystal crystalClass = unit.Class as Crystal;

                    unitManagerCrystal.dictionaryCallback.Add("GemBlossom", crystalClass.HandleGemBlossom);
                    break;
            }
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

        public static string GetAnimation(int animationId)
        {
            return animationId switch
            {
                2 or 7 or 10 or 13 or 16 or 19 or 22 => "Defend",
                23 => "MeleeAttack",
                25 => "RiverDance",
                26 => "Heal",
                29 => "GemBlossom",
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

        static void ApplyModifier(FightUnit unit, Stats stat, int turnCount, int modifier)
        {
            unit.CurrentModifiers.Add(new(stat, turnCount, modifier));
        }

        static void AddScore(FightUnit unit, int modifier)
        {
            unit.currentScore += modifier;
        }
    }
}
