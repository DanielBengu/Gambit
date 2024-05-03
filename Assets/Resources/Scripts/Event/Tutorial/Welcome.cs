using System;
using System.Collections.Generic;
using UnityEngine;
using static ChoiceManager;

public class Welcome : EventParent
{
    public static string TUTORIAL_GUY_CHARACTER_NAME = "Tutorial Guy";

    public override int EventId { get => 0; }
    public override int NumberOfEvents { get => 5; }

    public Welcome(GameObject enemyObject, Action callback, DialogueManager dialogueManager, VisualEffectsManager effectsManager, GameObject choicesObject) : base(enemyObject, callback, dialogueManager, effectsManager, choicesObject)
    {
    }

    public override void LoadNextStep()
    {
        CurrentEventCount++;

        switch (CurrentEventCount)
        {
            case 0:
                LoadCharacter(CurrentEventCount);
                break;
            case 1:
                var dialogueList = LoadDialogue(CurrentEventCount);
                StartDialogue(dialogueList);
                break;
            case 2:
                LoadChoices(CurrentEventCount);
                break;
            case 3:
                EndEvent();
                break;
            case 4:
                break;
            default:
                Debug.LogError($"NO COUNT ({CurrentEventCount}) FOUND FOR EVENT {EventId}");
                break;
        }
    }

    public void LoadChoices(int choiceId)
    {
        switch (choiceId)
        {
            case 2:
                LoadChoiceManager(new()
                {
                    new(choices.transform.GetChild(0).gameObject, "FIRST CHOICE BLA BLA BLA BLA BLA", "Icon_Skull", new(){LoadNextStep }),
                    new(choices.transform.GetChild(1).gameObject, "SECOND CHOICE BLA BLA BLA BLA BLA", "Icon_Crown", new() { LoadNextStep }),
                    new(choices.transform.GetChild(2).gameObject, "THIRD CHOICE BLA BLA BLA BLA BLA", "Icon_Star", new() { LoadNextStep })
                });
                break;
            default:
                break;
        }
    }

    public override List<DialogueSection> LoadDialogue(int dialogue)
    {
        return dialogue switch
        {
            1 => new List<DialogueSection>()
            {
                new("Hello! Welcome to the tutorial of GAMBIT!\n Choose your powerup:", 0.05f, enemyObject),
                new("THIS IS A SUPER FAST TEXT OMG WHY IS IT SO FAAAAAST", 0.01f, enemyObject)
            },
            _ => new List<DialogueSection>(){
                new("", 0f, enemyObject)
            },
        };
    }

    public override void LoadCharacter(int character)
    {
        switch (character)
        {
            case 0:
                SetupCharacter(TUTORIAL_GUY_CHARACTER_NAME);
                break;
            default:
                break;
        }
    }
}
