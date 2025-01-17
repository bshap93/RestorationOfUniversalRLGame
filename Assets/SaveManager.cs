using System;
using Project.Gameplay.SaveLoad;
using UnityEngine;

[Serializable]
public class SaveManager : MonoBehaviour
{
    const string SaveFilePrefix = "GameSave_"; // File prefix for save slots
    const string SaveFileExtension = ".es3"; // File extension

    [Header("Persistence Managers")] [SerializeField]
    InventoryPersistenceManager inventoryManager;
    [SerializeField] ResourcesPersistenceManager resourcesManager;
    [SerializeField] JournalPersistenceManager journalManager;
    int currentSlot = 1; // Default slot
    
    void Start()
    {
        inventoryManager = GetComponent<InventoryPersistenceManager>();
        resourcesManager = GetComponent<ResourcesPersistenceManager>();
        journalManager = GetComponent<JournalPersistenceManager>();
    }

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
    }

    string GetSaveFileName(int slot)
    {
        return $"{SaveFilePrefix}{slot}{SaveFileExtension}";
    }

    public void SaveAll()
    {
        Debug.Log("[SaveManager] Saving all data...");
        inventoryManager?.SaveInventories();
        resourcesManager?.SaveResources();
        journalManager?.SaveJournal();
    }

    public bool LoadAll()
    {
        Debug.Log("[SaveManager] Loading all data...");

        var inventoryLoaded = inventoryManager != null && inventoryManager.HasSavedData();
        var resourcesLoaded = resourcesManager != null && resourcesManager.HasSavedData();
        var journalLoaded = journalManager != null && journalManager.HasSavedData();

        if (inventoryLoaded) inventoryManager.RevertInventoriesToLastSave();
        if (resourcesLoaded) resourcesManager.RevertResourcesToLastSave();
        if (journalLoaded) journalManager.RevertJournalToLastSave();

        var hasSave = inventoryLoaded || resourcesLoaded || journalLoaded;
        Debug.Log(
            hasSave
                ? "[SaveManager] Save data loaded successfully."
                : "[SaveManager] No save data found.");

        return hasSave;
    }


    public void ResetAll()
    {
        Debug.Log("[SaveManager] Resetting all data...");
        inventoryManager?.RevertInventoriesToLastSave(); // Clear inventories
        resourcesManager?.RevertResourcesToLastSave(); // Reset health, currency, etc.
        journalManager?.RevertJournalToLastSave(); // Clear journal entries
    }
}
