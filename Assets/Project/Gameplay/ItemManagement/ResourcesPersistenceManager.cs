using MoreMountains.Tools;
using Project.Gameplay.Player;
using Project.Gameplay.Player.Health;
using UnityEngine;
using UnityEngine.Serialization;

namespace Project.Gameplay.ItemManagement
{
    public class ResourcesPersistenceManager : MonoBehaviour, MMEventListener<MMGameEvent>
    {
        const string HealthFileName = "PlayerHealth.save";
        const string MaxHealthFileName = "PlayerMaxHealth.save";
        const string CurrentCurrencyFileName = "PlayerCurrency.save";


        const string SaveFolderName = "Player";
        [FormerlySerializedAs("_playerHealth")] [Header("Health")] [SerializeField]
        HealthAlt playerHealth;

        [Header("Player Stats")] [SerializeField]
        PlayerStats playerStats;


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


        public void OnMMEvent(MMGameEvent eventType)
        {
            if (eventType.EventName == "SaveResources")
                SaveResources();
            else if (eventType.EventName == "RevertResources") RevertResourcesToLastSave();
        }

        void SaveResources()
        {
            // Save Player Health
            SaveHealthState();

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

            MMSaveLoadManager.Save(playerStats.playerCurrency, CurrentCurrencyFileName, SaveFolderName);

            Debug.Log($"Player currency saved: {playerStats.playerCurrency}");
        }


        void RevertResourcesToLastSave()
        {
            // Revert Player Health
            RevertHealthToLastSave();

            RevertPlayerStats();


            Debug.Log("Resources reverted.");
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

            // Load current currency
            var loadedCurrency = MMSaveLoadManager.Load(typeof(int), CurrentCurrencyFileName, SaveFolderName);
            var savedCurrency = loadedCurrency != null ? (int)loadedCurrency : 0;

            // Apply loaded currency value
            playerStats.playerCurrency = savedCurrency;

            Debug.Log($"Player currency reverted: {savedCurrency}");
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

            // Save primitive health values instead of the class
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

            // Load current health
            var loadedHealth = MMSaveLoadManager.Load(typeof(float), HealthFileName, SaveFolderName);
            var savedHealth = loadedHealth != null ? (float)loadedHealth : playerHealth.MaximumHealth;

            // Load maximum health
            var loadedMaxHealth = MMSaveLoadManager.Load(typeof(float), MaxHealthFileName, SaveFolderName);
            var savedMaxHealth = loadedMaxHealth != null ? (float)loadedMaxHealth : playerHealth.MaximumHealth;

            // Apply loaded health values
            playerHealth.SetHealth(savedHealth);
            playerHealth.SetMaximumHealth(savedMaxHealth);

            Debug.Log($"Health reverted: CurrentHealth={savedHealth}, MaximumHealth={savedMaxHealth}");
        }
    }
}
