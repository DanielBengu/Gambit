using Assets.Resources.Scripts.Fight;
using System;
using System.Collections.Generic;
using UnityEngine;

public class KaldorBoss : EventParent
{
    public string FollowerName { get => dialogueManager.languageManager.GetText(26); }

    public override int EventId { get => 0; }
    public override int NumberOfEvents { get => 5; }

    public KaldorBoss(GameObject enemyObject, Transform parent, Action callback, DialogueManager dialogueManager, VisualEffectsManager effectsManager, GameObject choicesObject, GameManager gameManager, Transform choicePosition) : base(enemyObject, parent, callback, dialogueManager, effectsManager, choicesObject, gameManager, choicePosition)
    {
    }

    public async override void LoadNextStep()
    {
        CurrentEventCount++;

        switch (CurrentEventCount)
        {
            case 0:
                LoadCharacter(0);
                await ExecuteWithDelay(LoadNextStep, 2f);
                break;
            case 1:
                var dialogueList = LoadDialogue(0);
                StartDialogue(dialogueList, LoadNextStep);
                break;
            case 2:
                LoadFight(0);
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
            0 => new()
            {
                new(dialogueManager.languageManager.GetText(27), 0.05f, enemyObject),
                new(dialogueManager.languageManager.GetText(28), 0.05f, enemyObject),
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
