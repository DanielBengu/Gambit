using Assets.Resources.Scripts.Fight;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static AnimationManager;
using static FightManager;

public class Wizard : IClass
{
    static readonly int RIVER_DANCE_CARD_ID = 25;

    public Wizard(FightManager manager) : base(manager, CardsManager.Classes.Wizard)
    {
        CardsHandler = new(manager);
    }


    public override void ResetTurn()
    {
    }

    public override void PlayJack(FightUnit unit, FightUnit enemy)
    {
        ActionCard skillCard = GetRandomCard(ActionType.Skill);

        FightManager.AddCardToHand(skillCard);
    }

    public override void PlayQueen(FightUnit unit, FightUnit enemy)
    {
        ActionCard attackCard = GetRandomCard(ActionType.Attack);

        FightManager.AddCardToHand(attackCard);
    }

    public override void PlayKing(FightUnit unit, FightUnit enemy)
    {
        ActionCard attackCard = GetRandomCard(ActionType.Attack);
        ActionCard skillCard = GetRandomCard(ActionType.Skill);


        FightManager.AddCardToHand(attackCard);
        FightManager.AddCardToHand(skillCard);
    }

    public ActionCard GetRandomCard(ActionType cardType)
    {
        ActionCard[] cardsArray = ActionCardArchive.CARD_ARCHIVE
            .Where(c => c.ClassId == CardsManager.Classes.Wizard && c.ActionId == cardType && !c.SpecialCard).ToArray();

        int cardIndex = Random.Range(0, cardsArray.Length);
        ActionCard cardChosen = cardsArray[cardIndex];

        return cardChosen;
    }

    public override void PlayAce(FightUnit unit, FightUnit enemy)
    {
        var card = ActionCardArchive.CARD_ARCHIVE.Find(c => c.Id == RIVER_DANCE_CARD_ID);
        FightManager.AddCardToHand(card);
    }

    public override string GetCardText(CardType cardType)
    {
        switch (cardType)
        {
            case CardType.Default:
                return string.Empty;
            case CardType.Ace:
                return "UNLOCKS RIVER DANCE FOR THE TURN";
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
                return "ADDS A RANDOM SKILL";
            case CardType.Queen:
                return "ADDS A RANDOM ATTACK";
            case CardType.King:
                return "ADDS A RANDOM ATTACK AND SKILL";
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

        string currentAnimationName = string.Empty;

        // Get the Animator component from the GameObject
        if (obj.TryGetComponent<Animator>(out var animator))
        {
            AnimatorClipInfo[] clipInfo = animator.GetCurrentAnimatorClipInfo(0);

            if (clipInfo.Length > 0)
                currentAnimationName = clipInfo[0].clip.name;
        }

        //We don't play another animation when on river dance combo
        if (currentAnimationName == "RiverDance")
            return string.Empty;

        return GetAnimationName(SpriteAnimation.UnitDealDamage);
    }

    public int GetFirstRiverDanceDamage()
    {
        return 1;
    }

    public void HandleFirstRiverDanceDamage()
    {
        FightManager.DamageCharacter(FightManager.Enemy, FightManager.enemyObj, GetFirstRiverDanceDamage());
    }

    public void HandleSecondRiverDanceDamage()
    {
        FightManager.DamageCharacter(FightManager.Enemy, FightManager.enemyObj, 2);
    }

    public void HandleThirdRiverDanceDamage()
    {
        FightManager.DamageCharacter(FightManager.Enemy, FightManager.enemyObj, 3);
    }
}
