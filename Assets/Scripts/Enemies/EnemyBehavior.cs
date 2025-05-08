using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class EnemyBehavior : MonoBehaviour
{
    protected Enemy enemyStats;

    protected virtual void Awake() {
        enemyStats = GetComponent<Enemy>();
    }

    public abstract IEnumerator ExecuteAttack(); 
}
