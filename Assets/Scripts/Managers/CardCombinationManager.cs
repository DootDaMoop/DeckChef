using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class CardCombination
{
    public CardData ingredientCard;
    public CardData techniqueCard;
    public CardData resultCard;
}

public class CardCombinationManager : MonoBehaviour
{
    public static CardCombinationManager instance;
    [SerializeField] private List<CardCombination> cardCombinations = new List<CardCombination>();
    private Dictionary<string, CardData> cardCombinationLibrary = new Dictionary<string, CardData>();

    private void Awake() {
        if (instance == null) {
            instance = this;
        } else{
            Destroy(gameObject);
        }

        foreach (var combination in cardCombinations) {
            string key1 = GetCombinationKey(combination.ingredientCard, combination.techniqueCard);
            string key2 = GetCombinationKey(combination.techniqueCard, combination.ingredientCard);
            cardCombinationLibrary[key1] = combination.resultCard;
            cardCombinationLibrary[key2] = combination.resultCard;   
        }
    }

    public bool CanCombine(CardData card1, CardData card2) {
        string key = GetCombinationKey(card1, card2);
        Debug.Log($"Checking combination: {key}");
        return cardCombinationLibrary.ContainsKey(key);
    }

    public CardData GetCombinationResults(CardData card1, CardData card2) {
        string key = GetCombinationKey(card1, card2);
        if (cardCombinationLibrary.TryGetValue(key, out CardData resultCard)) {
            return resultCard;
        }
        return null;
    }

    private string GetCombinationKey(CardData card1, CardData card2) {
        return $"{card1.cardName}_{card2.cardName}";
    }
}
