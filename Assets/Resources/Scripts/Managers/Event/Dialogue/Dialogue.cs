using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class Dialogue
{
    public List<DialogueSection> sections;
    readonly Action callback;
    bool dialogueCompleted;
    float timer;
    int currentIndex;

    public Dialogue(List<DialogueSection> sections, Action callback)
    {
        timer = 0;
        currentIndex = 0;
        dialogueCompleted = false;
        this.sections = sections;
        this.callback = callback;
    }

    public Dialogue(List<string> dialogues, float speed, GameObject character, Action callback)
    {
        timer = 0;
        currentIndex = 0;
        dialogueCompleted = false;
        sections = new();
        this.callback = callback;

        foreach (var dialogue in dialogues)
            sections.Add(new DialogueSection(dialogue, speed, character));
    }

    //USED ONLY DURING STARTUP AS A FIRST VARIABLE VALORIZATION, DOESN'T REPRESENT A REAL DIALOGUE
    public Dialogue(bool isForcedDialogueCompletion)
    {
        dialogueCompleted = isForcedDialogueCompletion;
        timer = 0;
        currentIndex = 0;
        sections = new();
        callback = () => { };
    }

    public bool IsDialogueCompleted()
    {
        return dialogueCompleted;
    }

    public void TickDialogue(TextMeshProUGUI bubbleText)
    {
        DialogueSection dialogue = sections.First();

        if (IsDialogueCompleted(dialogue))
        {
            HandleDialogueCompletion(dialogue);
            return;
        }

        StandardTickDialogue(dialogue, bubbleText);
    }

    public void HandleClick(TextMeshProUGUI bubbleText)
    {
        if (IsDialogueCompleted())
        {
            bubbleText.text = string.Empty;
            callback();
            return;
        }

        if (IsSectionCompleted() && InputManager.IsClick())
        {
            SetupNextDialogue(bubbleText);
            return;
        }

        SkipDialogue(sections[0], bubbleText);
    }

    void SkipDialogue(DialogueSection dialogue, TextMeshProUGUI bubbleText)
    {
        bubbleText.text = dialogue.text;

        HandleDialogueCompletion(dialogue);
    }

    bool IsDialogueCompleted(DialogueSection dialogue)
    {
        return currentIndex == dialogue.text.Length || dialogue.IsCompleted();
    }

    bool IsCharacterTimerCompleted(float dialogueSpeed)
    {
        return timer >= dialogueSpeed;
    }

    void HandleDialogueCompletion(DialogueSection section)
    {
        section.SetCompletion(true);

        if (sections.Count <= 1)
        {
            dialogueCompleted = true;
        }
    }

    void HandleCharacterTick(DialogueSection section, TextMeshProUGUI bubbleText)
    {
        bubbleText.text += section.text[currentIndex];
        currentIndex++;

        timer = 0f;
    }

    void StandardTickDialogue(DialogueSection dialogue, TextMeshProUGUI bubbleText)
    {
        timer += Time.deltaTime;

        if (IsCharacterTimerCompleted(dialogue.speed))
            HandleCharacterTick(dialogue, bubbleText);
    }

    public bool IsSectionCompleted()
    {
        return sections.First().IsCompleted();
    }

    public void SetupNextDialogue(TextMeshProUGUI bubbleText)
    {
        sections.RemoveAt(0);

        currentIndex = 0;
        timer = 0;
        bubbleText.text = string.Empty;
    }
}