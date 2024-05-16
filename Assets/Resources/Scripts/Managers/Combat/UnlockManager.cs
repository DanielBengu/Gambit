using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static CardsManager;

public class UnlockManager
{
    readonly GameObject unlockParent;

    readonly GameObject characterCardUnlock;

    public UnlockManager(GameObject unlockParent)
    {
        this.unlockParent = unlockParent;
        characterCardUnlock = Resources.Load<GameObject>("Prefabs/CharacterCard");
    }

    public void UnlockCard(UnlockableType type, int cardId, Action callback)
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

        LoadCharacterCard(instance, classUnlocked);
        LoadCharacterScript(instance, callback);
    }

    void LoadCharacterCard(GameObject obj,Classes classUnlocked)
    {
        Image sprite = obj.transform.Find("Sprite").GetComponent<Image>();
        TextMeshProUGUI title = obj.transform.Find("Title").GetComponent<TextMeshProUGUI>();

        sprite.sprite = Resources.Load<Sprite>($"Sprites/Characters/{classUnlocked}/{classUnlocked}");
        title.text = classUnlocked.ToString();
    }

    void LoadCharacterScript(GameObject obj, Action callback)
    {
        obj.GetComponent<UnlockableCardScript>().LoadScript(callback);
    }

    public enum UnlockableType
    {
        Character,
        ActionCard
    }
}