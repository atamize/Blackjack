using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Suit
{
    Clubs, Spades, Hearts, Diamonds
}

[System.Serializable]
public class CardData
{
    public Sprite sprite;
    public int value;
    public Suit suit;
}

