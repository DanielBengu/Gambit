using System;
using System.Collections.Generic;
using UnityEngine;

public abstract class EventParent
{
    public abstract int EventId { get; }
    public abstract int NumberOfEvents { get; }

    public int CurrentEventCount { get; set; } = -1;

    readonly DialogueManager dialogueManager;
    readonly VisualEffectsManager effectsManager;

    public Action endEventCallback;
    internal readonly GameObject enemyObject;

    public EventParent(GameObject enemyObject, Action callback, DialogueManager dialogueManager, VisualEffectsManager effectsManager)
    {
        this.enemyObject = enemyObject;
        this.dialogueManager = dialogueManager;
        this.effectsManager = effectsManager;
        endEventCallback = callback;
    }

    public void StartEvent()
    {
        LoadNextStep();
    }

    public void StartDialogue(List<DialogueSection> dialogueList)
    {
        Dialogue dialogues = new(dialogueList, LoadNextStep);

        dialogueManager.SetupDialogue(dialogues, enemyObject);
    }

    public abstract void LoadNextStep();

    public abstract List<DialogueSection> LoadDialogue(int dialogue);
    public abstract void LoadCharacter(int character);

    public void EndEvent()
    {
        endEventCallback?.Invoke();
    }

    public void SetupCharacter(string characterName)
    {
        CharacterManager.LoadCharacter(characterName, enemyObject);
        AnimationManager.PlayAnimation(enemyObject, AnimationManager.SpriteAnimation.UnitIntro, LoadNextStep, effectsManager);
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