using System;
using System.Collections.Generic;
using Project.Gameplay.DungeonGeneration.Spawning;
using UnityEngine;

namespace Project.Core.SaveSystem
{
    [Serializable]
    public class SaveData
    {
        public string saveVersion = "1.0.0"; // For save compatibility checking
        public int seed; // For dungeon regeneration
        public PlayerSaveData playerData;
        public DungeonSaveData dungeonData;
        public GameStateSaveData gameState;
        public DateTime timestamp;

        public SaveData()
        {
            timestamp = DateTime.Now;
            playerData = new PlayerSaveData();
            dungeonData = new DungeonSaveData();
            gameState = new GameStateSaveData();
        }
    }

    [Serializable]
    public class PlayerSaveData
    {
        public SerializableVector3 position;
        public SerializableQuaternion rotation;
        public float currentHealth;
        public float maxHealth;
        public InventorySaveData inventory;
        public Dictionary<string, float> stats; // Flexible stats system

        public PlayerSaveData()
        {
            position = new SerializableVector3(Vector3.zero);
            rotation = new SerializableQuaternion(Quaternion.identity);
            currentHealth = 100f;
            maxHealth = 100f;
            inventory = new InventorySaveData();
            stats = new Dictionary<string, float>();
        }
    }

    [Serializable]
    public struct SerializableVector3
    {
        public float x;
        public float y;
        public float z;

        public SerializableVector3(Vector3 vector)
        {
            x = vector.x;
            y = vector.y;
            z = vector.z;
        }

        public Vector3 ToVector3()
        {
            return new Vector3(x, y, z);
        }
    }

    [Serializable]
    public struct SerializableQuaternion
    {
        public float x;
        public float y;
        public float z;
        public float w;

        public SerializableQuaternion(Quaternion quaternion)
        {
            x = quaternion.x;
            y = quaternion.y;
            z = quaternion.z;
            w = quaternion.w;
        }

        public Quaternion ToQuaternion()
        {
            return new Quaternion(x, y, z, w);
        }
    }


    [Serializable]
    public class InventorySaveData
    {
        public List<ItemSaveData> items;
        public Dictionary<string, int> currencies; // Different types of currencies

        public InventorySaveData()
        {
            items = new List<ItemSaveData>();
            currencies = new Dictionary<string, int>();
        }
    }

    [Serializable]
    public class ItemSaveData
    {
        public string itemId;
        public int quantity;
        public Dictionary<string, object> customData; // For item-specific data

        public ItemSaveData()
        {
            customData = new Dictionary<string, object>();
        }
    }

    [Serializable]
    public class DungeonSaveData
    {
        public int currentLevel;
        public int currentDepth;
        public List<RoomSaveData> rooms;
        public List<EnemySaveData> enemies;
        public Dictionary<Vector2Int, bool> exploredRooms; // Fog of war

        public DungeonSaveData()
        {
            rooms = new List<RoomSaveData>();
            enemies = new List<EnemySaveData>();
            exploredRooms = new Dictionary<Vector2Int, bool>();
        }
    }

    [Serializable]
    public class RoomSaveData
    {
        public Vector2Int position; // Grid position
        public string roomType;
        public bool isCleared;
        public List<ItemSaveData> groundItems;
        public Dictionary<string, bool> interactableStates; // Chests, doors, etc.

        public RoomSaveData()
        {
            groundItems = new List<ItemSaveData>();
            interactableStates = new Dictionary<string, bool>();
        }
    }

    [Serializable]
    public class EnemySaveData
    {
        public string enemyId;
        public Vector3 position;
        public Quaternion rotation;
        public float currentHealth;
        public string currentState; // AI state
        public Dictionary<string, object> customData; // For enemy-specific data

        public EnemySaveData()
        {
            position = Vector3.zero;
            rotation = Quaternion.identity;
            customData = new Dictionary<string, object>();
        }
    }

    [Serializable]
    public class GameStateSaveData
    {
        public float playTime;
        public int score;
        public List<string> unlockedAchievements;
        public Dictionary<string, object> customGameData; // For misc game data
        public Dictionary<string, bool> flags; // Game state flags

        public GameStateSaveData()
        {
            unlockedAchievements = new List<string>();
            flags = new Dictionary<string, bool>();
            customGameData = new Dictionary<string, object>();
        }
    }

    // Add to SaveData class:
    public class LevelTransitionData
    {
        public SpawnDirection direction; // Direction of transition
        public string lastSpawnPointId; // ID of spawn point used
        public string levelId; // Scene/level name
        public Vector3 playerPosition; // Exact position (for precise spawning)
        public Quaternion playerRotation; // Exact rotation
    }
}
