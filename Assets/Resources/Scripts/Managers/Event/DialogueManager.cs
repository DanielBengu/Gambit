using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class DialogueManager
{
    readonly TextMeshProUGUI bubbleText;
    public readonly LanguageManager languageManager;

    Dialogue dialogueEvent;

    public  DialogueManager(TextMeshProUGUI bubbleText, LanguageManager languageManager)
    {
        dialogueEvent = new Dialogue(true);
        this.bubbleText = bubbleText;
        this.languageManager = languageManager;
    }

    public void TickDialogue()
    {
        if (InputManager.IsClickDown())
        {
            dialogueEvent.HandleClick(bubbleText);
            return;
        }

        if (dialogueEvent.IsDialogueCompleted()) {
            return;
        }

        dialogueEvent.TickDialogue(bubbleText);
    }

    public void SetupDialogue(Dialogue dialogue, GameObject character)
    {
        bubbleText.transform.parent.gameObject.SetActive(true);

        dialogueEvent = dialogue;

        AnimationManager.PlayAnimation(character, AnimationManager.SpriteAnimation.Idle, () => { });
    }
}