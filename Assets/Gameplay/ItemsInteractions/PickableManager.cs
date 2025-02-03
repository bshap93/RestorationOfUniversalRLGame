#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Gameplay.ItemsInteractions
{
    public static class PickableManagerDebug
    {
        [MenuItem("Debug/Reset Picked Items")]
        public static void ResetPickedItemsMenu()
        {
            PickableManager.ResetPickedItems();
        }
    }
#endif


    public class PickableManager : MonoBehaviour
    {
        public static HashSet<string> PickedItems = new();

        void Awake()
        {
            LoadPickedItems();

            // Don't destroy this object when loading a new scene
            DontDestroyOnLoad(gameObject);
        }

        void LoadPickedItems()
        {
            if (ES3.FileExists("PickedItems.es3"))
            {
                var keys = ES3.GetKeys("PickedItems.es3");
                foreach (var key in keys)
                    if (ES3.Load<bool>(key, "PickedItems.es3"))
                    {
                        PickedItems.Add(key);
                    }
                    else
                    {
                        // If the item is a loot drop, respawn it
                        var savedPosition = ES3.Load(key, "PickedItems.es3", Vector3.zero);
                        if (savedPosition != Vector3.zero)
                        {
                            var lootPrefab = Resources.Load<GameObject>("PathToLootPrefab"); // Adjust as needed
                            if (lootPrefab != null)
                            {
                                Instantiate(lootPrefab, savedPosition, Quaternion.identity);
                                Debug.Log($"Restored loot {key} at {savedPosition}");
                            }
                        }
                    }
            }
        }


        public static void ResetPickedItems()
        {
            // Delete the Easy Save file storing picked items
            ES3.DeleteFile("PickedItems.es3");

            // Clear the in-memory picked items list (if used)
            PickedItems.Clear();
        }


        public static bool IsItemPicked(string uniqueID)
        {
            return PickedItems.Contains(uniqueID);
        }
    }
}
