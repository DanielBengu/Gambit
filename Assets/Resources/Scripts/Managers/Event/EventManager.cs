using System.Collections.Generic;
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

    public EventManager(EventData eventData, GameObject character, VisualEffectsManager effectsManager, TextMeshProUGUI textBubble, GameUIManager gameUIManager, GameManager gameManager)
    {
        this.character = character;
        this.gameManager = gameManager;
        this.gameUIManager = gameUIManager;
        this.textBubble = textBubble;

        visualEffectsManager = effectsManager;

        CharacterManager.LoadCharacter(eventData.Description, character);

        dialogueManager = new(textBubble, effectsManager);
    }

    public void Update()
    {
        dialogueManager.TickDialogue();
    }

    public void CharacterTalk()
    {
        List<DialogueSection> dialogueList = new(){
            new("Hello! Welcome to the tutorial of GAMBIT!\n Choose your powerup:" , 0.05f, character),
            new("THIS IS A SUPER FAST TEXT OMG WHY IS IT SO FAAAAAST", 0.01f, character)
        };

        Dialogue dialogues = new(dialogueList, EndEvent);

        dialogueManager.SetupDialogue(dialogues, character);
    }

    void EndEvent()
    {
        CharacterManager.ResetCharacter(character, visualEffectsManager);

        textBubble.transform.parent.gameObject.SetActive(false);

        gameUIManager.TurnOffEventUI();
        gameManager.HandleFightVictory();
    }
}