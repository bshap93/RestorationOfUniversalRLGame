using System;
using Gameplay.ItemsInteractions;
using Gameplay.Player.Stats;
using Gameplay.SaveLoad;
using UnityEngine;
using UnityEngine.Serialization;
using SaveSystem = PixelCrushers.SaveSystem;

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


        SaveSystem.SaveToSlotImmediate(0);

        SaveSystem.SaveToSlot(currentSlot);
    }

    public bool LoadAll()
    {
        var inventoryLoaded = inventoryManager != null && inventoryManager.HasSavedData();
        var staminaLoaded = playerStaminaManager != null && playerStaminaManager.HasSavedData();
        var journalLoaded = craftingRecipeManager != null && craftingRecipeManager.HasSavedData();

        if (inventoryLoaded) inventoryManager.LoadInventories();
        if (staminaLoaded) playerStaminaManager.LoadPlayerStamina();
        if (journalLoaded) CraftingRecipeManager.ResetLearnedCraftingGroups();

        // Load pickable items and destroyed containers
        pickableManager?.LoadPickedItems();
        destructibleManager?.LoadDestroyedObjects();

        SaveSystem.LoadFromSlot(0);

        return inventoryLoaded || staminaLoaded || journalLoaded;
    }
}
