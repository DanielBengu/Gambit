using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuManager : MonoBehaviour
{
    public MenuOptions menuOptions;

    void Start()
    {
        if (true)
        {
            menuOptions.CoverCard(MenuOptions.CardPosition.Left);
            menuOptions.CoverCard(MenuOptions.CardPosition.Right);
        }

        menuOptions.StartCardAnimation();
    }
}
