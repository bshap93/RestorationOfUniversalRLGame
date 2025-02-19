using Gameplay.ItemsInteractions;
using Gameplay.SaveLoad;
using PixelCrushers;
using UnityEngine;

public class PurePrototypeReset : MonoBehaviour
{
    void Awake()
    {
        Debug.Log("PurePrototypeReset: Awake() called.");
        ClearAllSaveData();
    }
    void OnApplicationQuit()
    {
        Debug.Log("PurePrototypeReset: OnApplicationQuit() called.");
        ClearAllSaveData();
    }

    public static void ClearAllSaveData()
    {
        // Reset Pickables
        PickableManager.ResetPickedItems();

        // Reset Journal Recipes
        JournalPersistenceManager.ResetJournal();

        // Reset Dispenser States
        DispenserManager.ResetDispenserStates();

        // Reset Inventory System
        InventoryPersistenceManager.Instance?.ResetInventory();

        // Reset Resources (if applicable)
        ResourcesPersistenceManager.Instance?.ResetResources();

        DestructibleManager.ResetDestroyedObjects();

        Debug.Log("Destuctable containers reset.");


        // Delete Dialogue System PlayerPrefs
        PlayerPrefs.DeleteAll();
        Debug.Log("Deleted Dialogue System PlayerPrefs.");

        SaveSystem.ClearSavedGameData();

        Debug.Log("All save data cleared successfully.");
    }
}
