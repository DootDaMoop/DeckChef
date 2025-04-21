using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class CardTransformationManager : MonoBehaviour
{
    public static CardTransformationManager instance;

    [Header("Cooking Stations")]
    [SerializeField] private List<CookingStation> cookingStations;

    [Header("Play Area")]
    [SerializeField] private Transform playAreaTransform;

    [Header("Events")]
    public UnityEvent<CardData> onCardPlayed;

    private void Awake() {
        if (instance == null) {
            instance = this;
        } else {
            Destroy(gameObject);
        }
    }

    public void ProcessTurnEnd() {
        foreach (CookingStation station in cookingStations) {
            station.ProcessEndTurn();
        }
    }

    public void PlayCard(CardData card) {
        ApplyCardEffects(card);
        onCardPlayed?.Invoke(card);
        Debug.Log($"Played card: {card.name}");
    }

    private void ApplyCardEffects(CardData card) {
        if (card is IngredientCard) {
            IngredientCard ingredientCard = card as IngredientCard;
            ApplyIngredientEffects(ingredientCard);
        } else if (card is SpiceCard) {
            SpiceCard spiceCard = card as SpiceCard;
            ApplySpiceEffects(spiceCard);
        }

        TurnManager.instance.EndPlayerTurn();
    }

    private void ApplyIngredientEffects(IngredientCard ingredientCard) {
        // TODO: Implement logic to apply ingredient effects
        Debug.Log($"Applying effects of ingredient card: {ingredientCard.name}");
    }

    private void ApplySpiceEffects(SpiceCard spiceCard) {
        // TODO: Implement logic to apply spice effects
        Debug.Log($"Applying effects of spice card: {spiceCard.name}");
    }

    public void UseBasicTechnique(TechniqueType techniqueType) {
        EnemyController target = FindObjectOfType<EnemyController>();
        if (target == null) {
            return;
        }

        int damage = GetBasicTechniqueDamage(techniqueType);
        ElementType element = GetBasicTechniqueElement(techniqueType);
        target.TakeDamage(damage, element);
        Debug.Log($"Used basic technique: {techniqueType} on {target.name} for {damage} damage.");

        TurnManager.instance.EndPlayerTurn();
    }

    private int GetBasicTechniqueDamage(TechniqueType techniqueType) {
        switch (techniqueType) {
            case TechniqueType.Fry:
                return 10;
            case TechniqueType.Boil:
                return 8;
            case TechniqueType.Chop:
                return 6;
            case TechniqueType.Mix:
                return 4;
            case TechniqueType.Bake:
                return 12;
            default:
                return 0;
        }
    }

    private ElementType GetBasicTechniqueElement(TechniqueType techniqueType) {
        switch (techniqueType) {
            case TechniqueType.Fry:
                return ElementType.Fire;
            case TechniqueType.Boil:
                return ElementType.Water;
            case TechniqueType.Chop:
                return ElementType.Earth;
            case TechniqueType.Mix:
                return ElementType.Air;
            case TechniqueType.Bake:
                return ElementType.Fire;
            default:
                return ElementType.Neutral;
        }
    }

}
