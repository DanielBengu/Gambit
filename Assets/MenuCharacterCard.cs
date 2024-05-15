using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static CardsManager;

public class MenuCharacterCard : MonoBehaviour
{
    MenuManager manager;
    Classes classOfCard;

    public void LoadManager(MenuManager manager, Classes classOfCard)
    {
        this.manager = manager;
        this.classOfCard = classOfCard;
    }

    private void OnMouseDown()
    {
        manager.StartGame(classOfCard);
    }
}
