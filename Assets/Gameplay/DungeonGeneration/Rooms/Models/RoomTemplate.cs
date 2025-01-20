using System.Collections.Generic;
using System.Linq;
using Project.Gameplay.DungeonGeneration.Generators;
using Project.Gameplay.DungeonGeneration.Spawning;
using UnityEngine;

namespace Project.Gameplay.DungeonGeneration.Rooms.Models
{
    [System.Serializable]
    public class RoomTemplate
    {
        public string id;
        public GameObject prefab;
        public RoomType type;
        public List<DoorPoint> doorPoints;
    
        // Cache spawn points when room is initialized
        private Dictionary<SpawnPointType, List<SpawnPoint>> spawnPointsByType;
    
        public void Initialize()
        {
            // TODO: This should be called when room is instantiated
            spawnPointsByType = new Dictionary<SpawnPointType, List<SpawnPoint>>();
            var spawnPoints = prefab.GetComponentsInChildren<SpawnPoint>();
        
            foreach (var point in spawnPoints)
            {
                if (!spawnPointsByType.ContainsKey(point.Type))
                {
                    spawnPointsByType[point.Type] = new List<SpawnPoint>();
                }
                spawnPointsByType[point.Type].Add(point);
            }
        }

        public List<SpawnPoint> GetSpawnPoints(SpawnPointType type)
        {
            return spawnPointsByType.TryGetValue(type, out var points) ? points : new List<SpawnPoint>();
        }

        public SpawnPoint GetRandomSpawnPoint(SpawnPointType type, float difficulty = 0)
        {
            var validPoints = GetSpawnPoints(type)
                .Where(p => p.CanSpawn() && 
                            difficulty >= p.DifficultyMin && 
                            difficulty <= p.DifficultyMax)
                .ToList();
            
            return validPoints.Count > 0 ? 
                validPoints[Random.Range(0, validPoints.Count)] : null;
        }
    }
}
