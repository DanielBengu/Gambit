using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DialogueManager
{
    List<DialogueData> Dialogues { get; set; }

    public DialogueManager()
    {
        Dialogues = JSONManager.GetFileFromJSON<DialogueContainer>(JSONManager.DIALOGUES_PATH).Dialogues;
    }

    public string GetDialogue(int dialogueId, int dialogueIndex)
    {
        string[] dialogueBlock = Dialogues.Find(d => d.Id == dialogueId).Dialogue;

        if (dialogueBlock == null || dialogueIndex > dialogueBlock.Length)
            return string.Empty;

        return dialogueBlock[dialogueIndex];
    }
}