using TMPro;
using UnityEngine;

public class EventManager
{
    private GameObject _character;
    readonly DialogueManager dialogueManager;
    readonly Transform characterParent;
    GameObject Character { get { return GetEnemyCharacter(); } set { _character = value; } }
    readonly GameManager gameManager;
    readonly GameUIManager gameUIManager;
    readonly VisualEffectsManager visualEffectsManager;
    readonly TextMeshProUGUI textBubble;
    readonly int eventId;

    public GameObject choicesObj;
    public Transform choicePosition;

    EventParent currentEvent;

    public EventManager(int eventData, Transform characterParent, GameObject character,  VisualEffectsManager effectsManager, TextMeshProUGUI textBubble, GameUIManager gameUIManager, GameManager gameManager, GameObject choicesObj, LanguageManager languageManager, Transform choicesPositionObj)
    {
        this.characterParent = characterParent;
        this.Character = character;
        this.gameManager = gameManager;
        this.gameUIManager = gameUIManager;
        this.textBubble = textBubble;
        this.choicesObj = choicesObj;


        choicePosition = choicesPositionObj;

        eventId = eventData;

        visualEffectsManager = effectsManager;
        dialogueManager = new(textBubble, languageManager);

        LoadEvent();
    }

    public GameObject GetEnemyCharacter()
    {
        if(characterParent.childCount == 0) 
            return null;


        if(_character == null)
        {
            _character = characterParent.GetChild(0).gameObject;
        }
            

        return _character;
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
        CharacterManager.ResetCharacter(Character, visualEffectsManager);

        textBubble.transform.parent.gameObject.SetActive(false);

        gameUIManager.TurnOffEventUI();
        gameManager.HandleEventVictory();

        currentEvent = null;
    }

    EventParent GetCurrentEvent(int id)
    {
        return id switch
        {
            0 => new Welcome(Character, characterParent, EndEvent, dialogueManager, visualEffectsManager, choicesObj, gameManager, choicePosition),
            1 => new FollowerOfXsant(Character, characterParent, EndEvent, dialogueManager, visualEffectsManager, choicesObj, gameManager, choicePosition),
            2 => new KaldorBoss(Character, characterParent, EndEvent, dialogueManager, visualEffectsManager, choicesObj, gameManager, choicePosition),
            _ => new EmptyEvent(Character, characterParent, EndEvent, dialogueManager, visualEffectsManager, choicesObj, gameManager, choicePosition)
        };
    }
}