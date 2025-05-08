using UnityEngine;
using TMPro;

public class ShieldDisplay : MonoBehaviour
{
    [SerializeField] private GameObject shieldTextObject;
    [SerializeField] private TextMeshProUGUI shieldValueText;
    
    // Component can be attached to either player or enemy
    private void Start() {
        if (shieldValueText == null) {
            shieldValueText = shieldTextObject.GetComponentInChildren<TextMeshProUGUI>();
        }

        PlayerHealth playerHealth = GetComponent<PlayerHealth>();
        if (playerHealth != null) {
            playerHealth.onShieldChanged.AddListener(UpdateShieldValue);
            UpdateShieldValue(playerHealth.currentShield);
            return;
        }
        
        Enemy enemy = GetComponent<Enemy>();
        if (enemy != null) {
            enemy.onShieldChanged.AddListener(UpdateShieldValue);
            UpdateShieldValue(enemy.shieldAmount);
        }
    }
    
    private void UpdateShieldValue(int shieldAmount)
    {
        shieldTextObject.SetActive(shieldAmount > 0);
        
        if (shieldAmount > 0) {
            shieldValueText.text = shieldAmount.ToString();
        }
    }
    
    private void OnDestroy()
    {
        PlayerHealth playerHealth = GetComponent<PlayerHealth>();
        if (playerHealth != null) {
            playerHealth.onShieldChanged.RemoveListener(UpdateShieldValue);
        }
        
        Enemy enemy = GetComponent<Enemy>();
        if (enemy != null) {
            enemy.onShieldChanged.RemoveListener(UpdateShieldValue);
        }
    }
}