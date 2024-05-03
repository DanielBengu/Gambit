using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class DialogueManager
{
    readonly TextMeshProUGUI bubbleText;
    readonly VisualEffectsManager effectsManager;

    List<DialogueData> DialoguesJSON { get; set; }
    Dialogue dialogueEvent;

    public  DialogueManager(TextMeshProUGUI bubbleText, VisualEffectsManager effectsManager)
    {
        DialoguesJSON = JSONManager.GetFileFromJSON<DialogueContainer>(JSONManager.DIALOGUES_PATH).Dialogues;
        dialogueEvent = new Dialogue(true);
        this.bubbleText = bubbleText;
        this.effectsManager = effectsManager;
    }

    public string GetDialogue(int dialogueId, int dialogueIndex)
    {
        string[] dialogueBlock = DialoguesJSON.Find(d => d.Id == dialogueId).Dialogue;

        if (dialogueBlock == null || dialogueIndex > dialogueBlock.Length)
            return string.Empty;

        return dialogueBlock[dialogueIndex];
    }

    public void TickDialogue()
    {
        if (InputManager.IsClick())
        {
            dialogueEvent.HandleClick(bubbleText);
            return;
        }

        if (dialogueEvent.IsDialogueCompleted())
            return;

        dialogueEvent.TickDialogue(bubbleText);
    }

    public void SetupDialogue(Dialogue dialogue, GameObject character)
    {
        bubbleText.transform.parent.gameObject.SetActive(true);

        dialogueEvent = dialogue;

        AnimationManager.PlayAnimation(character, AnimationManager.SpriteAnimation.UnitTalk, () => { }, effectsManager);
    }
}