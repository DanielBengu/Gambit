using System;
using System.Collections.Generic;
using UnityEngine;
using static Assets.Resources.Scripts.Fight.CardsHandler;

public class Alchemist : EventParent
{
    public string TutorialGuyName { get => dialogueManager.languageManager.GetText(17); }

    public override int EventId { get => 0; }
    public override int NumberOfEvents { get => 5; }

    public Alchemist(GameObject enemyObject, Transform parent, Action callback, DialogueManager dialogueManager, VisualEffectsManager effectsManager, GameObject choicesObject, GameManager gameManager, Transform choicePosition) : base(enemyObject, parent, callback, dialogueManager, effectsManager, choicesObject, gameManager, choicePosition)
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
                var dialogueList = LoadDialogue(0);
                StartDialogue(dialogueList, LoadNextStep);
                break;
            case 2:
                LoadChoices(2);
                break;
            case 3:
                var dialogueListThree = LoadDialogue(3);
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
                    new(choices.transform.GetChild(0).gameObject, string.Format(dialogueManager.languageManager.GetText(44), 2), "Icon_Star", new()   { BuyHealthPotion, LoadNextStep }),
                    new(choices.transform.GetChild(2).gameObject, string.Format(dialogueManager.languageManager.GetText(45), 2), "Icon_Skull", new()  { BuyPoisonPotion, LoadNextStep }),
                    new(choices.transform.GetChild(1).gameObject, dialogueManager.languageManager.GetText(43), "Icon_Crown", new()  { LoadNextStep }),
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
            0 => new()
            {
                new(dialogueManager.languageManager.GetText(25), 0.05f, enemyObject),
            },
            3 => new()
            {
                new(dialogueManager.languageManager.GetText(30), 0.05f, enemyObject),
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

    public void BuyHealthPotion()
    {
        gameManager.playerData.CurrentRun.CardList.Add(new()
        {
            cardType = CardType.Potion,
            classId = CardsManager.Classes.Basic,
            id = (int)PotionType.Health,
            value = 2,
            destroyOnPlay = true
        });
    }

    public void BuyPoisonPotion()
    {
        gameManager.playerData.CurrentRun.CardList.Add(new()
        {
            cardType = CardType.Potion,
            classId = CardsManager.Classes.Basic,
            id = (int)PotionType.Damage,
            value = 2,
            destroyOnPlay = true
        });
    }
}
