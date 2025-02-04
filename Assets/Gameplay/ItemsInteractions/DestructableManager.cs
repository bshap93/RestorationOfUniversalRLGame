using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
#endif

namespace Gameplay.ItemsInteractions
{
    public class DestructableManager : MonoBehaviour
    {
        public static HashSet<string> DestroyedContainers = new();
        public PickableManager pickableManager;

        void Start()
        {
            if (pickableManager == null)
            {
                pickableManager = FindFirstObjectByType<PickableManager>();
                if (pickableManager == null)
                {
                    Debug.LogError("PickableManager not found in the scene! Loot may not load correctly.");
                    return;
                }
            }

            pickableManager.LoadPickedItems(); // Load loot first
            LoadDestroyedContainers();
        }


        public void LoadDestroyedContainers()
        {
            DestroyedContainers.Clear();

            if (ES3.FileExists("DestroyedContainers.es3"))
            {
                var keys = ES3.GetKeys("DestroyedContainers.es3");
                foreach (var key in keys)
                    if (ES3.Load<bool>(key, "DestroyedContainers.es3"))
                    {
                        DestroyedContainers.Add(key);
                        Debug.Log($"Loaded destroyed container: {key}");
                    }
            }

            // Find and destroy any containers that should be destroyed
            var containers = FindObjectsByType<ContainerDestruction>(FindObjectsSortMode.None);
            foreach (var container in containers)
                if (IsContainerDestroyed(container.UniqueID))
                {
                    Debug.Log($"Destroying container {container.UniqueID} on load");
                    container.DestroyContainer(false);
                }
        }

        public static void SaveDestroyedContainer(string uniqueID)
        {
            if (string.IsNullOrEmpty(uniqueID))
            {
                Debug.LogError("Attempted to save container with null or empty ID");
                return;
            }

            ES3.Save(uniqueID, true, "DestroyedContainers.es3");
            DestroyedContainers.Add(uniqueID);
            Debug.Log($"Saved destroyed container: {uniqueID}");
        }

        public static bool IsContainerDestroyed(string uniqueID)
        {
            return !string.IsNullOrEmpty(uniqueID) && DestroyedContainers.Contains(uniqueID);
        }

        public static void ResetDestroyedContainers()
        {
            ES3.DeleteFile("DestroyedContainers.es3");
            DestroyedContainers.Clear();
        }
    }
}
