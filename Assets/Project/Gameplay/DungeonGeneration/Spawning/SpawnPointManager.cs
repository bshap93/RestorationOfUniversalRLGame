using UnityEngine;
using DunGen;
using System.Linq;
using System.Collections.Generic;

namespace Project.Gameplay.DungeonGeneration.Spawning
{
    public class SpawnPointManager : MonoBehaviour
    {
        private Dictionary<Tile, List<SpawnPoint>> _spawnPointsByTile;
        private Dictionary<string, SpawnPoint> _spawnPointsById;

        private void Awake()
        {
            _spawnPointsByTile = new Dictionary<Tile, List<SpawnPoint>>();
            _spawnPointsById = new Dictionary<string, SpawnPoint>();
        }

        public void InitializeSpawnPoints(Dungeon dungeon)
        {
            _spawnPointsById.Clear();
            _spawnPointsById.Clear();

            // Gather all spawn points from all tiles
            foreach (var tile in dungeon.AllTiles)
            {
                var points = tile.GetComponentsInChildren<SpawnPoint>();
                _spawnPointsByTile[tile] = points.ToList();
            
                foreach (var point in points)
                {
                    // Now using runtime-generated IDs
                    var pointId = point.PointId;
                    _spawnPointsById[pointId] = point;
                }
            }
        }

        // Find specific spawn point by ID
        public SpawnPoint GetSpawnPointById(string id)
        {
            return _spawnPointsById.TryGetValue(id, out var point) ? point : null;
        }

        // Get all spawn points in a specific tile
        public List<SpawnPoint> GetSpawnPointsInTile(Tile tile, SpawnPointType? type = null)
        {
            if (!_spawnPointsByTile.TryGetValue(tile, out var points))
                return new List<SpawnPoint>();

            if (!type.HasValue)
                return points;

            return points.Where(p => p.Type == type.Value).ToList();
        }

        // Find all available spawn points of a specific type
        public List<SpawnPoint> GetAvailableSpawnPoints(SpawnPointType type)
        {
            return _spawnPointsByTile.Values
                .SelectMany(points => points)
                .Where(p => p.Type == type && p.CanSpawn())
                .ToList();
        }

        // Find player spawn point for level transition
        public PlayerSpawnPoint FindLevelTransitionSpawn(string spawnId, SpawnDirection direction)
        {
            var point = GetSpawnPointById(spawnId) as PlayerSpawnPoint;
            if (point != null)
                return point;

            // Fallback: find any valid transition point of the right type
            var validPoints = _spawnPointsByTile.Values
                .SelectMany(points => points)
                .OfType<PlayerSpawnPoint>()
                .Where(p => direction == SpawnDirection.Up ? 
                    p.SpawnType == PlayerSpawnPoint.PlayerSpawnType.LevelEntry :
                    p.SpawnType == PlayerSpawnPoint.PlayerSpawnType.LevelExit)
                .ToList();

            return validPoints.FirstOrDefault();
        }

        // Find random valid spawn point
        public SpawnPoint GetRandomSpawnPoint(
            SpawnPointType type, 
            float difficulty = 0f,
            Tile preferredTile = null)
        {
            var validPoints = GetAvailableSpawnPoints(type);

            // Filter by difficulty if it's an enemy spawn
            if (type == SpawnPointType.Enemy || type == SpawnPointType.Boss)
            {
                validPoints = validPoints
                    .OfType<EnemySpawnPoint>()
                    .Where(p => difficulty >= p.DifficultyMin && difficulty <= p.DifficultyMax)
                    .Cast<SpawnPoint>()
                    .ToList();
            }

            // Prefer points in the specified tile if one is given
            if (preferredTile != null)
            {
                var preferredPoints = validPoints
                    .Where(p => p.transform.IsChildOf(preferredTile.transform))
                    .ToList();

                if (preferredPoints.Any())
                    validPoints = preferredPoints;
            }

            return validPoints.Count > 0 ? 
                validPoints[Random.Range(0, validPoints.Count)] : 
                null;
        }

        // Find nearest spawn point to a position
        public SpawnPoint GetNearestSpawnPoint(
            Vector3 position, 
            SpawnPointType type,
            float maxDistance = float.MaxValue)
        {
            return GetAvailableSpawnPoints(type)
                .OrderBy(p => Vector3.Distance(position, p.transform.position))
                .FirstOrDefault(p => Vector3.Distance(position, p.transform.position) <= maxDistance);
        }

        // Get all spawn points in a radius
        public List<SpawnPoint> GetSpawnPointsInRadius(
            Vector3 center, 
            float radius, 
            SpawnPointType? type = null)
        {
            var points = _spawnPointsByTile.Values
                .SelectMany(p => p)
                .Where(p => Vector3.Distance(center, p.transform.position) <= radius);

            if (type.HasValue)
                points = points.Where(p => p.Type == type.Value);

            return points.ToList();
        }
    }
}