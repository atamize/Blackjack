using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class BlackjackGame : MonoBehaviour
{
    public static readonly int BLACKJACK_GOAL = 21;
    public static readonly int ACE_LOW_VALUE = 1;
    public static readonly int ACE_HIGH_VALUE = 11;
    public static readonly int DEALER_LIMIT = 17;

    public Player dealer;
    public List<Player> players;
    public Deck deck;
    public GameObject actionPanel;
    public GameObject endRoundPanel;
    public TextMeshProUGUI playerPrompt;

    int currentPlayer = 0;

    void Start()
    {
        Deal();
    }

    void Deal()
    {
        foreach (Player player in players)
        {
            for (int i = 0; i < 2; ++i)
            {
                bool faceUp = true;
                player.DealCard(deck.DrawCard(faceUp));
            }
        }

        dealer.DealCard(deck.DrawCard(true));
        dealer.DealCard(deck.DrawCard(false));

        currentPlayer = 0;
        NextPlayer();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return))
        {
            Deal();
        }
    }

    public void Hit()
    {
        players[currentPlayer].DealCard(deck.DrawCard(true));
    }

    public void Stay()
    {
        currentPlayer++;
        NextPlayer();
    }

    void NextPlayer()
    {
        if (currentPlayer < players.Count)
        {
            playerPrompt.text = string.Format("{0}, choose an action", players[currentPlayer].nameText.text);
        }
        else
        {
            actionPanel.SetActive(false);
            endRoundPanel.SetActive(true);
            EvaluateRound();
        }
    }

    void EvaluateRound()
    {
        dealer.RevealCards();
        while (dealer.GetValue() < DEALER_LIMIT)
        {
            dealer.DealCard(deck.DrawCard(true));
        }
        dealer.UpdateValue();

        int dealerValue = dealer.GetValue();
        if (dealerValue > BLACKJACK_GOAL)
        {
            // Dealer busts; everyone wins!
            foreach (Player player in players)
            {
                player.valueText.text = "WIN";
            }
        }
        else
        {
            foreach (Player player in players)
            {
                int playerValue = player.GetValue();
                if (playerValue > BLACKJACK_GOAL || playerValue <= dealerValue)
                {
                    player.valueText.text = "LOSE";
                }
                else
                {
                    player.valueText.text = "WIN";
                }
            }
        }
    }

    public void EndRound()
    {
        List<Player> playersToClear = new List<Player>(players);
        playersToClear.Add(dealer);
        foreach (Player player in playersToClear)
        {
            var cards = player.GetCards();
            foreach (Card card in cards)
            {
                deck.Discard(card);
            }
            player.ClearCards();
        }

        Deal();
        actionPanel.SetActive(true);
        endRoundPanel.SetActive(false);
    }
}
