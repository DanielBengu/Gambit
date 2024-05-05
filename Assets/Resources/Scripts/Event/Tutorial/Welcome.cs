using System;
using System.Collections.Generic;
using UnityEngine;

public class Welcome : EventParent
{
    public string TutorialGuyName { get => dialogueManager.languageManager.GetText(16); }

    public override int EventId { get => 0; }
    public override int NumberOfEvents { get => 5; }

    public Welcome(GameObject enemyObject, Action callback, DialogueManager dialogueManager, VisualEffectsManager effectsManager, GameObject choicesObject, GameManager gameManager) : base(enemyObject, callback, dialogueManager, effectsManager, choicesObject, gameManager)
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
                StartDialogue(dialogueList, LoadNextStep);
                break;
            case 2:
                LoadChoices(CurrentEventCount);
                break;
            case 3:
                var dialogueListThree = LoadDialogue(CurrentEventCount);
                StartDialogue(dialogueListThree, LoadNextStep);
                break;
            case 4:
                EndEvent();
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
                    new(choices.transform.GetChild(0).gameObject, dialogueManager.languageManager.GetText(11), "Icon_Star", new(){ RaiseArmor, LoadNextStep }),
                    new(choices.transform.GetChild(1).gameObject, dialogueManager.languageManager.GetText(12), "Icon_Crown", new() { RaiseMaxHP, LoadNextStep }),
                    new(choices.transform.GetChild(2).gameObject, dialogueManager.languageManager.GetText(13), "Icon_Skull", new() { gameManager.MakePlayerDie })
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
            1 => new()
            {
                new(dialogueManager.languageManager.GetText(14), 0.05f, enemyObject),
                new(dialogueManager.languageManager.GetText(15), 0.01f, enemyObject)
            },
            3 => new()
            {
                new(dialogueManager.languageManager.GetText(17), 0.05f, enemyObject),
            },
            _ => new()
            {
                new(string.Empty, 0f, enemyObject)
            },
        };
    }

    public override void LoadCharacter(int character)
    {
        switch (character)
        {
            case 0:
                SetupCharacter(TutorialGuyName, true);
                break;
            default:
                break;
        }
    }

    public override void LoadFight(int fight)
    {
        switch (fight)
        {
            case 0:
                break;
            default:
                break;
        }
    }

    public void RaiseArmor()
    {
        gameManager.playerData.UnitData.Armor += 2;
        gameManager.gameUIManager.UpdateArmor(FightManager.Character.Player, gameManager.playerData.UnitData.Armor);
    }

    public void RaiseMaxHP()
    {
        gameManager.playerData.UnitData.MaxHP += 5;
        gameManager.playerData.UnitData.CurrentHP += 5;
        gameManager.gameUIManager.UpdateUnitHP(FightManager.Character.Player, gameManager.playerData.UnitData.CurrentHP, gameManager.playerData.UnitData.MaxHP);
    }
}
