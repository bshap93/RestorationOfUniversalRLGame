using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
#endif

namespace Gameplay.ItemsInteractions
{
    public class DestructableManager : MonoBehaviour
    {
        public string SaveFileName = "DestroyedDestructable.es3";
        public static HashSet<string> DestroyedDestructables = new();
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

            // pickableManager.LoadPickedItems(); // Load loot first
            StartCoroutine(DestroyDestructablesAfterLoot()); // Delay container destruction
        }

        IEnumerator DestroyDestructablesAfterLoot()
        {
            yield return new WaitForSeconds(0.5f); // Wait for loot to spawn

            LoadDestroyedContainers(); // Now destroy containers
        }


        public void LoadDestroyedContainers()
        {
            DestroyedDestructables.Clear();

            if (ES3.FileExists("DestroyedContainers.es3"))
            {
                var keys = ES3.GetKeys("DestroyedContainers.es3");
                foreach (var key in keys)
                    if (ES3.Load<bool>(key, "DestroyedContainers.es3"))
                        DestroyedDestructables.Add(key);
            }

            // Find and destroy any containers that should be destroyed
            var destructables = FindObjectsByType<BaseDestructable>(FindObjectsSortMode.None);
            foreach (var destructable in destructables)
                if (IsContainerDestroyed(destructable.UniqueID))
                {
                    Debug.Log($"Destroying container {destructable.UniqueID} on load");
                    destructable.DestroyDestructableObject(false);
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
            DestroyedDestructables.Add(uniqueID);
            Debug.Log($"Saved destroyed container: {uniqueID}");
        }

        public static bool IsContainerDestroyed(string uniqueID)
        {
            return !string.IsNullOrEmpty(uniqueID) && DestroyedDestructables.Contains(uniqueID);
        }

        public static void ResetDestroyedContainers()
        {
            ES3.DeleteFile("DestroyedContainers.es3");
            DestroyedDestructables.Clear();
        }
    }
}
