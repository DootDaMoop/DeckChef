using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CookingProgress : MonoBehaviour
{
    private GameObject cookingProgressUIOverlay;
    private TextMeshProUGUI cookingProgressText;

    private TechniqueCard techniqueCard;
    private IngredientCard ingredientCard;
    private int remainingTurns;
    private CardUI cardUI;

    public void Initialize(TechniqueCard techniqueCard, IngredientCard ingredientCard) {
        this.techniqueCard = techniqueCard;
        this.ingredientCard = ingredientCard;
        cardUI = GetComponent<CardUI>();
        remainingTurns = techniqueCard.cookTime;

        cookingProgressUIOverlay = cardUI.GetCookingOverlayUI();
        cookingProgressText = cardUI.GetCookingProgressText();

        cookingProgressUIOverlay.SetActive(true);
        UpdateDisplay();
        TurnManager.instance.onPlayerTurnStart.AddListener(OnTurnStart);
    }

    private void OnTurnStart() {
        remainingTurns--;
        UpdateDisplay();

        if (remainingTurns <= 0) {
            CompleteCooking();
        }
    }

    private void UpdateDisplay() {
        if (cookingProgressText != null) {
            cookingProgressText.text = $"{remainingTurns}";
        }
    }

    private void CompleteCooking() {
        TurnManager.instance.onPlayerTurnStart.RemoveListener(OnTurnStart);

        CardData resultCard = CardCombinationManager.instance.GetCombinationResults(techniqueCard, ingredientCard);
        CardManager.instance.RemoveCardFromHand(ingredientCard);
        CardManager.instance.AddCardToHand(resultCard);

        if (techniqueCard.reusable) {
            CardManager.instance.ReturnTechnniqueCard(techniqueCard);
        }
        Destroy(gameObject);
    }

    private void OnDestroy() {
        if (TurnManager.instance != null) {
            TurnManager.instance.onPlayerTurnStart.RemoveListener(OnTurnStart);
        }
    }

}
