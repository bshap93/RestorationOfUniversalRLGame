﻿// GameInitiator.cs

using System.Threading.Tasks;
using Core.SaveSystem;
using MoreMountains.Tools;
using MoreMountains.TopDownEngine;
using Project.Core.SaveSystem;
using Project.Gameplay.Enemy;
using UnityEngine;

namespace Core.GameInitialization
{
    public class GameInitiator : MonoBehaviour, MMEventListener<MMCameraEvent>
    {
        public float enemySpawnRate;
        NewSaveManager _saveManager;

        void Awake()
        {
            // Find references in our prefab structure


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

        public void OnMMEvent(MMCameraEvent mmEvent)
        {
            if (mmEvent.EventType == MMCameraEventTypes.SetTargetCharacter)
            {
                // MMGameEvent.Trigger("SaveInventory");
                // MMGameEvent.Trigger("SaveResources");
                // MMGameEvent.Trigger("SaveJournal");
                SaveManager.Instance.SaveAll();
                ApplyCharacterCreationDataToPlayer(mmEvent.TargetCharacter.gameObject);

                SpawnEnemiesIfPossible(mmEvent.TargetCharacter.gameObject);
            }
        }
        public void OnMMEvent(TopDownEngineEvent engineEvent)
        {
            if (engineEvent.EventType == TopDownEngineEventTypes.CharacterSwitch)
                // Apply character creation data to the player
                Debug.Log("CharacterSwitch event received. ");
        }

        async Task InitializeCore()
        {
            var hasSave = SaveManager.Instance.LoadAll();

            var saveManager = FindFirstObjectByType<SaveStateManager>();
            if (saveManager == null)
                Debug.LogWarning("SaveStateManager not found in the scene.");
            else
                saveManager.IsSaveLoaded = hasSave;

            if (!hasSave)
                await StartNewGame();
            else
                Debug.Log("Valid save loaded.");
        }

        async Task<bool> LoadLastGame()
        {
            var saveLoaded = _saveManager.LoadGame();
            return await Task.FromResult(saveLoaded);
        }


        Task StartNewGame()
        {
            // Reset item placements 
            // PickableManager.ResetPickedItems();

            // Spawn the player
            var initialSpawnPoint = FindFirstObjectByType<SpawnPoint>();
            if (initialSpawnPoint == null) Debug.LogWarning("No CheckPoint found for initial spawn!");
            return Task.CompletedTask;
        }

        void ApplyCharacterCreationDataToPlayer(GameObject playerGameObject)
        {
            Debug.Log("Applying CharacterCreationData to PlayerStats.");
        }

        void SpawnEnemiesIfPossible(GameObject playerGameObject)
        {
            if (playerGameObject != null)
            {
                var enemySpawners = FindObjectsOfType<EnemySpawnPoint>();
                var randomPathGenerator = gameObject.AddComponent<RandomPathGenerator>();


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
