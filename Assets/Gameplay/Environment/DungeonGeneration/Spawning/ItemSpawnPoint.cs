using UnityEngine;

namespace Project.Gameplay.DungeonGeneration.Spawning
{
    // For items and pickups that should persist
    public class ItemSpawnPoint : SpawnPoint
    {
        public enum ItemSpawnType
        {
            Treasure,   // Valuable items
            Resource,   // Crafting/consumables
            Key,        // Important progression items
            Secret     // Hidden/bonus items
        }

        [SerializeField] private ItemSpawnType itemType;
        [SerializeField] private bool respawns = false;
        [SerializeField] private float respawnTime = 300f;

        private float _lastCollectTime;

        public override bool CanSpawn()
        {
            if (respawns && isOccupied)
            {
                return Time.time - _lastCollectTime >= respawnTime;
            }
            return !isOccupied;
        }
    }

}
