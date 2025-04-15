using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    [Header("Enemy Stats")]
    public string enemyName;
    public int maxHealth = 100;
    public int currentHealth;
    public ElementType elementalAffinity;

    [Header("Attack Settings")]
    public int baseDamage;
    public float attackDelay = 0.5f;

    [Header("UI")]
    public GameObject damageTextPrefab;

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

    public void TakeDamage(int damage, ElementType elementType = ElementType.Neutral) {
        int totalDamage = CalculateDamage(damage, elementType);
        currentHealth -= totalDamage;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

        if (damageTextPrefab != null) {
            ShowDamageText(totalDamage);
        }

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
