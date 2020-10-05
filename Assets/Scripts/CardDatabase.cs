using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CardDatabase", menuName = "ScriptableObjects/CardDatabase", order = 1)]
public class CardDatabase : ScriptableObject
{
    public CardData[] cards;

    public void AssignIds()
    {
        for (int i = 0; i < cards.Length; ++i)
        {
            cards[i].id = i;
        }
    }
}
