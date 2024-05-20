using System;
using System.Collections.Generic;
using UnityEngine;
using static CardsManager;

[Serializable]
public class Card
{
    public int Id;
    public string Name;
    public int Quantities;
}

[Serializable]
public class CardListData
{
    public List<Card> StartingCardList;
    public List<Card> WarriorActionCardList;
    public List<Card> WizardActionCardList;
    public List<Card> RangerActionCardList;
    public List<Card> RogueActionCardList;
    public List<Card> MonkActionCardList;
    public List<Card> CrystalActionCardList;
    public List<Card> TricksterActionCardList;
}

[Serializable]
public class GameCard
{
    public int id;
    public CardType cardType;
    public int value;
    public Classes classId;
    public bool destroyOnPlay;
}

[Serializable]
public class ActionCard
{
    public int Id;
    public int NameIdValue;
    public int DescriptionIdValue;
    public Classes ClassId;
    public ActionType ActionId;
}

public enum CardType
{
    Default = -1,
    Ace,
    One,
    Two,
    Three,
    Four,
    Five,
    Six,
    Jack = 11,
    Queen = 12,
    King = 13,
    Potion
}

public enum ActionType
{
    Equip,
    Attack,
    Skill,
    Artifact,
    Modifier
}