using System;
using Gameplay.ItemsInteractions;
using Gameplay.SaveLoad;
using UnityEngine;
using SaveSystem = PixelCrushers.SaveSystem;

[Serializable]
public class SaveManager : MonoBehaviour
{
    const string SaveFilePrefix = "GameSave_";
    const string SaveFileExtension = ".es3";

    [Header("Persistence Managers")] [SerializeField]
    InventoryPersistenceManager inventoryManager;
    [SerializeField] ResourcesPersistenceManager resourcesManager;
    [SerializeField] JournalPersistenceManager journalManager;

    [Header("Item & Container Persistence")]
    public PickableManager pickableManager;
    public DestructibleManager destructibleManager;

    int currentSlot = 1;

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
        resourcesManager?.SaveResources();
        journalManager?.SaveJournal();

        SaveSystem.SaveToSlotImmediate(0);
    }

    public bool LoadAll()
    {
        var inventoryLoaded = inventoryManager != null && inventoryManager.HasSavedData();
        var resourcesLoaded = resourcesManager != null && resourcesManager.HasSavedData();
        var journalLoaded = journalManager != null && journalManager.HasSavedData();

        if (inventoryLoaded) inventoryManager.LoadInventories();
        if (resourcesLoaded) resourcesManager.RevertResourcesToLastSave();
        if (journalLoaded) journalManager.RevertJournalToLastSave();

        // Load pickable items and destroyed containers
        pickableManager?.LoadPickedItems();
        destructibleManager?.LoadDestroyedObjects();

        SaveSystem.LoadFromSlot(0);

        return inventoryLoaded || resourcesLoaded || journalLoaded;
    }

    public void ResetAll()
    {
        Debug.Log("[SaveManager] Resetting all data...");
        inventoryManager?.ResetInventory();
        resourcesManager?.RevertResourcesToLastSave();
        journalManager?.RevertJournalToLastSave();

        PickableManager.ResetPickedItems();
        DestructibleManager.ResetDestroyedObjects();
    }
}
