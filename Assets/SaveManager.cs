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
    public DestructableManager destructableManager;

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

        if (destructableManager == null)
        {
            destructableManager = GetComponentInChildren<DestructableManager>(true);
            if (destructableManager == null)
            {
                var destructableGO = new GameObject("DestructableManager");
                destructableManager = destructableGO.AddComponent<DestructableManager>();
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

        if (inventoryLoaded) inventoryManager.RevertInventoriesToLastSave();
        if (resourcesLoaded) resourcesManager.RevertResourcesToLastSave();
        if (journalLoaded) journalManager.RevertJournalToLastSave();

        // Load pickable items and destroyed containers
        pickableManager?.LoadPickedItems();
        destructableManager?.LoadDestroyedContainers();

        SaveSystem.LoadFromSlot(0);

        return inventoryLoaded || resourcesLoaded || journalLoaded;
    }

    public void ResetAll()
    {
        Debug.Log("[SaveManager] Resetting all data...");
        inventoryManager?.RevertInventoriesToLastSave();
        resourcesManager?.RevertResourcesToLastSave();
        journalManager?.RevertJournalToLastSave();

        PickableManager.ResetPickedItems();
        DestructableManager.ResetDestroyedContainers();
    }
}
