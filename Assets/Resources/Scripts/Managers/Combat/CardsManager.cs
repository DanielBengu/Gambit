using System;
using System.Collections.Generic;
using UnityEngine;

public class CardsManager
{
    public Dictionary<Classes, Action> playerCardHandlers = new()
    {

    };
    public Dictionary<Classes, Action> enemyCardHandlers = new()
    {

    };

    public enum Classes
    {
        Basic = -1,
        Warrior,
        Rogue,
        Wizard,
        Berserk,
        Ranger,
        Archmage,
        Trickster
    }
}
