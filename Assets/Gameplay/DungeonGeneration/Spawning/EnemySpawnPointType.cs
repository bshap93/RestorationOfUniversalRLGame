using UnityEngine;

namespace Project.Gameplay.DungeonGeneration.Spawning
{
    // Example of a specialized enemy spawn point
    public class EnemySpawnPoint : SpawnPoint
    {
        [SerializeField] private float spawnDelay = 0f;
        [SerializeField] private bool respawnEnabled = false;
        [SerializeField] private float respawnTime = 30f;

        private float _lastSpawnTime;

        public override bool CanSpawn()
        {
            if (!base.CanSpawn()) return false;
            
            if (respawnEnabled)
            {
                return Time.time - _lastSpawnTime >= respawnTime;
            }
            
            return true;
        }
        
        // For save/load of enemy state
        public string GetEnemySpawnData()
        {
            return isOccupied ? PointId : null;
        }

        public void RestoreEnemySpawnData(bool wasKilled)
        {
            isOccupied = wasKilled;
        }
    }
}
