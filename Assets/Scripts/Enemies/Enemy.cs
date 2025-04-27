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
    [SerializeField] private ElementType elementalAffinity;

    [Header("Attack Settings")]
    [SerializeField] private int baseDamage;
    [SerializeField] private float attackDelay = 0.5f;

    [Header("UI")]
    public GameObject damageTextPrefab;
    [SerializeField] private TextMeshProUGUI healthText;

    private Transform playerTarget;
    
    // Element effectiveness matrix
    private static readonly float[,] elementMultipliers = new float[,] {
        // Attacking element (Fire, Water, Earth, Air, Neutral) vs defending element
        //Fire  Water  Earth  Air   Neutral
        { 1.0f, 0.75f, 1.5f,  1.0f, 1.0f }, // Fire attacking
        { 1.5f, 1.0f,  1.0f,  0.75f, 1.0f }, // Water attacking
        { 0.75f, 1.0f, 1.0f,  1.5f, 1.0f },  // Earth attacking
        { 1.0f, 1.5f, 0.75f,  1.0f, 1.0f },  // Air attacking
        { 1.0f, 1.0f, 1.0f,   1.0f, 1.0f }   // Neutral attacking
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
            ElementType elementType = ElementType.Neutral;

            switch (card.techniqueType) {
                case TechniqueType.Chop:
                    damage = 10;
                    elementType = ElementType.Neutral;
                    break;
                case TechniqueType.Fry:
                    damage = 15;
                    elementType = ElementType.Fire;
                    break;
                case TechniqueType.Boil:
                    damage = 12;
                    elementType = ElementType.Water;
                    break;
                case TechniqueType.Bake:
                    damage = 20;
                    elementType = ElementType.Fire;
                    break;
                case TechniqueType.Mix:
                    damage = 8;
                    elementType = ElementType.Air;
                    break;
                }
            
            TakeDamage(damage, elementType);

            if (card.reusable) {
                StartCoroutine(ReturnTechniqueCardNextTurn(card));
            }
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
            ElementType elementType = card.elementType;

            TakeDamage(damage, elementType);
        }
    }

    private void HandleSpiceCard(SpiceCard card) {
        if (card != null) {
            switch (card.buffType) {
                case BuffType.FireDamage:
                    TakeDamage(Mathf.RoundToInt(card.buffAmount), ElementType.Fire);
                    break;
                case BuffType.Burn: // TODO: make burn and fire damage different or remove one lol
                    TakeDamage(Mathf.RoundToInt(card.buffAmount), ElementType.Fire);
                    break;
                default:
                    Debug.LogWarning($"Unknown buff type: {card.buffType} or not implemented yet.");
                    break;
            }
        }
    }


    public void TakeDamage(int damage, ElementType elementType = ElementType.Neutral) {
        int totalDamage = CalculateDamage(damage, elementType);
        currentHealth -= totalDamage;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

        
        ShowDamageText(totalDamage);
        UpdateHealthDisplay();

        if (currentHealth <= 0) {
            Die();
        }
    }

    private int CalculateDamage(int baseDamage, ElementType damageType) {
        // Get the appropriate multiplier from the element effectiveness matrix
        float multiplier = elementMultipliers[(int)damageType, (int)elementalAffinity];
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

        Destroy(gameObject);
    }

    [ContextMenu("Test Enemy Take Damage")]
    public void TestTakeDamage() {
        TakeDamage(15, ElementType.Fire);
        Debug.Log($"{enemyName} took 15 Fire damage! Current health: {currentHealth}");
    }
}
