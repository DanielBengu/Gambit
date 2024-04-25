using System;
using System.Collections.Generic;
using UnityEngine;

public static class AnimationManager
{
    static readonly string unitIdleName = "Idle";

    public static void PlayAnimation(GameObject unit, SpriteAnimation animation, Action callback, EffectsManager effectsManager)
    {
        Animator animator = unit.GetComponent<Animator>();
        string animationToPlay = string.Empty;

        switch (animation)
        {
            case SpriteAnimation.UnitDealingDamage:
                animationToPlay = "DealDamage";
                break;
            case SpriteAnimation.UnitTakingDamage:
                animationToPlay = "TakeDamage";
                break;
            case SpriteAnimation.UnitIdle:
                animationToPlay = unitIdleName;
                break;
        }

        animator.Play(animationToPlay);
        effectsManager.animatingSprites.Add(new(animator, animation, callback));
    }

    public static string GetIdleAnimationName()
    {
        return unitIdleName;
    }

    public enum SpriteAnimation
    {
        UnitIdle,
        UnitTakingDamage,
        UnitDealingDamage
    }
}
