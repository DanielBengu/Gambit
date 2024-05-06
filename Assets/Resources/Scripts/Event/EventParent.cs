using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using static ChoiceManager;

public abstract class EventParent
{
    public abstract int EventId { get; }
    public abstract int NumberOfEvents { get; }

    public int CurrentEventCount { get; set; } = -1;

    internal readonly DialogueManager dialogueManager;
    internal ChoiceManager choiceManager;
    internal GameManager gameManager;
    internal readonly VisualEffectsManager effectsManager;

    public Action endEventCallback;
    internal readonly GameObject enemyObject;
    internal readonly GameObject choices;
    internal readonly Transform choicePosition;

    public EventParent(GameObject enemyObject, Action callback, DialogueManager dialogueManager, VisualEffectsManager effectsManager, GameObject choices, GameManager gameManager, Transform choicePosition)
    {
        this.enemyObject = enemyObject;
        this.dialogueManager = dialogueManager;
        this.effectsManager = effectsManager;
        endEventCallback = callback;
        this.choices = choices;
        this.gameManager = gameManager;
        this.choicePosition = choicePosition;
    }

    public void StartEvent()
    {
        LoadNextStep();
    }

    public void StartDialogue(List<DialogueSection> dialogueList, Action callback)
    {
        Dialogue dialogues = new(dialogueList, callback);

        dialogueManager.SetupDialogue(dialogues, enemyObject);
    }

    public async Task ExecuteWithDelay(Action action, float delayInSeconds)
    {
        await Task.Delay(TimeSpan.FromSeconds(delayInSeconds));
        action.Invoke();
    }

    public abstract void LoadNextStep();

    public abstract List<DialogueSection> LoadDialogue(int dialogue);
    public abstract void LoadCharacter(int character);
    public abstract void LoadFight(int character);

    public void EndEvent()
    {
        endEventCallback?.Invoke();
    }

    public void SetupCharacter(string characterName, bool playAnimation)
    {
        CharacterManager.LoadCharacter(characterName, enemyObject);
        if(playAnimation)
            AnimationManager.PlayAnimation(enemyObject, AnimationManager.SpriteAnimation.UnitIntro, LoadNextStep, effectsManager);
    }

    public void LoadChoiceManager(List<Choice> choices)
    {
        choiceManager = new(choices, choicePosition);
    }

    public struct DialogueConfig
    {
        public string DialogueText { get; set; }
        public float Speed { get; set; }

        public DialogueConfig(string dialogueText, float speed)
        {
            DialogueText = dialogueText;
            Speed = speed;
        }
    }
}