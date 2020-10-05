using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization;

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
    string savePath;

    void Start()
    {
        savePath = Application.persistentDataPath + "/save.dat";
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

        Save();

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
        int value = players[currentPlayer].GetValue();
        if (value > BLACKJACK_GOAL)
        {
            players[currentPlayer].Lose();
            Stay();
        }
        else if (value == BLACKJACK_GOAL)
        {
            Stay();
        }
        else
        {
            Save();
        }
    }

    public void Stay()
    {
        currentPlayer++;
        if (currentPlayer < players.Count) // Avoid doing a double save when round ends
        {
            Save();
        }
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
            Save();
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

    [System.Serializable]
    class PlayerData
    {
        public int money;
        public int bet;
        public List<int> cards;
    }

    [System.Serializable]
    class GameData
    {
        public List<PlayerData> playerData;
        public List<int> dealerCards;
        public List<int> deckCards;
        public int deckUsedCount;
        public int currentPlayer;
    }

    void Save()
    {
        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Create(savePath);
        
        GameData gameData = new GameData();
        gameData.playerData = new List<PlayerData>();
        foreach (Player player in players)
        {
            PlayerData playerData = new PlayerData();
            playerData.money = player.Money;
            playerData.bet = player.Bet;
            playerData.cards = new List<int>();

            foreach (Card card in player.GetCards())
            {
                playerData.cards.Add(card.cardData.id);
            }
            gameData.playerData.Add(playerData);
        }
        gameData.currentPlayer = currentPlayer;
        gameData.dealerCards = new List<int>();
        foreach (Card card in dealer.GetCards())
        {
            gameData.dealerCards.Add(card.cardData.id);
        }

        gameData.deckCards = new List<int>();
        gameData.deckUsedCount = deck.GetSaveData(gameData.deckCards);

        bf.Serialize(file, gameData);
        file.Close();
    }

    public void Load()
    {
        if (File.Exists(savePath))
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(savePath, FileMode.Open);
            GameData gameData = (GameData)bf.Deserialize(file);
            file.Close();

            currentPlayer = gameData.currentPlayer;
            bool roundInProgress = currentPlayer < players.Count;

            for (int i = 0; i < gameData.dealerCards.Count; ++i)
            {
                bool faceUp = !roundInProgress || i == 0;
                dealer.DealCard(deck.SpawnCard(gameData.dealerCards[i], faceUp));
            }

            for (int i = 0; i < gameData.playerData.Count; ++i)
            {
                PlayerData playerData = gameData.playerData[i];
                players[i].Bet = playerData.bet;
                players[i].Money = playerData.money;
                players[i].UpdateBet();
                players[i].UpdateMoney();

                foreach (int id in playerData.cards)
                {
                    players[i].DealCard(deck.SpawnCard(id, true));
                }
                players[i].UpdateValue();
            }

            deck.Initialize(gameData.deckUsedCount, gameData.deckCards);

            if (roundInProgress)
            {
                actionPanel.SetActive(true);
                betPanel.SetActive(false);
                endRoundPanel.SetActive(false);
                NextPlayer();
            }
            else
            {
                actionPanel.SetActive(false);
                betPanel.SetActive(false);
                endRoundPanel.SetActive(true);
            }
        }
    }
}
