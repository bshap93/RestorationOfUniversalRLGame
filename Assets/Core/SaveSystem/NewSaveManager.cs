using System;
using System.Collections.Generic;
using MoreMountains.Tools;
using Project.Gameplay.DungeonGeneration.Spawning;
using UnityEngine;

namespace Project.Core.SaveSystem
{
// Helper class to implement the ISaveable interface


// Save Manager to handle saving/loading
    public class NewSaveManager : MonoBehaviour
    {
        const string SaveFileName = "GameSave.save";
        const string SaveFolderName = "MyGameSaves";
        // Track level transitions for save/load
        readonly Dictionary<string, LevelTransitionData> levelTransitions = new();

        public static NewSaveManager Instance { get; private set; }
        public SaveData CurrentSave { get; private set; }

        void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                CurrentSave = new SaveData();
            }
            else
            {
                Destroy(gameObject);
            }
        }


        public void SetLastTransitionPoint(string levelId, string spawnPointId, SpawnDirection direction)
        {
            var playerGameObject = GameObject.FindGameObjectWithTag("Player");
            levelTransitions[levelId] = new LevelTransitionData
            {
                levelId = levelId,
                lastSpawnPointId = spawnPointId,
                direction = direction,
                playerPosition = playerGameObject.transform.position,
                playerRotation = playerGameObject.transform.rotation
            };
        }

        public void SaveGame(string slot = "default")
        {
            try
            {
                var playerGameObject = GameObject.FindGameObjectWithTag("Player");
                if (playerGameObject != null)
                {
                    // Update player data in the save file
                    CurrentSave.playerData.position = new SerializableVector3(playerGameObject.transform.position);
                    CurrentSave.playerData.rotation = new SerializableQuaternion(playerGameObject.transform.rotation);
                }

                // Use MMSaveLoadManager to save the current save data
                MMSaveLoadManager.Save(CurrentSave, SaveFileName, SaveFolderName);
                Debug.Log("Game saved successfully.");
            }
            catch (Exception e)
            {
                Debug.LogError($"Error saving game: {e.Message}");
            }
        }


        public bool LoadGame(string slot = "default")
        {
            try
            {
                // Load the save data
                var loadedData = (SaveData)MMSaveLoadManager.Load(
                    typeof(SaveData),
                    "GameSave.save",
                    "MyGameSaves"
                );

                if (loadedData != null)
                {
                    CurrentSave = loadedData;

                    // Apply loaded data to the player
                    var playerGameObject = GameObject.FindGameObjectWithTag("Player");
                    if (playerGameObject != null)
                    {
                        playerGameObject.transform.position = CurrentSave.playerData.position.ToVector3();
                        playerGameObject.transform.rotation = CurrentSave.playerData.rotation.ToQuaternion();
                    }

                    return true;
                }

                Debug.LogWarning("No save file found. Starting a new game.");
                CurrentSave = new SaveData(); // Initialize with default save data
                return false;
            }
            catch (Exception e)
            {
                Debug.LogError($"Error loading game: {e.Message}");
                return false;
            }
        }


        public void ClearCurrentSave()
        {
            CurrentSave = new SaveData(); // Reset save data to default
            Debug.Log("Current save cleared.");
        }
    }
}
