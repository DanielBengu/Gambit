using TMPro;
using UnityEngine;

public class EventManager
{
    readonly DialogueManager dialogueManager;
    readonly GameObject character;
    readonly GameManager gameManager;
    readonly GameUIManager gameUIManager;
    readonly VisualEffectsManager visualEffectsManager;
    readonly TextMeshProUGUI textBubble;
    readonly int eventId;

    public GameObject choicesObj;

    EventParent currentEvent;

    public EventManager(int eventData, GameObject character, VisualEffectsManager effectsManager, TextMeshProUGUI textBubble, GameUIManager gameUIManager, GameManager gameManager, GameObject choicesObj)
    {
        this.character = character;
        this.gameManager = gameManager;
        this.gameUIManager = gameUIManager;
        this.textBubble = textBubble;
        this.choicesObj = choicesObj;

        eventId = eventData;

        visualEffectsManager = effectsManager;
        dialogueManager = new(textBubble, effectsManager);

        LoadEvent();
    }

    public void LoadEvent()
    {
        currentEvent = GetCurrentEvent(eventId);
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

        currentEvent = null;
    }

    EventParent GetCurrentEvent(int id)
    {
        return id switch
        {
            0 => new Welcome(character, EndEvent, dialogueManager, visualEffectsManager, choicesObj),
            _ => new EmptyEvent(character, EndEvent, dialogueManager, visualEffectsManager, choicesObj)
        };
    }
}