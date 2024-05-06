using System.Collections.Generic;
using Unity.VisualScripting.Antlr3.Runtime.Tree;
using static CardsManager;

[System.Serializable]
public class PlayerData
{
    public UnitData UnitData;
    public CurrentRun CurrentRun;
}

[System.Serializable]
public class CurrentRun
{
    public bool IsOngoing;
    public int MapId;
    public Classes ClassId;
    public int CurrentFloor;
    public List<GameCard> CardList;
}

[System.Serializable]
public class UnitData
{
    public string Name;
    public int Armor;
    public int MaxHP;
    public int CurrentHP;
    public int MaxScore;
    public int NumberOfAttacks;
}