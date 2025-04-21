using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class CookingStation : MonoBehaviour, IDropHandler
{
    [Header("Station Settings")]
    public TechniqueType techniqueType;
    public string stationName;
    [TextArea] public string stationDescription;
    public int processingTime = 1;

    [Header("UI")]
    public Image stationImage;
    public TextMeshProUGUI stationNameText;
    public TextMeshProUGUI stationTimerText;
    public GameObject highlightEffectUI;
    public GameObject busyOverlayUI;

    [Header("Card Display")]
    public Transform cardHolder;
    public GameObject cardPrefab;

    private bool isProcessing = false;
    private int turnsRemaining = 0;
    private CardData processingCard;
    private GameObject displayedCard;

    private void Start() {
        if (stationNameText != null) {
            stationNameText.text = stationName;
        }

        UpdateVisuals();
    }

    private void UpdateVisuals() {
        if (stationTimerText != null) {
            stationTimerText.text = isProcessing ? turnsRemaining.ToString() : "";
            stationTimerText.gameObject.SetActive(isProcessing);
        }

        if (busyOverlayUI != null) {
            busyOverlayUI.SetActive(isProcessing);
        }

        if (highlightEffectUI != null) {
            highlightEffectUI.SetActive(!isProcessing);
        }
    }

    public void OnDrop(PointerEventData eventData) {
        if (isProcessing) return;

        GameObject droppedCard = eventData.pointerDrag;
        if (droppedCard != null) {
            CardUI cardUI = droppedCard.GetComponent<CardUI>();
            if (cardUI != null) {
                CardData cardData = cardUI.GetCardData();
                if (IsValidCard(cardData)) {
                    StartProcessing(cardData);
                    CardManager.instance.DiscardCard(cardData);
                }
            }
        }
    }

    private bool IsValidCard(CardData card) {
        return card != null && card.cardType == CardType.Ingredient;
    }

    private void StartProcessing(CardData card) {
        processingCard = card;
        turnsRemaining = processingTime;
        isProcessing = true;

        CreateCardDisplay(card);
        UpdateVisuals();
        Debug.Log($"Started processing {card.cardName} on {stationName}.");
    }

    private void CreateCardDisplay(CardData card) {
        if (displayedCard != null) {
            Destroy(displayedCard);
        }

        displayedCard = Instantiate(cardPrefab, cardHolder);
        CardUI cardUI = displayedCard.GetComponent<CardUI>();

        if (cardUI != null) {
            cardUI.Initialize(card);

            CardDragHandler dragHandler = displayedCard.GetComponent<CardDragHandler>();
            if (dragHandler != null) {
                dragHandler.enabled = false;
            }
        }
    }

    public void ProcessEndTurn() {
        if (!isProcessing) return;
        turnsRemaining--;

        if (turnsRemaining <= 0) {
            CompleteProcessing();
        } else {
            UpdateVisuals();
        }
    }

    private void CompleteProcessing() {
        CardData transformedCard = TransformCard(processingCard);

        AddCardToHand(transformedCard);

        isProcessing = false;
        processingCard = null;
        if (displayedCard != null) {
            Destroy(displayedCard);
            displayedCard = null;
        }

        UpdateVisuals();
    }

    private CardData TransformCard(CardData originalCard) {
        if (originalCard is IngredientCard) {
            IngredientCard ingredient = originalCard as IngredientCard;
            IngredientCard transformedCard = ScriptableObject.CreateInstance<IngredientCard>();

            transformedCard.cardName = ingredient.cardName + " (Processed)";
            transformedCard.cardType = CardType.Ingredient;
            transformedCard.cardDescription = ingredient.cardDescription + " (Processed)";
            transformedCard.cardImage = ingredient.cardImage;
            transformedCard.cost = ingredient.cost + 1;
            
            transformedCard.elementType = GetTransformedElement(ingredient.elementType);
            transformedCard.isConsumable = ingredient.isConsumable;

            Debug.Log("Transformed card: " + transformedCard.cardName);
            return transformedCard;
        }

        return originalCard;
    }

    private ElementType GetTransformedElement(ElementType originalElement) {
        // Example transformation logic
        switch (originalElement) {
            case ElementType.Fire:
                return ElementType.Water;
            case ElementType.Water:
                return ElementType.Earth;
            case ElementType.Earth:
                return ElementType.Air;
            case ElementType.Air:
                return ElementType.Fire;
            default:
                return originalElement;
        }
    }

    private void AddCardToHand(CardData card) {
        CardManager.instance.AddCardToHand(card);
    }
}
