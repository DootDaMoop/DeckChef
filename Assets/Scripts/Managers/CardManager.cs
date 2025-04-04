using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardManager : MonoBehaviour
{
    public static CardManager instance;

    [Header("Deck Setup")]
    public List<CardData> startingDeck;
    private List<CardData> drawPile = new List<CardData>();
    private List<CardData> discardPile = new List<CardData>();
    private List<CardData> hand = new List<CardData>();

    [Header("Hand Settings")]
    public int maxHandSize = 5;

    [Header("UI Settings")]
    public GameObject handUI;
    public GameObject cardPrefab;

    private void Awake() {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start() {
        InitializeDeck();
        DrawStartingHand();
    }

    public void InitializeDeck() {
        drawPile = new List<CardData>(startingDeck);
        Shuffle(drawPile);
    }

    public void DrawStartingHand() {
        for (int i = 0; i < maxHandSize; i++)
        {
            DrawCard();
        }
    }

    public void DrawCard() {
        if (drawPile.Count == 0) {
            drawPile = new List<CardData>(discardPile);
            discardPile.Clear();
            Shuffle(drawPile);
        }

        if (drawPile.Count == 0) {
            Debug.Log("No cards left to draw!");
            return;
        }

        CardData drawnCard = drawPile[0];
        drawPile.RemoveAt(0);
        hand.Add(drawnCard);
        Debug.Log("Drew card: " + drawnCard.cardName);
        
        GameObject cardUIObject = Instantiate(cardPrefab, handUI.transform);
        cardUIObject.GetComponent<CardUI>().Initialize(drawnCard);
    }

    private void Shuffle(List<CardData> list) {
        for (int i = 0; i < list.Count; i++) {
            int randomIndex = Random.Range(i, list.Count);
            (list[i], list[randomIndex]) = (list[randomIndex], list[i]);
        }
    }

    public void PlayCard(CardData card) {
        if (hand.Contains(card)) {
            hand.Remove(card);
            discardPile.Add(card);
            Debug.Log("Played card: " + card.cardName);
            // TODO: UI update for hand and discard pile
        }
    }

    public void DiscardCard(CardData card) {
        if (hand.Contains(card)) {
            hand.Remove(card);
            discardPile.Add(card);
            Debug.Log("Discarded card: " + card.cardName);
        }
    }

    public List<CardData> GetHand() {
        return hand;
    }
    
}
