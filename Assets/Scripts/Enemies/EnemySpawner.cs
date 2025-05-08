using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] private GameObject enemyPrefab;
    [SerializeField] private EnemyData[] availableEnemyTypes;
    [SerializeField] private Transform[] spawnPoints;
    
    [Header("Spawn Settings")]
    [SerializeField] private int minEnemies = 1;
    [SerializeField] private int maxEnemies = 4;
    [SerializeField] private bool spawnImmediately = true;
    
    private void Start() {
        if (spawnImmediately) {
            SpawnRandomEnemies();
        }
    }
    
    public void SpawnRandomEnemies() {
        int enemyCount = Random.Range(minEnemies, maxEnemies + 1);
        enemyCount = Mathf.Min(enemyCount, spawnPoints.Length);
        
        // Shuffled list of spawn points
        List<Transform> shuffledSpawnPoints = new List<Transform>(spawnPoints);
        shuffledSpawnPoints.Shuffle();
        
        for (int i = 0; i < enemyCount; i++) {
            GameObject enemyObject = Instantiate(enemyPrefab, shuffledSpawnPoints[i].position, Quaternion.identity);
            Enemy enemy = enemyObject.GetComponent<Enemy>();
            
            if (enemy != null) {
                EnemyData randomType = availableEnemyTypes[Random.Range(0, availableEnemyTypes.Length)];
                enemy.SetupEnemyData(randomType);
            }
        }
        TurnManager.instance.RefreshEnemyList();
    }
}
