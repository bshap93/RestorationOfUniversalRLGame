#if UNITY_EDITOR
using System.IO;
using Gameplay.SaveLoad;
using PixelCrushers.Wrappers;
using UnityEditor;
using UnityEngine;
using SaveSystem = PixelCrushers.SaveSystem;

namespace Project.Editor.Utilities
{
    public static class DebugMenu
    {
        const string QuestMachineSavePath = "Assets/MMData/PixelCrushers/QuestMachineURP.data";

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
            if (InventoryPersistenceManager.Instance != null) InventoryPersistenceManager.Instance.ResetInventory();

            // Reset Resources (if applicable)
            ResourcesPersistenceManager.Instance?.ResetResources();


            // Delete QuestMachine Save File
            if (File.Exists(QuestMachineSavePath))
            {
                File.Delete(QuestMachineSavePath);
                Debug.Log($"Deleted QuestMachine save file: {QuestMachineSavePath}");
            }
            else
            {
                Debug.LogWarning($"QuestMachine save file not found: {QuestMachineSavePath}");
            }

            // Delete Dialogue System PlayerPrefs
            PlayerPrefs.DeleteAll();
            Debug.Log("Deleted Dialogue System PlayerPrefs.");

            SaveSystem.ClearSavedGameData();
            
            Debug.Log("All save data cleared successfully.");
        }
    }
}
#endif
