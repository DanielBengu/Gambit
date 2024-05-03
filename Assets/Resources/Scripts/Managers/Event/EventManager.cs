using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using static EventParent;

public class EventManager
{
    readonly DialogueManager dialogueManager;
    readonly GameObject character;
    readonly GameManager gameManager;
    readonly GameUIManager gameUIManager;
    readonly VisualEffectsManager visualEffectsManager;
    readonly TextMeshProUGUI textBubble;
    readonly int eventId;

    public GameObject pageObj;

    public EventManager(int eventData, GameObject character, VisualEffectsManager effectsManager, TextMeshProUGUI textBubble, GameUIManager gameUIManager, GameManager gameManager, GameObject pageObj)
    {
        this.character = character;
        this.gameManager = gameManager;
        this.gameUIManager = gameUIManager;
        this.textBubble = textBubble;
        this.pageObj = pageObj;

        eventId = eventData;

        visualEffectsManager = effectsManager;
        dialogueManager = new(textBubble, effectsManager);

        LoadEvent();
    }

    public void LoadEvent()
    {
        EventParent currentEvent = GetCurrentEvent(eventId);
        currentEvent.StartEvent();
    }

    public void Update()
    {
        dialogueManager.TickDialogue();
    }

    void EndEvent()
    {
        CharacterManager.ResetCharacter(character, visualEffectsManager);

        textBubble.transform.parent.gameObject.SetActive(false);

        gameUIManager.TurnOffEventUI();
        gameManager.HandleFightVictory();
    }

    EventParent GetCurrentEvent(int id)
    {
        return id switch
        {
            0 => new Welcome(character, EndEvent, dialogueManager, visualEffectsManager),
            _ => new EmptyEvent(character, EndEvent, dialogueManager, visualEffectsManager)
        };
    }
}