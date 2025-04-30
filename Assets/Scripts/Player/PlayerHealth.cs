using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PlayerHealth : MonoBehaviour
{
    [Header("Health Stats")]
    public int maxHealth = 100;
    public int currentHealth;

    [Header("Events")]
    public UnityEvent<int,int> onHealthChanged;
    public UnityEvent onPlayerDamaged;
    public UnityEvent onPlayerHealed;
    public UnityEvent onPlayerDefeated;

    [Header("UI")]
    public GameObject damageTextPrefab;

    private void Start() {
        currentHealth = maxHealth;
    }

    public void TakeDamage(int damage) {
        currentHealth -= damage;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
        if (damageTextPrefab != null) {
            ShowDamageText(damage);
        } else {
            Debug.LogError("Damage text prefab is not assigned!");
        }

        onHealthChanged?.Invoke(currentHealth, maxHealth);
        onPlayerDamaged?.Invoke();

        if (currentHealth <= 0) {
            Die();
        }
    }

    public void Heal(int amount) {
        int initialHealth = currentHealth;
        currentHealth += amount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
        
        if (currentHealth > initialHealth) {
            onPlayerHealed?.Invoke();
        }
        onHealthChanged?.Invoke(currentHealth, maxHealth);
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
        Debug.Log("Player has been defeated!");
        onPlayerDefeated?.Invoke();

        if (TurnManager.instance != null) {
            TurnManager.instance.SetDefeatState();
        } else {
            Debug.LogError("TurnManager instance not found!");
        }
    }

    [ContextMenu("Test Player Take Damage")]
    public void TestTakeDamage() {
        TakeDamage(10);

        Debug.Log($"Player took 10 damage! Current health: {currentHealth}");
    }
}
