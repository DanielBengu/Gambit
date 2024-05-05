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

    public EventManager(int eventData, GameObject character, VisualEffectsManager effectsManager, TextMeshProUGUI textBubble, GameUIManager gameUIManager, GameManager gameManager, GameObject choicesObj, LanguageManager languageManager)
    {
        this.character = character;
        this.gameManager = gameManager;
        this.gameUIManager = gameUIManager;
        this.textBubble = textBubble;
        this.choicesObj = choicesObj;

        eventId = eventData;

        visualEffectsManager = effectsManager;
        dialogueManager = new(textBubble, effectsManager, languageManager);

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
        gameManager.HandleEventVictory();

        currentEvent = null;
    }

    EventParent GetCurrentEvent(int id)
    {
        return id switch
        {
            0 => new Welcome(character, EndEvent, dialogueManager, visualEffectsManager, choicesObj, gameManager),
            1 => new FollowerOfXsant(character, EndEvent, dialogueManager, visualEffectsManager, choicesObj, gameManager),
            _ => new EmptyEvent(character, EndEvent, dialogueManager, visualEffectsManager, choicesObj, gameManager)
        };
    }
}