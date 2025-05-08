// Basic melee enemy that attacks with simple patterns
using System.Collections;
using UnityEngine;

public class BasicEnemyBehavior : EnemyBehavior
{
    [SerializeField] private int[] damagePattern = { 10, 8, 12 }; // Different damage values in a pattern
    private int currentPatternIndex = 0;
    
    public override IEnumerator ExecuteAttack()
    {
        int damage = damagePattern[currentPatternIndex];
        currentPatternIndex = (currentPatternIndex + 1) % damagePattern.Length;
        
        // TODO: Attack animation/feedback
        yield return new WaitForSeconds(0.5f);

        PlayerHealth player = FindObjectOfType<PlayerHealth>();
        if (player != null)
        {
            player.TakeDamage(damage);
        }
    }
}

