using Assets.Resources.Scripts.Fight;
using System;
using System.Collections.Generic;
using UnityEngine;
using static AnimationManager;
using static FightManager;

internal class Basic : IClass
{
    public Basic(FightManager manager) : base(manager, CardsManager.Classes.Basic)
    {
        CardsHandler = new(manager);
    }

    public override void ResetTurn()
    {
    }

    public override void PlayJack(FightUnit unit, FightUnit enemy)
    {
    }

    public override void PlayQueen(FightUnit unit, FightUnit enemy)
    {
    }

    public override void PlayKing(FightUnit unit, FightUnit enemy)
    {
    }

    public override void PlayAce(FightUnit unit, FightUnit enemy)
    {

    }

    public override string GetCardText(CardType cardType)
    {
        switch (cardType)
        {
            case CardType.Default:
                return string.Empty;
            case CardType.Ace:
                return "ACE";
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
                return "JACK";
            case CardType.Queen:
                return "QUEEN";
            case CardType.King:
                return "KING";
            default:
                Debug.LogError($"Card {cardType} not implemented for {Class}");
                return string.Empty;
        }
    }

    public override Sprite GetCardIcon(CardType cardType)
    {
        return null;
    }

    public override string GetAttackAnimation(FightUnit unit, Queue<AttackStruct> attacks, GameObject obj)
    {
        /*
        if (unit.status == CharacterStatus.StandingOnCrit)
            return SpriteAnimation.Crit.ToString();*/

        return GetAnimationName(SpriteAnimation.UnitDealDamage);
    }
}
