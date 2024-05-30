using Assets.Resources.Scripts.Fight;
using System;
using System.Collections.Generic;
using UnityEngine;
using static AnimationManager;
using static FightManager;

public class Crystal : IClass
{
    private readonly Dictionary<GemType, int> _gemFragments = new()
    {
        { GemType.Ruby, 0 },
        { GemType.Emerald, 0 },
        { GemType.Sapphire, 0 },
        { GemType.Silver, 0 },
        { GemType.Gold, 0 },
        { GemType.Default, 0 }
    };

    public int Rubies { get { return _gemFragments[GemType.Ruby] / 3; } set { _gemFragments[GemType.Ruby] += value; } }
    public int Emeralds { get { return _gemFragments[GemType.Emerald] / 3; } set { _gemFragments[GemType.Emerald] += value; } }
    public int Sapphires { get { return _gemFragments[GemType.Sapphire] / 3; } set { _gemFragments[GemType.Sapphire] += value; } }
    public int Silvers { get { return _gemFragments[GemType.Silver] / 3; } set { _gemFragments[GemType.Silver] += value; } }
    public int Gold { get { return _gemFragments[GemType.Gold] / 3; } set { _gemFragments[GemType.Gold] += value; } }

    public Crystal(FightManager manager): base(manager, CardsManager.Classes.Crystal)
    {
        CardsHandler = new(manager);
    }

    public override void ResetTurn()
    {
    }

    public override void PlayJack(FightUnit unit, FightUnit enemy)
    {
        unit.CurrentModifiers.Add(new(FightUnit.Stats.Armor, 1, 1));
    }

    public override void PlayQueen(FightUnit unit, FightUnit enemy)
    {
        unit.CurrentModifiers.Add(new(FightUnit.Stats.Armor, 1, 2));
    }

    public override void PlayKing(FightUnit unit, FightUnit enemy)
    {
        unit.CurrentModifiers.Add(new(FightUnit.Stats.Armor, 1, 3));
    }

    public override void PlayAce(FightUnit unit, FightUnit enemy)
    {
        unit.CurrentModifiers.Add(new(FightUnit.Stats.Attacks, 1, 2));
    }

    public override string GetCardText(CardType cardType)
    {
        switch (cardType)
        {
            case CardType.Default:
                return string.Empty;
            case CardType.Ace:
                return "GAIN 2 EXTRA ATTACKS";
            case CardType.One:
                return "1";
            case CardType.Two:
                return "2";
            case CardType.Three:
                return "3";
            case CardType.Four:
                return "4";
            case CardType.Five:
                return "5";
            case CardType.Six:
                return "6";
            case CardType.Jack:
                return "GAIN 1 ARMOR";
            case CardType.Queen:
                return "GAIN 2 ARMOR";
            case CardType.King:
                return "GAIN 3 ARMOR";
            case CardType.Potion:
                return "POTION";
            default:
                Debug.LogError($"Card {cardType} not implemented for {Class}");
                return string.Empty;
        }
    }

    public override string GetAttackAnimation(FightUnit unit, Queue<AttackStruct> attacks, GameObject obj)
    {
        if (unit.status == CharacterStatus.StandingOnCrit)
            return GetAnimationName(SpriteAnimation.Crit);

        return GetAnimationName(SpriteAnimation.UnitDealDamage);
    }

    public override void HandleActionCardPlayed(ActionCard card)
    {
        GemType gemFragment = card.ActionId switch
        {
            ActionType.Equip => GemType.Emerald,
            ActionType.Attack => GemType.Ruby,
            ActionType.Skill => GemType.Sapphire,
            ActionType.Artifact => GemType.Gold,
            ActionType.Modifier => GemType.Silver,
            _ => GemType.Default,
        };

        _gemFragments[gemFragment]++;
    }

    public void HandleGemBlossom()
    {

    }

    public enum GemType
    {
        Emerald,
        Ruby,
        Sapphire,
        Silver,
        Gold,
        Default
    }
}
