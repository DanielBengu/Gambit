using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static CardsManager;
using static Unity.Collections.AllocatorManager;

public class UnlockManager
{
    readonly GameObject unlockParent;

    readonly GameObject characterCardUnlock;

    readonly Queue<Tuple<UnlockableType, int>> unlocksQueue = new();
    Action queueCallback;

    public UnlockManager(GameObject unlockParent)
    {
        this.unlockParent = unlockParent;
        characterCardUnlock = Resources.Load<GameObject>("Prefabs/CharacterCard");
    }

    public void ManageUnlocksByMapCompletion(PlayerData playerData, Map mapCompleted, Action callback)
    {
        unlocksQueue.Clear();
        queueCallback = callback;

        LoadUnlocksIntoQueue(playerData, mapCompleted);

        StartQueueUnload();
    }

    void StartQueueUnload()
    {
        if(unlockParent.transform.childCount > 0)
        {
            UnityEngine.Object.Destroy(unlockParent.transform.GetChild(0).gameObject);
        }

        var unlock = unlocksQueue.Dequeue();

        Action callback = unlocksQueue.Count > 0 ? StartQueueUnload : queueCallback;

        UnlockCard(unlock.Item1, unlock.Item2, callback);
    }

    void UnlockCard(UnlockableType type, int cardId, Action callback)
    {
        unlockParent.SetActive(true);

        switch (type)
        {
            case UnlockableType.Character:
                UnlockCharacter((Classes)cardId, callback);
                break;
            case UnlockableType.ActionCard: 
                break;
        }
    }

    void UnlockCharacter(Classes classUnlocked, Action callback)
    {
        var instance = UnityEngine.Object.Instantiate(characterCardUnlock, unlockParent.transform);

        PlayerPrefsManager.PlayerPrefsEnum unlock = classUnlocked switch { 
            Classes.Warrior => PlayerPrefsManager.PlayerPrefsEnum.WarriorUnlocked,
            Classes.Rogue => PlayerPrefsManager.PlayerPrefsEnum.RogueUnlocked,
            Classes.Wizard => PlayerPrefsManager.PlayerPrefsEnum.WizardUnlocked,
            Classes.Berserk => PlayerPrefsManager.PlayerPrefsEnum.BerserkUnlocked,
            Classes.Trickster => PlayerPrefsManager.PlayerPrefsEnum.TricksterUnlocked,
            Classes.Archmage => PlayerPrefsManager.PlayerPrefsEnum.ArchmageUnlocked,
            Classes.Poisoner => PlayerPrefsManager.PlayerPrefsEnum.PoisonerUnlocked,
            _ => PlayerPrefsManager.PlayerPrefsEnum.WarriorUnlocked
        };

        LoadCharacterCard(instance, classUnlocked);
        LoadCharacterScript(instance, unlock, callback);
    }

    void LoadCharacterCard(GameObject obj,Classes classUnlocked)
    {
        Image sprite = obj.transform.Find("Sprite").GetComponent<Image>();
        TextMeshProUGUI title = obj.transform.Find("Title").GetComponent<TextMeshProUGUI>();

        sprite.sprite = Resources.Load<Sprite>($"Sprites/Characters/{classUnlocked}/{classUnlocked}");
        title.text = classUnlocked.ToString();
    }

    void LoadCharacterScript(GameObject obj, PlayerPrefsManager.PlayerPrefsEnum unlock, Action callback)
    {
        obj.GetComponent<UnlockableCardScript>().LoadScript(callback, unlock);
    }

    public void LoadUnlocksIntoQueue(PlayerData playerData, Map mapCompleted)
    {
        if(mapCompleted.Id == MenuManager.TUTORIAL_WORLD_ID)
        {
            unlocksQueue.Enqueue(new(UnlockableType.Character, (int)Classes.Rogue));
            unlocksQueue.Enqueue(new(UnlockableType.Character, (int)Classes.Wizard));
            return;
        }

        switch (playerData.CurrentRun.ClassId)
        {
            case Classes.Warrior:
                unlocksQueue.Enqueue(new(UnlockableType.Character, (int)Classes.Berserk));
                break;
            case Classes.Rogue:
                unlocksQueue.Enqueue(new(UnlockableType.Character, (int)Classes.Trickster));
                break;
            case Classes.Wizard:
                unlocksQueue.Enqueue(new(UnlockableType.Character, (int)Classes.Archmage));
                break;
        }
    }

    public enum UnlockableType
    {
        Character,
        ActionCard
    }
}