using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CardUI : MonoBehaviour, IDropHandler
{
    public TextMeshProUGUI cardNameText;
    public TextMeshProUGUI cardDescriptionText;
    public Image cardImage;
    [SerializeField] private GameObject cookingProgressUIOverlay;
    [SerializeField] private TextMeshProUGUI cookingProgressText;

    private CardData cardData;

    public void Initialize(CardData data) {
        cardData = data;
        cardNameText.text = cardData.cardName;
        cardDescriptionText.text = cardData.cardDescription;
        cardImage.sprite = cardData.cardImage;
    }

    public void OnDrop(PointerEventData eventData) {
        GameObject droppedObject = eventData.pointerDrag;
        if (droppedObject != null) {
            CardDragHandler dragHandler = droppedObject.GetComponent<CardDragHandler>();
            if (dragHandler != null) {
                dragHandler.CombineWithCard(this);
            }
        }
    }

    public CardData GetCardData() {
        return cardData;
    }

    public GameObject GetCookingOverlayUI() {
        return cookingProgressUIOverlay;
    }

    public TextMeshProUGUI GetCookingProgressText() {
        return cookingProgressText;
    }
}
