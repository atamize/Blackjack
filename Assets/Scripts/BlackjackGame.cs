using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlackjackGame : MonoBehaviour
{
    public List<Player> players;
    public Deck deck;

    int currentPlayer = 0;

    void Start()
    {
        Deal();
    }

    void Deal()
    {
        foreach (Player player in players)
        {
            bool faceUp = true;
            player.DealCard(deck.DrawCard(faceUp));
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return))
        {
            Deal();
        }
    }
}
