using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour
{
    [Header("Health Stats")]
    public int maxHealth = 100;
    public int currentHealth;
    public int currentShield = 0;

    [Header("Events")]
    public UnityEvent<int,int> onHealthChanged;
    public UnityEvent onPlayerDamaged;
    public UnityEvent onPlayerHealed;
    public UnityEvent onPlayerDefeated;
    public UnityEvent onShieldApplied;
    public UnityEvent<int> onShieldChanged;

    [Header("UI")]
    public GameObject damageTextPrefab;
    public GameObject shieldTextObject;

    private void Start() {
        currentHealth = maxHealth;
    }

    public void TakeDamage(int damage) {
        if (currentShield > 0) {
            int shieldDamage = Mathf.Min(currentShield, damage);
            currentShield -= shieldDamage;
            damage -= shieldDamage;
            onShieldChanged?.Invoke(currentShield);

            if (currentShield <= 0) {
                Debug.Log("Shield broken!");
            }
        }

        if (damage > 0) {
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

    public void AddShield(int amount) {
        currentShield += amount;
        onShieldChanged?.Invoke(currentShield);
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
            Text textComponent = damageTextObject.GetComponent<Text>();
            if (textComponent != null) {
                textComponent.color = Color.red;
            }
        } else {
            Debug.LogError("DamageText component not found on the prefab!");
        }
    }
    
    // private void ShowShieldText(int shieldAmount) {
    //     Canvas mainCanvas = FindObjectOfType<Canvas>();
    //     if (mainCanvas == null) {
    //         Debug.LogError("No Canvas found in the scene!");
    //         return;
    //     }
        
    //     GameObject shieldTextObject = Instantiate(damageTextPrefab, mainCanvas.transform);
    //     RectTransform rectTransform = shieldTextObject.GetComponent<RectTransform>();
    //     if (rectTransform != null) {
    //         Vector2 screenPosition = RectTransformUtility.WorldToScreenPoint(Camera.main, transform.position + Vector3.up);
    //         rectTransform.position = screenPosition;
    //     }

    //     if (shieldTextObject != null && shieldTextObject.activeSelf) {
    //         if (currentShield > 0) {
    //             TextMeshProUGUI shieldText = shieldTextObject.GetComponent<TextMeshProUGUI>();
    //             if (shieldText != null) {
    //                 shieldText.text = $"+{shieldAmount} Shield";
    //                 shieldText.color = Color.blue;
    //             } else {
    //                 Debug.LogError("Text component not found on the shield text object!");
    //             }
    //         } else {
    //            Destroy(shieldTextObject);
    //         }
    //     } else {
    //         Debug.LogError("Shield text object is not assigned!");
    //     }
    // }

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
