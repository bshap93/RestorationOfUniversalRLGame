// GameInitiator.cs

using System.Threading.Tasks;
using DunGen;
using MoreMountains.Tools;
using MoreMountains.TopDownEngine;
using Project.Core.SaveSystem;
using Project.Gameplay.DungeonGeneration;
using Project.Gameplay.Enemy;
using Project.Gameplay.Player;
using TopDownEngine.Common.Scripts.Spawn;
using UnityEngine;

namespace Project.Core.GameInitialization
{
    public class GameInitiator : MonoBehaviour, MMEventListener<MMCameraEvent>
    {

        public float enemySpawnRate;
        RuntimeDungeon _runtimeDungeon;
        NewSaveManager _saveManager;
        NewDungeonManager _dungeonManager;

        void Awake()
        {


            // Find references in our prefab structure
            _dungeonManager = GetComponentInChildren<NewDungeonManager>();
            _runtimeDungeon = GetComponentInChildren<RuntimeDungeon>();

            if (_dungeonManager == null || _runtimeDungeon == null)
                Debug.Log("No dungeon was generated for this scene");

            // Check if NewSaveManager is already in the scene
            _saveManager = NewSaveManager.Instance;
            if (_saveManager == null) _saveManager = gameObject.AddComponent<NewSaveManager>();
        }


        async void Start()
        {
            await InitializeCore();
        }

        void OnEnable()
        {
            // Listen for CharacterSwitch events
            this.MMEventStartListening();
        }

        void OnDisable()
        {
            this.MMEventStopListening();
        }

        public void OnMMEvent(MMCameraEvent eventType)
        {
            if (eventType.EventType == MMCameraEventTypes.SetTargetCharacter)
            {
                MMGameEvent.Trigger("SaveInventory");
                ApplyCharacterCreationDataToPlayer(eventType.TargetCharacter.gameObject);

                SpawnEnemiesIfPossible(eventType.TargetCharacter.gameObject);
            }
        }
        public void OnMMEvent(TopDownEngineEvent engineEvent)
        {
            if (engineEvent.EventType == TopDownEngineEventTypes.CharacterSwitch)
            {
                // Apply character creation data to the player
                NewSaveManager.Instance.ApplyCharacterCreationDataToPlayer();
                Debug.Log("CharacterSwitch event received. Applied CharacterCreationData to PlayerStats.");
            }
        }

        async Task InitializeCore()
        {
            var hasSave = await LoadLastGame(); // Attempt to load the last save
            var saveManager = FindObjectOfType<SaveStateManager>();

            if (saveManager == null)
                Debug.LogWarning("SaveStateManager not found in the scene.");
            else
                saveManager.IsSaveLoaded = hasSave;
            // if (SaveStateManager.Instance == null)
            //     Debug.LogWarning("SaveStateManager not found in the scene.");
            // else
            //     SaveStateManager.Instance.IsSaveLoaded = hasSave;

            if (!hasSave)
            {
                Debug.Log("No valid save found. Starting a new game.");
                await StartNewGame();
            }
            else
            {
                Debug.Log("Valid save loaded.");
            }
        }

        async Task<bool> LoadLastGame()
        {
            var saveLoaded = _saveManager.LoadGame();
            return await Task.FromResult(saveLoaded);
        }


        async Task StartNewGame()
        {
            var seed = Random.Range(0, int.MaxValue);
            await _dungeonManager.GenerateNewDungeon(seed);

            // Spawn the player
            var initialSpawnPoint = FindObjectOfType<CheckPoint>();
            if (initialSpawnPoint == null) Debug.LogError("No CheckPoint found for initial spawn!");
        }

        void ApplyCharacterCreationDataToPlayer(GameObject playerGameObject)
        {
            if (playerGameObject != null)
            {
                var playerStats = playerGameObject.GetComponent<PlayerStats>();
                if (playerStats != null)
                    playerStats.Initialize(NewSaveManager.Instance.CurrentSave.characterCreationData);
                else
                    Debug.LogError("PlayerStats component not found on Player GameObject.");
            }
            else
            {
                Debug.LogError("Player GameObject not found.");
            }
        }

        void SpawnEnemiesIfPossible(GameObject playerGameObject)
        {
            if (playerGameObject != null)
            {
                var enemySpawners = FindObjectsOfType<EnemySpawnPoint>();
                var randomPathGenerator = gameObject.AddComponent<RandomPathGenerator>();

                Debug.Log("Spawning enemies...");

                foreach (var spawner in enemySpawners)
                {
                    // Return early at the rate of the  EnemySpawnRate randomly
                    if (Random.Range(0f, 1f) > enemySpawnRate) continue;


                    var enemyClass = spawner.GetComponent<EnemySpawnPoint>().EnemyClass;
                    var enemyPrefab = enemyClass.GetRandomEnemyPrefab();

                    // Spawn the enemy
                    Instantiate(enemyPrefab, spawner.transform.position, Quaternion.identity);

                    Debug.Log("Enemy spawned.");
                }
            }
        }
    }
}
