using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [Header("Enemy Stats")]
    [SerializeField] private string enemyName;
    [SerializeField] private int maxHealth = 100;
    [SerializeField] private int currentHealth;
    [SerializeField] private TasteType tasteAffinity;

    [Header("Attack Settings")]
    [SerializeField] private int baseDamage;
    [SerializeField] private float attackDelay = 0.5f;

    [Header("UI")]
    public GameObject damageTextPrefab;
    [SerializeField] private TextMeshProUGUI healthText;

    private Transform playerTarget;
    
    // Element effectiveness matrix
    private static readonly float[,] tasteEffectiveness = new float[,] {
    //            Sweet  Sour   Salty  Bitter Umami  Spicy  Neutral
    /* Sweet */  { 1.0f, 0.75f, 0.75f, 1.5f,  1.5f,  1.0f,  1.0f },
    /* Sour */   { 1.5f, 1.0f,  1.5f,  0.75f, 1.0f,  0.75f, 1.0f },
    /* Salty */  { 1.5f, 0.75f, 1.0f,  0.75f, 1.5f,  1.0f,  1.0f },
    /* Bitter */ { 0.75f, 1.5f, 1.5f,  1.0f,  0.75f, 1.5f,  1.0f },
    /* Umami */  { 0.75f, 1.0f, 0.75f, 1.5f,  1.0f,  1.5f,  1.0f },
    /* Spicy */  { 1.0f, 1.5f,  1.5f,  0.75f, 0.75f, 1.0f,  1.0f },
    /* Neutral */{ 1.0f, 1.0f,  1.0f,  1.0f,  1.0f,  1.0f,  1.0f }
};


    private void Start() {
        currentHealth = maxHealth;
        UpdateHealthDisplay();

        PlayerHealth player = FindAnyObjectByType<PlayerHealth>();
        if (player != null) {
            playerTarget = player.transform;
        }
    }

    public bool IsAlive() {
        return currentHealth > 0;
    }

    public IEnumerator TakeAction() {
        yield return StartCoroutine(AttackPlayer());
    }

    private IEnumerator AttackPlayer() {
        Debug.Log($"{enemyName} is attacking the player!");

        yield return new WaitForSeconds(attackDelay);

        if (playerTarget != null) {
            PlayerHealth playerHealth = playerTarget.GetComponent<PlayerHealth>();
            if (playerHealth != null) {
                playerHealth.TakeDamage(baseDamage); // TODO add more stuff
            }
        }
    }

    public void ReceiveCard(CardData card) {
        switch (card.cardType) {
            case CardType.Technique:
                HandleTechniqueCard(card as TechniqueCard);
                break;
            case CardType.Ingredient:
                HandleIngredientCard(card as IngredientCard);
                break;
            case CardType.Spice:
                HandleSpiceCard(card as SpiceCard);
                break;
        }
    }

    private void HandleTechniqueCard(TechniqueCard card) {
        if (card != null) {
            int damage = 0;

            // TODO: Change how technique cards do damage
            switch (card.techniqueType) {
                case TechniqueType.Chop:
                    damage = 10;
                    break;
                case TechniqueType.Fry:
                    damage = 15;
                    break;
                case TechniqueType.Boil:
                    damage = 12;
                    break;
                case TechniqueType.Bake:
                    damage = 20;
                    break;
                case TechniqueType.Mix:
                    damage = 8;
                    break;
                }
            
            TakeDamage(damage, TasteType.Neutral);
            StartCoroutine(ReturnTechniqueCardNextTurn(card));
        }
    }

    private IEnumerator ReturnTechniqueCardNextTurn(TechniqueCard card) {
        int currentTurnCount = TurnManager.instance.GetPlayerTurnCount();

        yield return new WaitUntil(() => TurnManager.instance.IsPlayerTurn() && TurnManager.instance.GetPlayerTurnCount() > currentTurnCount);

        if (CardManager.instance != null) {
            CardManager.instance.ReturnTechnniqueCard(card);
        }
    }

    private void HandleIngredientCard(IngredientCard card) {
        if (card != null) {
            int damage = 5;
            TasteType tasteType = card.tasteType;

            TakeDamage(damage, tasteType);
        }
    }

    private void HandleSpiceCard(SpiceCard card) {
        if (card != null) {
            switch (card.buffType) {
                case BuffType.Burn:
                    TakeDamage(Mathf.RoundToInt(card.buffAmount), TasteType.Spicy);
                    break;
                default:
                    Debug.LogWarning($"Unknown buff type: {card.buffType} or not implemented yet.");
                    break;
            }
        }
    }


    public void TakeDamage(int damage, TasteType tasteType = TasteType.Neutral) {
        int totalDamage = CalculateDamage(damage, tasteType);
        currentHealth -= totalDamage;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

        
        ShowDamageText(totalDamage);
        UpdateHealthDisplay();

        if (currentHealth <= 0) {
            Die();
        }
    }

    private int CalculateDamage(int baseDamage, TasteType damageType) {
        // Get the appropriate multiplier from the element effectiveness matrix
        float multiplier = tasteEffectiveness[(int)damageType, (int)tasteAffinity];
        return Mathf.RoundToInt(baseDamage * multiplier);
    }

    private void ShowDamageText(int damage) {
        Canvas mainCanvas = FindObjectOfType<Canvas>();
        if (mainCanvas == null) {
            Debug.LogError("No Canvas found in the scene!");
            return;
        }

        GameObject damageTextObject = Instantiate(damageTextPrefab, mainCanvas.transform);
        RectTransform rectTransform = damageTextObject.GetComponent<RectTransform>();
        if (rectTransform != null) {
            Vector2 screenPosition = RectTransformUtility.WorldToScreenPoint(Camera.main, transform.position + Vector3.up);
            rectTransform.position = screenPosition;
        }

        DamageText damageText = damageTextObject.GetComponent<DamageText>();
        if (damageText != null) {
            damageText.SetValue(damage);
        } else {
            Debug.LogError("DamageText component not found on the prefab!");
        }
    }

    private void UpdateHealthDisplay() {
        if (healthText != null) {
            healthText.text = $"{enemyName}: {currentHealth}/{maxHealth}";
        } else {
            Debug.LogError("Health Text UI element not assigned!");
        }
    }

    private void Die() {
        Debug.Log($"{enemyName} has died!");

        // TODO: Spawn Loot, Ingredients, etc.

        Enemy[] remainingEnemies = FindObjectsOfType<Enemy>();
        int aliveEnemies = 0;
        foreach (var enemy in remainingEnemies) {
            if (enemy.IsAlive()) {
                aliveEnemies++;
            }
        }

        if (aliveEnemies == 0 && TurnManager.instance != null) {
            TurnManager.instance.SetVictoryState();
        }

        Destroy(gameObject);
    }

    [ContextMenu("Test Enemy Take Damage")]
    public void TestTakeDamage() {
        TakeDamage(15, TasteType.Sweet);
        Debug.Log($"{enemyName} took 15 Fire damage! Current health: {currentHealth}");
    }
}
