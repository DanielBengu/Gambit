using System;
using System.Collections.Generic;
using UnityEngine;

public static class AnimationManager
{
    public static void PlayAnimation(GameObject unit, SpriteAnimation animation, Action callback, VisualEffectsManager effectsManager)
    {
        Animator animator = unit.GetComponent<Animator>();
        string animationToPlay = GetAnimationName(animation);

        animator.Play(animationToPlay);
        effectsManager.animatingSprites.Add(new(animator, animation, callback));
    }

    public static string GetAnimationName(SpriteAnimation animation)
    {
        return animation switch
        {
            SpriteAnimation.UnitDealingDamage => "DealDamage",
            SpriteAnimation.UnitTakingDamage => "TakeDamage",
            SpriteAnimation.UnitIdle => "Idle",
            SpriteAnimation.UnitDeath => "Death",
            SpriteAnimation.UnitTalk => "Talk_Start",
            SpriteAnimation.UnitIntro => "Intro",
            _ => string.Empty,
        };
    }

    public enum SpriteAnimation
    {
        UnitIdle,
        UnitTakingDamage,
        UnitDealingDamage,
        UnitDeath,
        UnitTalk,
        UnitIntro
    }
}
