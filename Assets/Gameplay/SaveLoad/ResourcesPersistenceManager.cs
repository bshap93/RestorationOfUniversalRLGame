using System;
using Gameplay.Player;
using MoreMountains.Tools;
using Project.Gameplay.Player.Health;
using UnityEngine;

namespace Gameplay.SaveLoad
{
    public class ResourcesPersistenceManager : MonoBehaviour, MMEventListener<MMGameEvent>
    {
        // const string HealthFileName = "PlayerHealth.save";
        // const string MaxHealthFileName = "PlayerMaxHealth.save";
        // const string CurrentCurrencyFileName = "PlayerCurrency.save";
        // const string XPFileName = "PlayerXP.save";
        // const string LevelFileName = "PlayerLevel.save";
        // const string XPForNextLevelFileName = "PlayerXPForNextLevel.save";


        const string SaveFolderName = "Player";
        public static ResourcesPersistenceManager Instance;

        [Header("Health")] [SerializeField] HealthAlt playerHealth;

        [Header("Player Stats")] [SerializeField]
        PlayerStats playerStats;

        public Action OnStatsUpdated;

        void Awake()
        {
            if (Instance == null)
                Instance = this;
            else
                Destroy(gameObject);
        }

        void OnEnable()
        {
            // Subscribe to global save/load events
            this.MMEventStartListening();
        }


        void OnDisable()
        {
            // Unsubscribe to prevent leaks
            this.MMEventStopListening();
        }

        public void OnMMEvent(MMGameEvent mmEvent)
        {
            if (mmEvent.EventName == "SaveResources")
                SaveResources();
            else if (mmEvent.EventName == "RevertResources")
                RevertResourcesToLastSave();
        }

        public void SaveResources()
        {
            // Save Player Health
            SaveHealthState();

            // Save Player Stats (currency, XP, level, etc.)
            SavePlayerStats();
        }

        void SavePlayerStats()
        {
            if (playerStats == null)
            {
                playerStats = FindObjectOfType<PlayerStats>();
                if (playerStats == null)
                {
                    Debug.LogWarning("Player Stats is null. Cannot save player stats.");
                    return;
                }
            }

            // Save player currency
            ES3.Save(GetSaveFilePathCurrency(), playerStats.playerCurrency);

            // Save XPManager data
            var xpManager = playerStats.XpManager;
            if (xpManager != null)
            {
                ES3.Save(GetSaveFilePathXP(), xpManager.playerExperiencePoints);
                ES3.Save(GetSaveFilePathLevel(), xpManager.playerCurrentLevel);
                ES3.Save(GetSaveFilePathXPForNextLevel(), xpManager.playerXpForNextLevel);
            }
            else
            {
                Debug.LogWarning("XPManager is null. Cannot save XP data.");
            }
        }


        public void RevertResourcesToLastSave()
        {
            // Revert Player Health
            RevertHealthToLastSave();

            // Revert Player Stats (currency, XP, level, etc.)
            RevertPlayerStats();


            UpdateUI();
        }

        void UpdateUI()
        {
            NotifyUIOfUpdatedStats();
        }
        void RevertPlayerStats()
        {
            if (playerStats == null)
            {
                playerStats = FindObjectOfType<PlayerStats>();
                if (playerStats == null)
                {
                    Debug.LogWarning("Player Stats is null. Cannot revert player stats.");
                    return;
                }
            }

            // Load player currency
            if (ES3.KeyExists(GetSaveFilePathCurrency()))
            {
                playerStats.playerCurrency = ES3.Load<int>(GetSaveFilePathCurrency());
            }
            else
            {
                playerStats.playerCurrency = 0;
                Debug.LogWarning("No save data for player currency. Defaulting to 0.");
            }

            // Load XPManager data
            var xpManager = playerStats.XpManager;
            if (xpManager != null)
            {
                xpManager.playerExperiencePoints =
                    ES3.KeyExists(GetSaveFilePathXP()) ? ES3.Load<int>(GetSaveFilePathXP()) : 0;

                xpManager.playerCurrentLevel =
                    ES3.KeyExists(GetSaveFilePathLevel()) ? ES3.Load<int>(GetSaveFilePathLevel()) : 1;

                xpManager.playerXpForNextLevel =
                    ES3.KeyExists(GetSaveFilePathXPForNextLevel())
                        ? ES3.Load<int>(GetSaveFilePathXPForNextLevel())
                        : 20;
            }
            else
            {
                Debug.LogWarning("XPManager is null. Cannot revert XP data.");
            }
        }


