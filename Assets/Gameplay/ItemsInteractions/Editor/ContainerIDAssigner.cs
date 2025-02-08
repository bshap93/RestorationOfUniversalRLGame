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
            var allContainers = FindObjectsOfType<ContainerDestruction>();
            var allDestructableMineables = FindObjectsOfType<DestructableMineable>();

            if (allContainers.Length == 0) Debug.LogWarning("No ContainerDestruction components found in the scene.");

            if (allDestructableMineables.Length == 0)
                Debug.LogWarning("No DestructableMineable components found in the scene.");

            if (allContainers.Length == 0 && allDestructableMineables.Length == 0) return;


            foreach (var container in allContainers)
                if (container != null && string.IsNullOrEmpty(container.UniqueID))
                {
                    container.UniqueID = Guid.NewGuid().ToString();
                    EditorUtility.SetDirty(container);
                }


            foreach (var mineable in allDestructableMineables)
                if (mineable != null && string.IsNullOrEmpty(mineable.UniqueID))
                {
                    mineable.UniqueID = Guid.NewGuid().ToString();
                    EditorUtility.SetDirty(mineable);
                }

            AssetDatabase.SaveAssets();
            Debug.Log(
                $"Assigned unique IDs to {allContainers.Length} containers and {allDestructableMineables.Length} mineables.");
        }
    }
}
