using MoreMountains.Tools;
using Project.Gameplay.Player.Health;
using UnityEngine;
using UnityEngine.Serialization;

namespace Project.Gameplay.ItemManagement
{
    public class ResourcesPersistenceManager : MonoBehaviour, MMEventListener<MMGameEvent>
    {
        const string HealthFileName = "PlayerHealth";
        const string MaxHealthFileName = "PlayerMaxHealth";

        const string SaveFolderName = "Player";
        [FormerlySerializedAs("_playerHealth")] [Header("Health")] [SerializeField]
        HealthAlt playerHealth;

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

            Debug.Log("Resources saved.");
        }

        void RevertResourcesToLastSave()
        {
            // Revert Player Health
            RevertHealthToLastSave();

            Debug.Log("Resources reverted.");
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
            var loadedHealth = MMSaveLoadManager.Load(typeof(float), "PlayerHealth.save", "PlayerData");
            var savedHealth = loadedHealth != null ? (float)loadedHealth : playerHealth.MaximumHealth;

            // Load maximum health
            var loadedMaxHealth = MMSaveLoadManager.Load(typeof(float), "PlayerMaxHealth.save", "PlayerData");
            var savedMaxHealth = loadedMaxHealth != null ? (float)loadedMaxHealth : playerHealth.MaximumHealth;

            // Apply loaded health values
            playerHealth.SetHealth(savedHealth);
            playerHealth.SetMaximumHealth(savedMaxHealth);

            Debug.Log($"Health reverted: CurrentHealth={savedHealth}, MaximumHealth={savedMaxHealth}");
        }
    }
}
