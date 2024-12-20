using UnityEngine;

public class SpawnEnemiesInTile : MonoBehaviour
{
    public float SpawnRate = 0.5f; // Percentage chance that each spawn point will spawn an enemy
    public bool HasSpawnedEnemies { get; private set; } = false; // Prevent multiple spawns

    private EnemySpawnPoint[] _enemySpawnPoints;

    public void SpawnEnemies()
    {
        // Do not spawn enemies more than once
        if (HasSpawnedEnemies) return;

        Debug.Log($"Spawning enemies for tile: {gameObject.name}");
        
        // Find all enemy spawn points inside this tile
        _enemySpawnPoints = GetComponentsInChildren<EnemySpawnPoint>();

        foreach (var enemySpawnPoint in _enemySpawnPoints)
        {
            if (Random.value < SpawnRate)
            {
                // Instantiate a random enemy at the spawn point
                var enemy = Instantiate(
                    enemySpawnPoint.EnemyClass.GetRandomEnemyPrefab(), 
                    enemySpawnPoint.transform.position, 
                    Quaternion.identity
                );

                enemy.transform.SetParent(enemySpawnPoint.transform);
            }
        }

        // Mark this tile as "spawned"
        HasSpawnedEnemies = true;
        Debug.Log($"Enemies have been spawned for {gameObject.name}");
    }
}
