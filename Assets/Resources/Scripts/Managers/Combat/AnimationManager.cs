using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class AnimationManager
{
    public static void PlayAnimation(GameObject unit, SpriteAnimation animation, Action callback)
    {
        string animationToPlay = GetAnimationName(animation);

        Animator animator = unit.GetComponent<Animator>();

        if (!animator.HasState(0, Animator.StringToHash(animationToPlay)))
        {
            if(animation != SpriteAnimation.UnitIntro)
                Debug.LogError("Animation '" + animationToPlay + "' not found in Animator controller of GameObject: " + unit.name);

            callback();
            return;
        }

        animator.Play(animationToPlay, 0, 0f);

        UnitAnimationManager animationManager = unit.GetComponent<UnitAnimationManager>();

        animationManager.SaveAnimationCallback(animationToPlay, callback);
    }

    public static void PlayCustomAnimation(GameObject unit, string animation, Action callback)
    {
        if (animation == string.Empty)
            return;

        Debug.Log($"Playing animation {animation} for unit {unit.name}");

        Animator animator = unit.GetComponent<Animator>();

        animator.Play(animation, 0, 0f);

        UnitAnimationManager animationManager = unit.GetComponent<UnitAnimationManager>();

        animationManager.SaveAnimationCallback(animation, callback);
    }

    public static string GetAnimationOfObject(GameObject obj)
    {
        if (!obj.TryGetComponent<Animator>(out var animator))
        {
            Debug.LogError("GameObject does not have an Animator component.");
            return null;
        }

        AnimatorClipInfo[] clipInfos = animator.GetCurrentAnimatorClipInfo(0);

        // Check if any clip is playing
        if (clipInfos.Length == 0)
        {
            return string.Empty;
        }

        // Return the name of the currently playing animation clip
        return clipInfos[0].clip.name;
    }

    public static string GetAnimationName(SpriteAnimation animation)
    {
        return animation switch
        {
            SpriteAnimation.UnitDealDamage => "DealDamage",
            SpriteAnimation.UnitTakingDamage => "TakeDamage",
            SpriteAnimation.Idle => "Idle",
            SpriteAnimation.UnitDeath => "Death",
            SpriteAnimation.UnitTalk => "Talk_Start",
            SpriteAnimation.UnitIntro => "Intro",
            SpriteAnimation.UnitDefend => "Defend",
            SpriteAnimation.ChestOpen => "ChestOpen",
            SpriteAnimation.Crit => "Crit",
            _ => string.Empty,
        };
    }

    public enum SpriteAnimation
    {
        Idle,
        UnitTakingDamage,
        UnitDealDamage,
        UnitDeath,
        UnitTalk,
        UnitIntro,
        UnitDefend,
        ChestOpen,
        Crit
    }
}
