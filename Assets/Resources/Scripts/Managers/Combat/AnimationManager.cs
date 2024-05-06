using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class AnimationManager
{
    public static void PlayAnimation(GameObject unit, SpriteAnimation animation, Action callback, VisualEffectsManager effectsManager)
    {
        string animationToPlay = GetAnimationName(animation);

        Animator animator = unit.GetComponent<Animator>();
        animator.Play(animationToPlay);

        UnitAnimationManager animationManager = unit.GetComponent<UnitAnimationManager>();

        animationManager.SaveAnimationCallback(animationToPlay, callback);
    }

    public static void PlayCustomAnimation(GameObject unit, string animation, Action callback, VisualEffectsManager effectsManager)
    {
        Animator animator = unit.GetComponent<Animator>();
        animator.Play(animation);

        UnitAnimationManager animationManager = unit.GetComponent<UnitAnimationManager>();

        animationManager.SaveAnimationCallback(animation, callback);
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