        void NotifyUIOfUpdatedStats()
        {
            OnStatsUpdated?.Invoke();
        }


        void SaveHealthState()
        {
            if (playerHealth == null)
            {
                playerHealth = FindObjectOfType<HealthAlt>();
                if (playerHealth == null)
                {
                    Debug.LogWarning("Player Health is null. Cannot save health state.");
                    return;
                }
            }

            ES3.Save(GetSaveFilePathHealth(), playerHealth.CurrentHealth);
            ES3.Save(GetSaveFilePathMaxHealth(), playerHealth.MaximumHealth);
        }


        void RevertHealthToLastSave()
        {
            if (playerHealth == null)
            {
                playerHealth = FindObjectOfType<HealthAlt>();
                if (playerHealth == null)
                {
                    Debug.LogWarning("Player Health is null. Cannot revert health state.");
                    return;
                }
            }

            if (ES3.KeyExists(GetSaveFilePathHealth()))
            {
                playerHealth.SetHealth(ES3.Load<float>(GetSaveFilePathHealth()));
            }
            else
            {
                playerHealth.SetHealth(playerHealth.MaximumHealth);
                Debug.LogWarning("No save data for health. Defaulting to maximum health.");
            }

            if (ES3.KeyExists(GetSaveFilePathMaxHealth()))
                playerHealth.SetMaximumHealth(ES3.Load<float>(GetSaveFilePathMaxHealth()));
        }

        public bool HasSavedData()
        {
            return ES3.KeyExists(GetSaveFilePathHealth()) ||
                   ES3.KeyExists(GetSaveFilePathMaxHealth()) ||
                   ES3.KeyExists(GetSaveFilePathCurrency()) ||
                   ES3.KeyExists(GetSaveFilePathXP()) ||
                   ES3.KeyExists(GetSaveFilePathLevel()) ||
                   ES3.KeyExists(GetSaveFilePathXPForNextLevel());
        }

        static string GetSaveFilePathHealth()
        {
            return GetSaveFilePath("PlayerHealth.save");
        }
        static string GetSaveFilePathMaxHealth()
        {
            return GetSaveFilePath("PlayerMaxHealth.save");
        }
        static string GetSaveFilePathCurrency()
        {
            return GetSaveFilePath("PlayerCurrency.save");
        }
        static string GetSaveFilePathXP()
        {
            return GetSaveFilePath("PlayerXP.save");
        }
        static string GetSaveFilePathLevel()
        {
            return GetSaveFilePath("PlayerLevel.save");
        }
        static string GetSaveFilePathXPForNextLevel()
        {
            return GetSaveFilePath("PlayerXPForNextLevel.save");
        }

        static string GetSaveFilePath(string fileName)
        {
            var slotPath = ES3SlotManager.selectedSlotPath;
            return string.IsNullOrEmpty(slotPath) ? fileName : $"{slotPath}/{fileName}";
        }

        public void ResetResources()
        {
            Debug.Log("[ResourcesPersistenceManager] Resetting all resources to default values...");

            // Delete all save files
            ES3.DeleteFile(GetSaveFilePathHealth());
            ES3.DeleteFile(GetSaveFilePathMaxHealth());
            ES3.DeleteFile(GetSaveFilePathCurrency());
            ES3.DeleteFile(GetSaveFilePathXP());
            ES3.DeleteFile(GetSaveFilePathLevel());
            ES3.DeleteFile(GetSaveFilePathXPForNextLevel());

            // Reset health to max
            if (playerHealth != null) playerHealth.SetHealth(playerHealth.MaximumHealth);

            // Reset player stats to initial state
            if (playerStats != null)
            {
                playerStats.playerCurrency = 0;
                if (playerStats.XpManager != null)
                {
                    playerStats.XpManager.playerExperiencePoints = 0;
                    playerStats.XpManager.playerCurrentLevel = 1;
                    playerStats.XpManager.playerXpForNextLevel = 20;
                }
            }

            NotifyUIOfUpdatedStats();
            Debug.Log("Resources reset successfully.");
        }
    }
}
