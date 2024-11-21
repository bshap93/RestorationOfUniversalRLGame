using Project.Core.SaveSystem;
using UnityEngine;

namespace Project.Gameplay.DungeonGeneration.Spawning
{
    public class PlayerSpawnPoint : SpawnPoint
    {
        
        public enum PlayerSpawnType
        {
            LevelStart,     // New game start point
            LevelEntry,     // Coming UP from a deeper level
            LevelExit,      // Going DOWN to a deeper level
            Checkpoint,     // Save/restore point
        }

        [SerializeField] private PlayerSpawnType playerSpawnType;
        [SerializeField] private string connectedLevelId;      // Which level this connects to
        [SerializeField] private string connectedSpawnId;      // Which spawn point in the other level

        public PlayerSpawnType SpawnType => playerSpawnType;
        
        // Used when transitioning between levels
        public string GetConnectedSpawnId(SpawnDirection direction)
        {
            return connectedSpawnId;
        }
        
        public override void OnLevelTransition(SpawnDirection direction)
        {
            // Save last used transition point for returning to this level
            NewSaveManager.Instance.SetLastTransitionPoint(
                gameObject.scene.name, 
                PointId, 
                direction
            );
        }
        
        // Additional player-specific spawn properties
        [SerializeField] private bool shouldRestoreHealth = true;
        [SerializeField] private bool shouldSaveCheckpoint = true;

    }
}
