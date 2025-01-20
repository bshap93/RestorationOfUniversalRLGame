using System;
using Project.Gameplay.Player.Inventory;
using UnityEditor;
using UnityEngine;

namespace Project.Gameplay.ItemsInteractions.Editor
{
    public class ManualItemPickerIDAssigner : UnityEditor.Editor
    {
        [MenuItem("Debug/Assign Unique IDs to ManualItemPickers")]
        static void AssignUniqueIDs()
        {
            // Find all ManualItemPicker components in the current scene
            var allItemPickers = FindObjectsByType<ManualItemPicker>(FindObjectsSortMode.None);

            // Check if any were found
            if (allItemPickers.Length == 0)
            {
                Debug.LogWarning("No ManualItemPicker components found in the scene.");
                return;
            }

            // Iterate through each ManualItemPicker and assign a unique ID
            foreach (var picker in allItemPickers)
                if (picker != null)
                {
                    // Generate a unique ID using GUID
                    picker.UniqueID = Guid.NewGuid().ToString();
                    EditorUtility.SetDirty(picker); // Mark the object as dirty for saving
                }

            // Save the scene to persist changes
            AssetDatabase.SaveAssets();

            Debug.Log($"Assigned unique IDs to {allItemPickers.Length} ManualItemPicker components.");
        }
    }
}
