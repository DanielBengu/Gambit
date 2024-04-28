using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class EventManager
{
    readonly GameObject character;
    readonly TextMeshProUGUI bubbleText;

    readonly EffectsManager effectsManager;

    string dialogue;

    public float textSpeed = 0.05f; // Adjust the speed at which the text appears
    private int currentIndex = 0; // Index to track the current character being displayed
    private float timer = 0f; // Timer to track the time since the last character was added


    public EventManager(EventData eventData, GameObject character, EffectsManager effectsManager, TextMeshProUGUI textBubble)
    {
        this.character = character;
        this.effectsManager = effectsManager;

        Animator animator = character.GetComponent<Animator>();
        animator.runtimeAnimatorController = CharacterManager.LoadAnimator(eventData.Description);

        SpriteRenderer charRenderer = character.GetComponent<SpriteRenderer>();
        Sprite charPng = CharacterManager.LoadSprite(eventData.Description);
        charRenderer.sprite = charPng;

        bubbleText = textBubble;
    }

    public void Update()
    {
        if (dialogue == null || dialogue == string.Empty)
            return;

        if(currentIndex == dialogue.Length)
        {
            currentIndex = 0;
            dialogue = string.Empty;
            return;
        }

        timer += Time.deltaTime;

        // Check if it's time to display the next character
        if (timer >= textSpeed)
        {
            // Add the next character to the text
            bubbleText.text += dialogue[currentIndex];
            currentIndex++;

            // Reset the timer
            timer = 0f;
        }
    }

    public void CharacterTalk()
    {
        bubbleText.transform.parent.gameObject.SetActive(true);

        dialogue = "Hello! Welcome to the tutorial of GAMBIT!\n Choose your powerup:";

        AnimationManager.PlayAnimation(character, AnimationManager.SpriteAnimation.UnitTalk, () => { }, effectsManager);
    }
}