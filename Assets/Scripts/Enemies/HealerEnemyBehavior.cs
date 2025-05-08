// Basic melee enemy that attacks with simple patterns
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealerEnemyBehavior : EnemyBehavior 
{
    [SerializeField] private int healAmount = 10;
    [SerializeField] private int attackDamage = 5;
    private bool willHealNextTurn = true;
    
    public override IEnumerator ExecuteAttack() {
        if (willHealNextTurn && FindAlliesInNeed().Count > 0) {
            // TODO: Healing animation/effect
            yield return new WaitForSeconds(0.6f);
            
            // Find and heal allies
            List<Enemy> alliesInNeed = FindAlliesInNeed();
            if (alliesInNeed.Count > 0)
            {
                Enemy targetAlly = alliesInNeed[Random.Range(0, alliesInNeed.Count)];
                targetAlly.HealDamage(healAmount);
                
                // Visual healing effect between this enemy and the target
                StartCoroutine(ShowHealingEffect(targetAlly.transform.position));
            }
        } else {
            // TODO: Attack animation
            yield return new WaitForSeconds(0.4f);
            
            // Basic attack when no healing is needed
            PlayerHealth player = FindObjectOfType<PlayerHealth>();
            if (player != null)
            {
                player.TakeDamage(attackDamage);
            }
        }
        
        // Toggle behavior for next turn
        willHealNextTurn = !willHealNextTurn;
    }
    
    private List<Enemy> FindAlliesInNeed()
    {
        List<Enemy> alliesInNeed = new List<Enemy>();
        Enemy[] allEnemies = FindObjectsOfType<Enemy>();
        
        foreach (Enemy ally in allEnemies)
        {
            // Don't include self and only include enemies that need healing
            if (ally != enemyStats && ally.IsAlive() && ally.NeedsHealing())
            {
                alliesInNeed.Add(ally);
            }
        }
        
        return alliesInNeed;
    }
    
    private IEnumerator ShowHealingEffect(Vector3 targetPosition)
    {
        // TODO: Instantiate healing particles or a line renderer here
        // This is a placeholder for visual feedback
        Debug.DrawLine(transform.position, targetPosition, Color.green, 0.5f);
        yield return null;
    }
}
