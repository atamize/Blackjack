using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Deck : MonoBehaviour
{
    public CardDatabase cardDatabase;
    public GameObject cardPrefab;
    public int numDecks = 1;

    List<CardData> currentDeck;
    int usedCount = 0;
    LinkedList<GameObject> cardPool;

    private void Awake()
    {
        currentDeck = new List<CardData>();
        for (int i = 0; i < numDecks; ++i)
        {
            currentDeck.AddRange(cardDatabase.cards);
        }
        cardPool = new LinkedList<GameObject>();
        Shuffle(currentDeck);
    }

    private void Shuffle(List<CardData> cards)
    {
        for (int i = 0; i < cards.Count; ++i)
        {
            int randomIndex = Random.Range(0, cards.Count - 1);
            CardData temp = cards[i];
            cards[i] = cards[randomIndex];
            cards[randomIndex] = temp;
        }
    }

    public Card DrawCard(bool faceUp)
    {
        if (usedCount == currentDeck.Count)
        {
            Shuffle(currentDeck);
            usedCount = 0;
        }

        CardData data = currentDeck[usedCount++];
        GameObject cardObject = null;

        if (cardPool.Count > 0)
        {
            cardObject = cardPool.First.Value;
            cardObject.SetActive(true);
            cardPool.RemoveFirst();
        }
        else
        {
            cardObject = Instantiate(cardPrefab);
        }

        Card card = cardObject.GetComponent<Card>();
        card.cardData = data;
        card.spriteRenderer.sprite = data.sprite;
        card.FaceUp = faceUp;
        return card;
    }

    public void Discard(Card card)
    {
        cardPool.AddLast(card.gameObject);
        card.transform.parent = transform;
        card.gameObject.SetActive(false);
    }

    private void Update()
    {

    }
}
