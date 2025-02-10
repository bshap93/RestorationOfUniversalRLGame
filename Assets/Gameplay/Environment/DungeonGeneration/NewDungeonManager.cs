using System;
using System.Threading.Tasks;
using DunGen;
using Project.Gameplay.DungeonGeneration.Generators;
using Project.Gameplay.DungeonGeneration.Spawning;
using UnityEngine;
using UnityEngine.Serialization;

// DunGen's namespace

namespace Project.Gameplay.DungeonGeneration
{
    public class NewDungeonManager : MonoBehaviour
    {
        [SerializeField] bool isTestMode;
        [FormerlySerializedAs("_runtimeDungeon")] [SerializeField]
        RuntimeDungeon runtimeDungeon;
        [SerializeField] GameObject playerPrefab; // Assign your player prefab in inspector
        [SerializeField] GameObject playerSpawnPoint;

        TaskCompletionSource<bool> _generationTaskSource;
        GameObject _playerInstance;
        SpawnPointManager _spawnPointManager;


        void Awake()
        {
            _spawnPointManager = gameObject.GetComponent<SpawnPointManager>();
            if (_spawnPointManager == null) _spawnPointManager = gameObject.AddComponent<SpawnPointManager>();
            // runtimeDungeon = gameObject.GetComponent<RuntimeDungeon>();
            if (!isTestMode)
            {
                if (runtimeDungeon == null)
                {
                    runtimeDungeon = gameObject.AddComponent<RuntimeDungeon>();
                    Debug.LogWarning("DungeonFlow asset needs to be assigned to RuntimeDungeon!");
                }

                // Subscribe to DunGen's generation complete event
                runtimeDungeon.Generator.OnGenerationComplete += OnDungeonGenerationComplete;
            }
        }

        void OnDestroy()
        {
            if (!isTestMode && runtimeDungeon != null && runtimeDungeon.Generator != null)
                runtimeDungeon.Generator.OnGenerationComplete -= OnDungeonGenerationComplete;
        }

        void OnDungeonGenerationComplete(DungeonGenerator generator)
        {
            _generationTaskSource?.TrySetResult(true);
        }


        public async Task GenerateNewDungeon(int seed)
        {
            if (isTestMode)
            {
                // In test mode, just let LevelManager handle the spawn
                Debug.Log("Test mode - skipping dungeon generation");
                return;
            }

            try
            {
                _generationTaskSource = new TaskCompletionSource<bool>();

                runtimeDungeon.Generator.Seed = seed;
                runtimeDungeon.Generate();

                // Wait for generation complete event
                await _generationTaskSource.Task;
            }
            catch (Exception e)
            {
                Debug.LogError($"Failed to generate dungeon: {e.Message}");
                throw;
            }
        }

        public void LoadDungeon(DungeonData data)
        {
            // TODO: Implement using DunGen's systems
            throw new NotImplementedException();
        }
    }
}
