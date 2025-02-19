using System;
using UnityEditor;
using UnityEngine;

namespace Gameplay.ItemsInteractions.Editor
{
    public class ContainerIDAssigner : UnityEditor.Editor
    {
        [MenuItem("Debug/Assign Unique IDs to Destructables")]
        static void AssignUniqueIDs()
        {
            var allDestructables = FindObjectsOfType<BaseDestructible>();

            if (allDestructables.Length == 0)
                Debug.LogWarning("No ContainerDestruction components found in the scene.");


            if (allDestructables.Length == 0) return;


            foreach (var destructable in allDestructables)
                if (destructable != null && string.IsNullOrEmpty(destructable.UniqueID))
                {
                    destructable.UniqueID = Guid.NewGuid().ToString();
                    EditorUtility.SetDirty(destructable);
                }


            AssetDatabase.SaveAssets();
            Debug.Log(
                $"Assigned unique IDs to {allDestructables.Length} destructables");
        }
    }
}
