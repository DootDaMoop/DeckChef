using UnityEngine;

[CreateAssetMenu(fileName = "New Enemy", menuName = "Enemy/Enemy")]
public class EnemyData : ScriptableObject
{
    [Header("Enemy Stats")]
    public string enemyName;
    public int maxHealth = 100;
    public TasteType tasteAffinity;
    public Sprite enemySprite;
    public bool isBoss = false;

    [Header("Combat Stats")]
    public int baseDamage = 10;
    public EnemyBehaviorType enemyBehaviorType;

    [Header("Rewards")]
    public CardData[] possibleRewards;
    public float dropChance = 0.5f;

    [System.Serializable]
    public enum EnemyBehaviorType {
        Basic,
        Defensive,
        Healer,
        // TODO: More enemy types

    }
}