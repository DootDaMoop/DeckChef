using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CardUI : MonoBehaviour
{
    public TextMeshProUGUI cardNameText;
    public TextMeshProUGUI cardDescriptionText;
    public Image cardImage;

    private CardData cardData;

    public void Initialize(CardData data) {
        cardData = data;
        cardNameText.text = cardData.cardName;
        cardDescriptionText.text = cardData.cardDescription;
        cardImage.sprite = cardData.cardImage;
    }

    public CardData GetCardData() {
        return cardData;
    }
}
