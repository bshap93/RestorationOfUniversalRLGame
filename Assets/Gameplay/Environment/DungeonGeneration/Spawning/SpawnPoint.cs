using System;
using UnityEngine;

namespace Project.Gameplay.DungeonGeneration.Spawning
{
    public enum SpawnPointType
    {
        Enemy,
        Item,
        Pickup,
        Player,
        Boss,
        Decoration
    }

    // Direction of level transition
    public enum SpawnDirection
    {
        Up, // Moving to previous level
        Down // Moving to next level
    }

    [Serializable]
    public class SpawnPoint : MonoBehaviour
    {
        [SerializeField] protected SpawnPointType type;
        [SerializeField] protected string basePointId; // The ID set in the prefab

        [SerializeField] protected bool oneTimeUse = true;
        [SerializeField] protected bool isOccupied;

        // Optional spawn weights/restrictions
        [SerializeField] float difficultyMin;
        [SerializeField] float difficultyMax = float.MaxValue;
        [SerializeField] string[] allowedPrefabTags;
        protected string RuntimePointId; // Generated at runtime based on tile/instance
        public string PointId => RuntimePointId;

        // Getters
        public SpawnPointType Type => type;
        public float DifficultyMin => difficultyMin;
        public float DifficultyMax => difficultyMax;
        public string[] AllowedPrefabTags => allowedPrefabTags;

        public void Reset()
        {
            isOccupied = false;
        }

        // Optional visualization for editor
        void OnDrawGizmos()
        {
            Gizmos.color = type switch
            {
                SpawnPointType.Enemy => Color.red,
                SpawnPointType.Item => Color.yellow,
                SpawnPointType.Pickup => Color.green,
                SpawnPointType.Player => Color.blue,
                SpawnPointType.Boss => Color.magenta,
                SpawnPointType.Decoration => Color.gray,
                _ => Color.white
            };

            Gizmos.DrawWireSphere(transform.position, 0.5f);
            if (isOccupied) Gizmos.DrawWireCube(transform.position, Vector3.one * 0.3f);
        }


        // For level transitions
        public virtual void OnLevelTransition(SpawnDirection direction)
        {
        }


        public virtual bool CanSpawn()
        {
            return !isOccupied || !oneTimeUse;
        }


        public void MarkOccupied()
        {
            isOccupied = true;
        }
    }
}
