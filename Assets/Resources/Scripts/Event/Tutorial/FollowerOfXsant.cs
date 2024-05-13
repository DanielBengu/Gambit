using Assets.Resources.Scripts.Fight;
using System;
using System.Collections.Generic;
using UnityEngine;

public class FollowerOfXsant : EventParent
{
    public string FollowerName { get => dialogueManager.languageManager.GetText(18); }

    public override int EventId { get => 0; }
    public override int NumberOfEvents { get => 5; }

    public FollowerOfXsant(GameObject enemyObject, Transform parent, Action callback, DialogueManager dialogueManager, VisualEffectsManager effectsManager, GameObject choicesObject, GameManager gameManager, Transform choicePosition) : base(enemyObject, parent, callback, dialogueManager, effectsManager, choicesObject, gameManager, choicePosition)
    {
    }

    public async override void LoadNextStep()
    {
        CurrentEventCount++;

        switch (CurrentEventCount)
        {
            case 0:
                LoadCharacter(CurrentEventCount);
                await ExecuteWithDelay(LoadNextStep, 2f);
                break;
            case 1:
                var dialogueList = LoadDialogue(CurrentEventCount);
                StartDialogue(dialogueList, LoadNextStep);
                break;
            case 2:
                LoadChoices(0);
                break;
            case 3:
                var dialogueListFriendly = LoadDialogue(3);
                StartDialogue(dialogueListFriendly, LoadNextStep);
                break;
            case 4:
                EndEvent();
                break;
            case 20:
                var dialogueListFight = LoadDialogue(2);
                StartDialogue(dialogueListFight, LoadNextStep);
                break;
            case 21:
                AnimationManager.PlayCustomAnimation(enemyObject, "IntroToIdle", LoadNextStep);
                break;
            case 22:
                LoadFight(0);
                break;
            case 23:
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
            case 0:
                LoadChoiceManager(new()
                {
                    new(choices.transform.GetChild(0).gameObject, dialogueManager.languageManager.GetText(20), "Icon_Star", new(){ SwitchToFight }),
                    new(choices.transform.GetChild(2).gameObject, dialogueManager.languageManager.GetText(22), "Icon_Skull", new() { SwitchToFight }),
                    new(choices.transform.GetChild(1).gameObject, dialogueManager.languageManager.GetText(21), "Icon_Crown", new() { LoadNextStep }),
                });
                break;
            default:
                break;
        }
    }

    public void SwitchToFight()
    {
        CurrentEventCount = 19;
        LoadNextStep();
    }

    public override List<DialogueSection> LoadDialogue(int dialogue)
    {
        return dialogue switch
        {
            1 => new()
            {
                new(dialogueManager.languageManager.GetText(19), 0.05f, enemyObject),
            },
            2 => new()
            {
                new(dialogueManager.languageManager.GetText(23), 0.05f, enemyObject),
            },
            3 => new()
            {
                new(dialogueManager.languageManager.GetText(24), 0.05f, enemyObject),
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
                SetupCharacter(FollowerName, false);
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
                EnemyList enemyList = JSONManager.GetFileFromJSON<EnemyList>(JSONManager.ENEMIES_PATH);
                EnemyData enemy = enemyList.Enemies.Find(e => e.Id == 2);
                gameManager.PlayCombat(enemy, gameManager.SetNextSectionButtonClick);
                gameManager.FightManager.SetupFightUIAndStartGame();
                break;
            default:
                break;
        }
    }
}
