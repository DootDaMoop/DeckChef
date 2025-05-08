using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

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

    [Header("Enemy Behavior")]
    [SerializeField] private EnemyData enemyData;
    [SerializeField] private EnemyBehavior enemyBehavior;
    public int shieldAmount = 0;

    [Header("UI")]
    public GameObject damageTextPrefab;
    [SerializeField] private TextMeshProUGUI healthText;
    [Header("Events")]
    public UnityEvent<int> onShieldChanged = new UnityEvent<int>();

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

    private void Awake() {
        if (enemyData != null) {
            SetupEnemyData(enemyData);
        }
    }

    private void Start() {
        currentHealth = maxHealth;
        UpdateHealthDisplay();
        enemyBehavior = GetComponent<EnemyBehavior>();

        PlayerHealth player = FindAnyObjectByType<PlayerHealth>();
        if (player != null) {
            playerTarget = player.transform;
        }
    }

    public void SetupEnemyData(EnemyData data) {
        enemyName = data.enemyName;
        maxHealth = data.maxHealth;
        currentHealth = maxHealth;
        tasteAffinity = data.tasteAffinity;
        baseDamage = data.baseDamage;
        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer && data.enemySprite) {
            spriteRenderer.sprite = data.enemySprite;
        }

        UpdateHealthDisplay();
        AddEnemyBehavior(data.enemyBehaviorType);
    }

    private void AddEnemyBehavior(EnemyData.EnemyBehaviorType behaviorType) {
        EnemyBehavior existingBehavior = GetComponent<EnemyBehavior>();
        if (existingBehavior != null) {
            Destroy(existingBehavior);
        }

        switch (behaviorType) {
            case EnemyData.EnemyBehaviorType.Basic:
                enemyBehavior = gameObject.AddComponent<BasicEnemyBehavior>();
                break;
            case EnemyData.EnemyBehaviorType.Defensive:
                enemyBehavior = gameObject.AddComponent<DefensiveEnemyBehavior>();
                break;
            case EnemyData.EnemyBehaviorType.Healer:
                enemyBehavior = gameObject.AddComponent<HealerEnemyBehavior>();
                break;
        }
    }

    public bool IsAlive() {
        return currentHealth > 0;
    }

    public IEnumerator TakeAction() {
        if (enemyBehavior != null) {
            yield return StartCoroutine(enemyBehavior.ExecuteAttack());
        } else {
            yield return StartCoroutine(AttackPlayer());
        }
    }

    public void AddShield(int amount) {
        shieldAmount += amount;
        onShieldChanged?.Invoke(shieldAmount);
        // TODO: Add visual indicator for shield
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

        if (shieldAmount > 0) {
            if (shieldAmount >= totalDamage) {
                shieldAmount -= totalDamage;
                totalDamage = 0;
            } else {
                totalDamage -= shieldAmount;
                shieldAmount = 0;
            }
        }

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
            TextMeshProUGUI textComponent = damageTextObject.GetComponent<TextMeshProUGUI>();
            if (textComponent != null) {
                textComponent.color = Color.red; // Red for dmg
            }
        } else {
            Debug.LogError("DamageText component not found on the prefab!");
        }
    }

    private void ShowHealingText(int healAmount) {
        Canvas mainCanvas = FindObjectOfType<Canvas>();
        if (mainCanvas == null || damageTextPrefab == null) {
            Debug.LogError("No Canvas or DamageTextPrefab found in the scene!");
            return;
        }

        GameObject healingTextObject = Instantiate(damageTextPrefab, mainCanvas.transform);
        RectTransform rectTransform = healingTextObject.GetComponent<RectTransform>();
        if (rectTransform != null) {
            Vector2 screenPosition = RectTransformUtility.WorldToScreenPoint(Camera.main, transform.position + Vector3.up);
            rectTransform.position = screenPosition;
        }

        DamageText healingText = healingTextObject.GetComponent<DamageText>();
        if (healingText != null) {
            healingText.SetValue(healAmount);
            TextMeshProUGUI textComponent = healingText.GetComponent<TextMeshProUGUI>();
            if (textComponent != null) {
                textComponent.color = Color.green; // Green for heal
            }
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

    public bool NeedsHealing() {
        // Healing at 60% health
        return currentHealth < (maxHealth * 0.6f);
    }

    public void HealDamage(int amount) {
        int previousHealth = currentHealth;
        currentHealth += amount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

        if (currentHealth > previousHealth){
            ShowHealingText(currentHealth - previousHealth);
        }

        UpdateHealthDisplay();
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
