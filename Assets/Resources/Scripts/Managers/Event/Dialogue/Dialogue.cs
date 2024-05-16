using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
            if(bubbleText.text != string.Empty)
                callback();
            bubbleText.text = string.Empty;
            bubbleText.transform.parent.gameObject.SetActive(false);

            return;
        }

        if (IsSectionCompleted() && InputManager.IsClickDown())
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
        if (currentIndex > section.text.Length)
        {
            currentIndex = section.text.Length;
            return;
        }

        char nextChar = section.text[currentIndex];

        if(nextChar == '<')
        {
            ApplyHTML(section, bubbleText);
            return;
        }

        bubbleText.text += nextChar;
        currentIndex++;

        timer = 0f;
    }

    void ApplyHTML(DialogueSection section, TextMeshProUGUI bubbleText)
    {
        string remainingText = section.text[currentIndex..];

        string htmlCommand = ParseHTMLCommand(remainingText);

        if (!string.IsNullOrEmpty(htmlCommand))
        {
            bubbleText.text += htmlCommand;

            currentIndex += htmlCommand.Length;
            timer = 0f;
        }
    }

    string ParseHTMLCommand(string text)
    {
        StringBuilder parsedHTML = new();
        Stack<string> tagStack = new();

        int currentIndex = 0;
        while (currentIndex < text.Length)
        {
            if (text[currentIndex] == '<')
            {
                int endIndex = text.IndexOf('>', currentIndex);
                if (endIndex != -1)
                {
                    string htmlTag = text.Substring(currentIndex, endIndex - currentIndex + 1);
                    if (htmlTag.StartsWith("</"))
                    {
                        // Pop the corresponding opening tag and append both to the parsedHTML
                        if (tagStack.Count > 0)
                        {
                            string openingTag = tagStack.Pop();
                            parsedHTML.Append(openingTag);
                            parsedHTML.Append(htmlTag);
                        }
                    }
                    else
                    {
                        // Push opening tag onto the stack
                        tagStack.Push(htmlTag);
                        parsedHTML.Append(htmlTag); // Append the opening tag to the parsedHTML
                    }

                    currentIndex = endIndex + 1; // Move currentIndex past the tag
                    continue;
                }
            }

            // If it's not an HTML tag, simply append the character to the parsedHTML
            parsedHTML.Append(text[currentIndex]);
            currentIndex++;
        }

        return parsedHTML.ToString();
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