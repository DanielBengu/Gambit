using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static CardsManager;

public class MenuCharacterCard : MonoBehaviour
{
    MenuManager manager;
    Classes classOfCard;
    bool isUnlocked;

    public void LoadManager(MenuManager manager, Classes classOfCard, bool isUnlocked)
    {
        this.manager = manager;
        this.classOfCard = classOfCard;
        this.isUnlocked = isUnlocked;
    }

    private void OnMouseDown()
    {
        if(isUnlocked)
            manager.StartGame(classOfCard);
    }
}
