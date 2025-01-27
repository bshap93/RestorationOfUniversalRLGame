#if UNITY_EDITOR
using System.IO;
using UnityEditor;
using UnityEngine;

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

            Debug.Log("All save data cleared successfully.");
        }
    }
}
#endif
