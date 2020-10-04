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
    public GameObject betPanel;
    public GameObject actionPanel;
    public GameObject endRoundPanel;
    public TextMeshProUGUI playerPrompt;
    public BetUI betUI;

    int currentPlayer = 0;

    void Start()
    {
        NextBet();
    }

    void NextBet()
    {
        betUI.prompt.text = string.Format("{0}, place your bet", players[currentPlayer].nameText.text);
    }

    public void PlaceBet()
    {
        players[currentPlayer].Bet = betUI.BetAmount;
        players[currentPlayer].UpdateBet();

        currentPlayer++;
        if (currentPlayer == players.Count)
        {
            Deal();
            betPanel.SetActive(false);
            actionPanel.SetActive(true);
        }
        else
        {
            NextBet();
        }
    }

    void Deal()
    {
        currentPlayer = 0;
        foreach (Player player in players)
        {
            for (int i = 0; i < 2; ++i)
            {
                bool faceUp = true;
                player.DealCard(deck.DrawCard(faceUp));
            }

            int value = player.GetValue();
            if (value > BLACKJACK_GOAL)
            {
                player.Lose();
                currentPlayer++;
            }
            else if (value == BLACKJACK_GOAL)
            {
                currentPlayer++;
            }
        }

        dealer.DealCard(deck.DrawCard(true));
        dealer.DealCard(deck.DrawCard(false));
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
        if (players[currentPlayer].GetValue() > BLACKJACK_GOAL)
        {
            players[currentPlayer].Lose();
            Stay();
        }
    }

    public void Stay()
    {
        currentPlayer++;
        NextPlayer();
    }

    public void DoubleDown()
    {
        int player = currentPlayer;
        players[currentPlayer].Bet *= 2;
        players[currentPlayer].UpdateBet();
        Hit();

        // Busting automatically advances to the next player, so only stay if still alive
        if (player == currentPlayer)
        {
            Stay();
        }
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
        bool dealerBusts = dealerValue > BLACKJACK_GOAL;
        
        foreach (Player player in players)
        {
            int playerValue = player.GetValue();
            if (playerValue <= BLACKJACK_GOAL)
            {
                if (dealerBusts || playerValue > dealerValue)
                {
                    player.Win();
                }
                else if (playerValue < dealerValue)
                {
                    player.Lose();
                }
                else if (playerValue == BLACKJACK_GOAL && player.GetCards().Count == 2 && dealer.GetCards().Count > 2)
                {
                    player.Win();
                }
                else
                {
                    player.Tie();
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

        currentPlayer = 0;
        NextBet();
        betPanel.SetActive(true);
        endRoundPanel.SetActive(false);
    }
}
