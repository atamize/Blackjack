using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Player : MonoBehaviour
{
    public Transform[] cardSlots;
    public TextMeshPro valueText;

    List<Card> cards = new List<Card>();

    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void DealCard(Card card)
    {
        card.transform.parent = cardSlots[cards.Count];
        card.transform.localPosition = Vector3.zero;
        card.spriteRenderer.sortingOrder = cards.Count;
        cards.Add(card);
        UpdateValue();
    }

    public int GetValue()
    {
        int sum = 0;
        for (int i = 0; i < cards.Count; ++i)
        {
            if (cards[i].FaceUp)
            {
                sum += cards[i].GetValue();
            }
        }
        return sum;
    }

    void UpdateValue()
    {
        int value = GetValue();
        valueText.text = string.Format("Value: {0}", value);
    }
}
