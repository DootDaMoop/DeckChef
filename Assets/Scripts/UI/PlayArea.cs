using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class PlayArea : MonoBehaviour, IDropHandler
{
    public void OnDrop(PointerEventData eventData)
    {
        GameObject droppedObject = eventData.pointerDrag;
        if (droppedObject != null)
        {
            CardUI cardUI = droppedObject.GetComponent<CardUI>();
            if (cardUI != null)
            {
                // Get the card data
                CardData cardData = cardUI.GetCardData();
                if (cardData != null)
                {
                    // Play the card via the transform manager
                    if (CardTransformationManager.instance != null)
                    {
                        CardTransformationManager.instance.PlayCard(cardData);
                        
                        // Remove the card from the player's hand
                        CardManager.instance.DiscardCard(cardData);
                    }
                }
            }
        }
    }
}