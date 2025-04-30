using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Recipe
{
    public string recipeName;
    public List<CardData> ingredients;
    public CardData resultCard;
}
public class RecipeManager : MonoBehaviour
{
    public static RecipeManager instance;
    [SerializeField] private List<Recipe> availableRecipes = new List<Recipe>();

    private List<CardData> currentCombination = new List<CardData>();

    private void Awake() {
        if (instance == null) {
            instance = this;
        } else {
            Destroy(gameObject);
        }
    }

    public void AddCardCombination(CardData card) {
        currentCombination.Add(card);
        CheckForRecipe();
    }

    public void ClearCombination() {
        currentCombination.Clear();
    }

    public void CheckForRecipe() {
        foreach (var recipe in availableRecipes) {
            if (IsRecipeValid(recipe)) {
                CardData resultCard = recipe.resultCard;

                foreach (var ingredientCard in currentCombination) {
                    CardManager.instance.RemoveCardFromHand(ingredientCard);
                }

                CardManager.instance.AddCardToHand(resultCard);
                ClearCombination();
                return;
            }
        }
    }

    public bool IsRecipeValid(Recipe recipe) {
        if (currentCombination.Count != recipe.ingredients.Count) {
            return false;
        }

        List<CardData> recipeIngredients = new List<CardData>(recipe.ingredients);
        List<CardData> combination = new List<CardData>(currentCombination);

        foreach (var card in combination) {
            bool foundMatch = false;

            for (int i = 0; i < recipeIngredients.Count; i++) {
                if (card.name == recipeIngredients[i].cardName) {
                    recipeIngredients.RemoveAt(i);
                    foundMatch = true;
                    break;
                }
            }

            if (!foundMatch) {
                return false;
            }
        }

        return recipeIngredients.Count == 0;
    }

}
