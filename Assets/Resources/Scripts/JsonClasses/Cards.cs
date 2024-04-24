using System;
using System.Collections.Generic;
using UnityEngine;

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
}

[Serializable]
public class GameCard
{
    public int id;
    public CardType cardType;
    public int value;
    public int classId;
}

public enum CardType
{
    Default = -1,
    Ace,
    Number,
    Figure,
    Special
}