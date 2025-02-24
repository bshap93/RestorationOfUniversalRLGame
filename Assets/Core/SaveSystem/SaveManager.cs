using System;
using Gameplay.ItemsInteractions;
using Gameplay.Player.Stats;
using Gameplay.SaveLoad;
using UnityEngine;
using UnityEngine.Serialization;

namespace Core.SaveSystem
{
    [Serializable]
    public class SaveManager : MonoBehaviour
    {
        const string SaveFilePrefix = "GameSave_";
        const string SaveFileExtension = ".es3";

        [Header("Persistence Managers")] [SerializeField]
        InventoryPersistenceManager inventoryManager;
        [FormerlySerializedAs("playerMutableStatsManager")]
        [FormerlySerializedAs("playerStatsManager")]
        [FormerlySerializedAs("resourcesManager")]
        [SerializeField]
        PlayerStaminaManager playerStaminaManager;
        [SerializeField] PlayerHealthManager playerHealthManager;
        [FormerlySerializedAs("journalManager")] [SerializeField]
        CraftingRecipeManager craftingRecipeManager;

        [Header("Item & Container Persistence")]
        public PickableManager pickableManager;
        public DestructibleManager destructibleManager;

        public int currentSlot;

        public static SaveManager Instance { get; private set; }

        void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            DontDestroyOnLoad(gameObject);

            // Initialize managers if needed
            if (pickableManager == null)
            {
                pickableManager = GetComponentInChildren<PickableManager>(true);
                if (pickableManager == null)
                {
                    var pickableGO = new GameObject("PickableManager");
                    pickableManager = pickableGO.AddComponent<PickableManager>();
                    pickableGO.transform.SetParent(transform);
                }
            }

            if (destructibleManager == null)
            {
                destructibleManager = GetComponentInChildren<DestructibleManager>(true);
                if (destructibleManager == null)
                {
                    var destructableGO = new GameObject("DestructableManager");
                    destructibleManager = destructableGO.AddComponent<DestructibleManager>();
                    destructableGO.transform.SetParent(transform);
                }
            }

            if (playerStaminaManager == null)
            {
                playerStaminaManager = GetComponentInChildren<PlayerStaminaManager>(true);
                if (playerStaminaManager == null) Debug.LogError("PlayerStaminaManager not found in SaveManager");
            }

            if (playerHealthManager == null)
            {
                playerHealthManager = GetComponentInChildren<PlayerHealthManager>(true);
                if (playerHealthManager == null) Debug.LogError("PlayerHealthManager not found in SaveManager");
            }
        }

        string GetSaveFileName(int slot)
        {
            return $"{SaveFilePrefix}{slot}{SaveFileExtension}";
        }

        public void SaveAll()
        {
            inventoryManager?.SaveInventories();
            PlayerStaminaManager.SavePlayerStamina();
            // craftingRecipeManager?.SaveJournal();


            PixelCrushers.SaveSystem.SaveToSlotImmediate(0);

            PixelCrushers.SaveSystem.SaveToSlot(currentSlot);
        }

        public bool LoadAll()
        {
            var inventoryLoaded = inventoryManager != null && inventoryManager.HasSavedData();
            var staminaLoaded = playerStaminaManager != null && playerStaminaManager.HasSavedData();
            var healthLoaded = playerHealthManager != null && playerHealthManager.HasSavedData();
            var journalLoaded = craftingRecipeManager != null && craftingRecipeManager.HasSavedData();

            if (inventoryLoaded) inventoryManager.LoadInventories();
            if (staminaLoaded) playerStaminaManager.LoadPlayerStamina();
            if (journalLoaded) CraftingRecipeManager.ResetLearnedCraftingGroups();

            // Load pickable items and destroyed containers
            pickableManager?.LoadPickedItems();
            destructibleManager?.LoadDestroyedObjects();

            PixelCrushers.SaveSystem.LoadFromSlot(0);

            return inventoryLoaded || staminaLoaded || journalLoaded;
        }
    }
}
