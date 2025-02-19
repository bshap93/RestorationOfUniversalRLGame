#if UNITY_EDITOR
using Gameplay.ItemsInteractions;
using Gameplay.SaveLoad;
using UnityEditor;
using UnityEngine;
using SaveSystem = PixelCrushers.SaveSystem;

namespace Project.Editor.Utilities
{
    public static class DebugMenu
    {
        [MenuItem("Debug/Clear All Save Data")]
        public static void ClearAllSaveData()
        {
            // Reset Pickables
            PickableManager.ResetPickedItems();

            // Reset Journal Recipes
            JournalPersistenceManager.ResetJournal();

            // Reset Dispenser States
            DispenserManager.ResetDispenserStates();

            // Reset Inventory System
            // InventoryPersistenceManager.Instance?.ResetInventory();

            // Reset Resources (if applicable)
            ResourcesPersistenceManager.Instance?.ResetResources();


            Debug.Log("Destuctable containers reset.");


            // Delete Dialogue System PlayerPrefs
            PlayerPrefs.DeleteAll();
            Debug.Log("Deleted Dialogue System PlayerPrefs.");

            SaveSystem.ClearSavedGameData();

            Debug.Log("All save data cleared successfully.");
        }
    }
}
#endif
