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
            ES3.Save(CurrentCurrencyFileName, playerStats.playerCurrency);

            // Save XPManager data
            var xpManager = playerStats.XpManager;
            if (xpManager != null)
            {
                ES3.Save(XPFileName, xpManager.playerExperiencePoints);
                ES3.Save(LevelFileName, xpManager.playerCurrentLevel);
                ES3.Save(XPForNextLevelFileName, xpManager.playerXpForNextLevel);
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
            if (ES3.KeyExists(CurrentCurrencyFileName))
            {
                playerStats.playerCurrency = ES3.Load<int>(CurrentCurrencyFileName);
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
                xpManager.playerExperiencePoints = ES3.KeyExists(XPFileName) ? ES3.Load<int>(XPFileName) : 0;
                xpManager.playerCurrentLevel = ES3.KeyExists(LevelFileName) ? ES3.Load<int>(LevelFileName) : 1;
                xpManager.playerXpForNextLevel =
                    ES3.KeyExists(XPForNextLevelFileName) ? ES3.Load<int>(XPForNextLevelFileName) : 20;
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

            ES3.Save(HealthFileName, playerHealth.CurrentHealth);
            ES3.Save(MaxHealthFileName, playerHealth.MaximumHealth);

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

            if (ES3.KeyExists(HealthFileName))
            {
                playerHealth.SetHealth(ES3.Load<float>(HealthFileName));
            }
            else
            {
                playerHealth.SetHealth(playerHealth.MaximumHealth);
                Debug.LogWarning("No save data for health. Defaulting to maximum health.");
            }

            if (ES3.KeyExists(MaxHealthFileName)) playerHealth.SetMaximumHealth(ES3.Load<float>(MaxHealthFileName));
        }

        public bool HasSavedData()
        {
            return ES3.KeyExists(HealthFileName) ||
                   ES3.KeyExists(MaxHealthFileName) ||
                   ES3.KeyExists(CurrentCurrencyFileName) ||
                   ES3.KeyExists(XPFileName) ||
                   ES3.KeyExists(LevelFileName) ||
                   ES3.KeyExists(XPForNextLevelFileName);
        }
    }
}
