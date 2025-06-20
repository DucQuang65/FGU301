using UnityEngine;
using System.Collections;
public class EnemySpawner : MonoBehaviour
{
    [SerializeField] private GameObject[] enemies;
    [SerializeField] private GameObject[] BossEnemy;
    [SerializeField] private Transform[] spawnPoint;
    [SerializeField] private float initialTimeBetweenSpawns = 2f; // Initial spawn delay
    [SerializeField] private float minTimeBetweenSpawns = 0.5f; // Minimum spawn delay
    [SerializeField] private float spawnTimeReduction = 0.1f; // Spawn time reduction per round
    [SerializeField] private float hpIncreasePercent = 5f; // HP increase per round (5%)
    [SerializeField] private float roundDuration = 300f; // 5 minutes in seconds
    [SerializeField] private float breakDuration = 30f; // 30 seconds break
    [SerializeField] private int bossRoundInterval = 5; // Boss spawns every 5 rounds
    [SerializeField] private MainTower mainTower; // Reference to the tower
    private float timeBetweenSpawns;
    private int round = 1;
    void Start()
    {
        timeBetweenSpawns = initialTimeBetweenSpawns;
        StartCoroutine(SpawnEnemies());
    }
    private IEnumerator SpawnEnemies()
    {
        while (true)
        {
            yield return new WaitForSeconds(timeBetweenSpawns);
            GameObject enemyPrefab = (round % bossRoundInterval == 0) ? 
                BossEnemy[Random.Range(0, BossEnemy.Length)] : enemies[Random.Range(0, enemies.Length)];
            Transform spawn = spawnPoint[Random.Range(0, spawnPoint.Length)];
            GameObject enemy = Instantiate(enemyPrefab, spawn.position, Quaternion.identity);
        }
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
