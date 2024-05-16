using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static CardsManager;

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

        PlayerPrefsManager.PlayerPrefsEnum unlock = GetPrefFromClass(classUnlocked);

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

    public PlayerPrefsManager.PlayerPrefsEnum GetPrefFromClass(Classes cl)
    {
        return cl switch
        {
            Classes.Warrior => PlayerPrefsManager.PlayerPrefsEnum.WarriorUnlocked,
            Classes.Rogue => PlayerPrefsManager.PlayerPrefsEnum.RogueUnlocked,
            Classes.Wizard => PlayerPrefsManager.PlayerPrefsEnum.WizardUnlocked,
            Classes.Berserk => PlayerPrefsManager.PlayerPrefsEnum.BerserkUnlocked,
            Classes.Trickster => PlayerPrefsManager.PlayerPrefsEnum.TricksterUnlocked,
            Classes.Archmage => PlayerPrefsManager.PlayerPrefsEnum.ArchmageUnlocked,
            Classes.Ranger => PlayerPrefsManager.PlayerPrefsEnum.RangerUnlocked,
            _ => PlayerPrefsManager.PlayerPrefsEnum.WarriorUnlocked
        };
    }

    public void LoadUnlocksIntoQueue(PlayerData playerData, Map mapCompleted)
    {
        if(mapCompleted.Id == MenuManager.TUTORIAL_WORLD_ID)
        {
            EnqueueClassIfNotUnlocked(Classes.Rogue);
            EnqueueClassIfNotUnlocked(Classes.Wizard);
            return;
        }

        switch (playerData.CurrentRun.ClassId)
        {
            case Classes.Warrior:
                EnqueueClassIfNotUnlocked(Classes.Berserk);
                break;
            case Classes.Rogue:
                EnqueueClassIfNotUnlocked(Classes.Ranger);
                break;
            case Classes.Wizard:
                EnqueueClassIfNotUnlocked(Classes.Archmage);
                break;
        }
    }

    public void EnqueueClassIfNotUnlocked(Classes classUnit)
    {
        PlayerPrefsManager.PlayerPrefsEnum unlock = GetPrefFromClass(classUnit);

        if(PlayerPrefsManager.GetPref<int>(unlock) == 0)
            unlocksQueue.Enqueue(new(UnlockableType.Character, (int)classUnit));
    }

    public enum UnlockableType
    {
        Character,
        ActionCard
    }
}