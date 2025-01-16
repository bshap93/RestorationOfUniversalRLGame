using System;
using MoreMountains.Tools;
using Project.Gameplay.Player;
using Project.Gameplay.Player.Health;
using UnityEngine;

namespace Project.Gameplay.SaveLoad
{
    public class ResourcesPersistenceManager : MonoBehaviour, MMEventListener<MMGameEvent>
    {
        const string HealthFileName = "PlayerHealth.save";
        const string MaxHealthFileName = "PlayerMaxHealth.save";
        const string CurrentCurrencyFileName = "PlayerCurrency.save";
        const string XPFileName = "PlayerXP.save";
        const string LevelFileName = "PlayerLevel.save";
        const string XPForNextLevelFileName = "PlayerXPForNextLevel.save";

        const string SaveFolderName = "Player";

        [Header("Health")] [SerializeField] HealthAlt playerHealth;

        [Header("Player Stats")] [SerializeField]
        PlayerStats playerStats;

        public Action OnStatsUpdated;

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

        public void OnMMEvent(MMGameEvent @event)
        {
            if (@event.EventName == "SaveResources")
                SaveResources();
            else if (@event.EventName == "RevertResources")
                RevertResourcesToLastSave();
        }

        void SaveResources()
        {
            // Save Player Health
            SaveHealthState();

            // Save Player Stats (currency, XP, level, etc.)
            SavePlayerStats();

            Debug.Log("Resources saved.");
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
            MMSaveLoadManager.Save(playerStats.playerCurrency, CurrentCurrencyFileName, SaveFolderName);
            Debug.Log($"Player currency saved: {playerStats.playerCurrency}");

            // Save XPManager data
            var xpManager = playerStats.XpManager;
            if (xpManager != null)
            {
                MMSaveLoadManager.Save(xpManager.playerExperiencePoints, XPFileName, SaveFolderName);
                MMSaveLoadManager.Save(xpManager.playerCurrentLevel, LevelFileName, SaveFolderName);
                MMSaveLoadManager.Save(xpManager.playerXpForNextLevel, XPForNextLevelFileName, SaveFolderName);
                Debug.Log(
                    $"XP data saved: XP={xpManager.playerExperiencePoints}, Level={xpManager.playerCurrentLevel}, XPForNextLevel={xpManager.playerXpForNextLevel}");
            }
            else
            {
                Debug.LogWarning("XPManager is null. Cannot save XP data.");
            }
        }

        void RevertResourcesToLastSave()
        {
            // Revert Player Health
            RevertHealthToLastSave();

            // Revert Player Stats (currency, XP, level, etc.)
            RevertPlayerStats();

            Debug.Log("Resources reverted.");

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
            var loadedCurrency = MMSaveLoadManager.Load(typeof(int), CurrentCurrencyFileName, SaveFolderName);
            playerStats.playerCurrency = loadedCurrency != null ? (int)loadedCurrency : 0;
            Debug.Log($"Player currency reverted: {playerStats.playerCurrency}");

            // Load XPManager data
            var xpManager = playerStats.XpManager;
            if (xpManager != null)
            {
                var loadedXP = MMSaveLoadManager.Load(typeof(int), XPFileName, SaveFolderName);
                var loadedLevel = MMSaveLoadManager.Load(typeof(int), LevelFileName, SaveFolderName);
                var loadedXPForNextLevel = MMSaveLoadManager.Load(typeof(int), XPForNextLevelFileName, SaveFolderName);

                xpManager.playerExperiencePoints = loadedXP != null ? (int)loadedXP : 0;
                xpManager.playerCurrentLevel = loadedLevel != null ? (int)loadedLevel : 1;
                xpManager.playerXpForNextLevel = loadedXPForNextLevel != null ? (int)loadedXPForNextLevel : 20;

                Debug.Log(
                    $"XP data reverted: XP={xpManager.playerExperiencePoints}, Level={xpManager.playerCurrentLevel}, XPForNextLevel={xpManager.playerXpForNextLevel}");
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

            MMSaveLoadManager.Save(playerHealth.CurrentHealth, HealthFileName, SaveFolderName);
            MMSaveLoadManager.Save(playerHealth.MaximumHealth, MaxHealthFileName, SaveFolderName);

            Debug.Log(
                $"Health saved: CurrentHealth={playerHealth.CurrentHealth}, MaximumHealth={playerHealth.MaximumHealth}");
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

            var loadedHealth = MMSaveLoadManager.Load(typeof(float), HealthFileName, SaveFolderName);
            var loadedMaxHealth = MMSaveLoadManager.Load(typeof(float), MaxHealthFileName, SaveFolderName);

            playerHealth.SetHealth(loadedHealth != null ? (float)loadedHealth : playerHealth.MaximumHealth);
            playerHealth.SetMaximumHealth(
                loadedMaxHealth != null ? (float)loadedMaxHealth : playerHealth.MaximumHealth);

            Debug.Log(
                $"Health reverted: CurrentHealth={playerHealth.CurrentHealth}, MaximumHealth={playerHealth.MaximumHealth}");
        }
    }
}
