using System;
using System.Collections.Generic;
using UnityEngine;
using static Assets.Resources.Scripts.Fight.CardsHandler;

public class Alchemist : EventParent
{
    public int HEALTH_POTION_COST = 1000;
    public int POISON_POTION_COST = 200;

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
                    new(choices.transform.GetChild(0).gameObject, string.Format(dialogueManager.languageManager.GetText(44), 2), "Icon_Star", ChoiceManager.Choice.ChoiceType.ActionCard, new object[1] { HEALTH_POTION_COST }, new()   { BuyHealthPotion }),
                    new(choices.transform.GetChild(2).gameObject, string.Format(dialogueManager.languageManager.GetText(45), 2), "Icon_Skull", ChoiceManager.Choice.ChoiceType.ActionCard, new object[1] { POISON_POTION_COST }, new()  { BuyPoisonPotion }),
                    new(choices.transform.GetChild(1).gameObject, dialogueManager.languageManager.GetText(43), "Icon_Crown", ChoiceManager.Choice.ChoiceType.Standard, new object[0], new()  { LoadNextStep }),
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
        var currentRun = gameManager.playerData.CurrentRun;

        if (currentRun.GoldAmount < HEALTH_POTION_COST)
        {
            gameManager.FightManager.gameUIManager.HandleInsufficientFunds();
            LoadNextStep();
            return;
        }

        currentRun.GoldAmount -= HEALTH_POTION_COST;

        currentRun.CardList.Add(new()
        {
            cardType = CardType.Potion,
            classId = CardsManager.Classes.Basic,
            id = (int)PotionType.Health,
            value = 2,
            destroyOnPlay = true
        });

        LoadNextStep();
    }

    public void BuyPoisonPotion()
    {
        var currentRun = gameManager.playerData.CurrentRun;

        if (currentRun.GoldAmount < POISON_POTION_COST)
        {
            gameManager.FightManager.gameUIManager.HandleInsufficientFunds();
            LoadNextStep();
            return;
        }

        currentRun.GoldAmount -= POISON_POTION_COST;


        currentRun.CardList.Add(new()
        {
            cardType = CardType.Potion,
            classId = CardsManager.Classes.Basic,
            id = (int)PotionType.Damage,
            value = 2,
            destroyOnPlay = true
        });

        LoadNextStep();
    }
}
