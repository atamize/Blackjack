using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Player : MonoBehaviour
{
    public Transform[] cardSlots;
    public TextMeshPro nameText;
    public TextMeshPro valueText;
    public TextMeshPro moneyText;
    public TextMeshPro betText;

    List<Card> cards = new List<Card>();
    public int Money { get; set; }
    public int Bet { get; set; }

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

    public void Win()
    {
        if (cards.Count == 2 && GetValue() == BlackjackGame.BLACKJACK_GOAL)
        {
            valueText.text = "BLACKJACK";
            Money += (int)(Bet * BlackjackGame.BlackjackPayout);
        }
        else
        {
            valueText.text = "WIN";
            Money += Bet;
        }
        
        UpdateMoney();
    }

    public void Lose()
    {
        valueText.text = "LOSE";
        Money -= Bet;
        UpdateMoney();
    }

    public void Tie()
    {
        valueText.text = "TIE";
    }

    public void UpdateBet()
    {
        betText.text = string.Format("Bet: ${0}", Bet);
    }

    public void UpdateMoney()
    {
        moneyText.text = string.Format("Money: ${0}", Money);
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
        Bet = 0;
        UpdateBet();
        UpdateValue();
    }
}
