using System;
using UnityEditor;
using UnityEngine;

namespace Gameplay.ItemsInteractions.Editor
{
    public class ContainerIDAssigner : UnityEditor.Editor
    {
        [MenuItem("Debug/Assign Unique IDs to Containers")]
        static void AssignUniqueIDs()
        {
            var allContainers = FindObjectsOfType<ContainerDestruction>();

            if (allContainers.Length == 0)
            {
                Debug.LogWarning("No ContainerDestruction components found in the scene.");
                return;
            }

            foreach (var container in allContainers)
                if (container != null && string.IsNullOrEmpty(container.UniqueID))
                {
                    container.UniqueID = Guid.NewGuid().ToString();
                    EditorUtility.SetDirty(container);
                }

            AssetDatabase.SaveAssets();
            Debug.Log($"Assigned unique IDs to {allContainers.Length} containers.");
        }
    }
}
