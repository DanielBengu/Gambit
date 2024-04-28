using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventManager
{
    readonly GameObject character;

    readonly EffectsManager effectsManager;

    public EventManager(EventData eventData, GameObject character, EffectsManager effectsManager)
    {
        this.character = character;
        this.effectsManager = effectsManager;

        Animator animator = character.GetComponent<Animator>();
        animator.runtimeAnimatorController = CharacterManager.LoadAnimator(eventData.Description);

        SpriteRenderer charRenderer = character.GetComponent<SpriteRenderer>();
        Sprite charPng = CharacterManager.LoadSprite(eventData.Description);
        charRenderer.sprite = charPng;
    }

    public void CharacterTalk()
    {
        AnimationManager.PlayAnimation(character, AnimationManager.SpriteAnimation.UnitTalk, () => { }, effectsManager);
    }
}
