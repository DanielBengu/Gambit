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
    public List<Card> TestCardList;
}

[Serializable]
public class GameCard
{
    public int id;
    public CardType cardType;
    public int value;
    public Classes classId;
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
    King = 13
}