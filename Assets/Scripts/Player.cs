using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Player : MonoBehaviour
{
    public Transform[] cardSlots;
    public TextMeshPro nameText;
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
        int aceCount = 0;
        for (int i = 0; i < cards.Count; ++i)
        {
            if (cards[i].FaceUp)
            {
                int val = cards[i].GetValue();

                // Handle Aces
                if (val == BlackjackGame.ACE_LOW_VALUE)
                {
                    aceCount++;
                }
                else
                {
                    sum += val;
                }
            }
        }

        for (int i = 0; i < aceCount; ++i)
        {
            if (sum + BlackjackGame.ACE_HIGH_VALUE <= BlackjackGame.BLACKJACK_GOAL)
            {
                sum += BlackjackGame.ACE_HIGH_VALUE;
            }
            else
            {
                sum += BlackjackGame.ACE_LOW_VALUE;
            }
        }
        return sum;
    }

    public void UpdateValue()
    {
        int value = GetValue();
        valueText.text = string.Format("Value: {0}", value);
    }

    public void RevealCards()
    {
        foreach (Card card in cards)
        {
            card.FaceUp = true;
        }
    }

    public List<Card> GetCards()
    {
        return cards;
    }

    public void ClearCards()
    {
        cards.Clear();
        UpdateValue();
    }
}
