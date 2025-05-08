using System.Collections;
using UnityEngine;

// Defensive enemy that alternates between attacking and buffing itself
public class DefensiveEnemyBehavior : EnemyBehavior
{
    [SerializeField] private int attackDamage = 8;
    [SerializeField] private int shieldAmount = 5;
    private bool willAttackNextTurn = true;
    
    public override IEnumerator ExecuteAttack()
    {
        if (willAttackNextTurn)
        {
            // Attack animation
            yield return new WaitForSeconds(0.5f);
            
            // Attack player
            PlayerHealth player = FindObjectOfType<PlayerHealth>();
            if (player != null)
            {
                player.TakeDamage(attackDamage);
            }
        }
        else
        {
            // TODO: Shield/buff animation
            yield return new WaitForSeconds(0.5f);
            
            enemyStats.AddShield(shieldAmount);
        }
        
        willAttackNextTurn = !willAttackNextTurn;
    }
}

