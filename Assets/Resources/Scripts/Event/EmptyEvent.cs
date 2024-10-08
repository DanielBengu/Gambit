using System;
using System.Collections.Generic;
using UnityEngine;

public class EmptyEvent : EventParent
{
    public static string TUTORIAL_GUY_CHARACTER_NAME = "Tutorial Guy";

    public override int EventId { get => 0; }
    public override int NumberOfEvents { get => 5; }

    public EmptyEvent(GameObject enemyObject, Transform parent,  Action callback, DialogueManager dialogueManager, VisualEffectsManager effectsManager, GameObject choicesObject, GameManager gameManager, Transform choicePosition) : base(enemyObject, parent, callback, dialogueManager, effectsManager, choicesObject, gameManager, choicePosition)
    {
    }

    public override void LoadNextStep()
    {
        CurrentEventCount++;

        switch (CurrentEventCount)
        {
            case 0:
                EndEvent();
                break;
            case 1:
                break;
            default:
                Debug.LogError($"NO COUNT ({CurrentEventCount}) FOUND FOR EVENT {EventId}");
                break;
        }
    }

    public override List<DialogueSection> LoadDialogue(int dialogue)
    {
        return new List<DialogueSection>(){
            new(string.Empty, 0.0f, enemyObject) 
        };
    }

    public override void LoadCharacter(int character)
    {
    }

    public override void LoadFight(int fight)
    {
    }
}
