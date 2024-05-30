using Assets.Resources.Scripts.Fight;
using System;
using System.Collections.Generic;
using UnityEngine;
using static AnimationManager;
using static FightManager;

public class Monk : IClass
{
    public Monk(FightManager manager): base(manager, CardsManager.Classes.Monk)
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
    }
}
