using System.Collections.Generic;
using Gameplay.Player.Inventory;
using UnityEngine;

namespace Gameplay.ItemsInteractions
{
    public class PickableManager : MonoBehaviour
    {
        public static HashSet<string> PickedItems = new();
        [Tooltip("Dictionary of prefab names to their original SaveableLoot components")]
        readonly Dictionary<string, SaveableLoot> lootSources = new();

        void Start()
        {
            // Find all SaveableLoot components in the scene
            var containers = FindObjectsByType<ContainerDestruction>(FindObjectsSortMode.None);
            foreach (var container in containers)
            {
                var loot = container.GetComponent<SaveableLoot>();
                if (loot != null)
                {
                    // Store each unique prefab type
                    var prefabName = loot.GetLootPrefabName();
                    if (!string.IsNullOrEmpty(prefabName) && !lootSources.ContainsKey(prefabName))
                    {
                        lootSources[prefabName] = loot;
                        Debug.Log($"Registered loot source for prefab: {prefabName}");
                    }
                }
            }

            LoadPickedItems();
        }

        public void LoadPickedItems()
        {
            PickedItems.Clear();

            if (!ES3.FileExists("PickedItems.es3"))
                return;

            if (lootSources.Count == 0)
            {
                Debug.LogError("Cannot restore loot: No SaveableLoot components found");
                return;
            }

            var keys = ES3.GetKeys("PickedItems.es3");
            foreach (var key in keys)
                // Position keys are stored with "_pos" suffix
                if (key.EndsWith("_pos"))
                {
                    var itemId = key.Substring(0, key.Length - 4); // Remove "_pos" suffix

                    // Skip if this item was picked up
                    if (ES3.KeyExists($"{itemId}_state", "PickedItems.es3") &&
                        ES3.Load<bool>($"{itemId}_state", "PickedItems.es3"))
                    {
                        PickedItems.Add(itemId);
                        continue;
                    }

                    // Get the prefab type from the saved metadata
                    var prefabName = ES3.Load($"{itemId}_type", "PickedItems.es3", "");
                    if (string.IsNullOrEmpty(prefabName) || !lootSources.ContainsKey(prefabName))
                    {
                        Debug.LogError($"Could not find loot source for prefab type: {prefabName}");
                        continue;
                    }

                    var lootPrefab = lootSources[prefabName].GetLootPrefab();
                    var savedPosition = ES3.Load<Vector3>(key, "PickedItems.es3");

                    if (savedPosition != Vector3.zero && lootPrefab != null)
                    {
                        var spawnedLoot = Instantiate(lootPrefab, savedPosition, Quaternion.identity);
                        var itemPicker = spawnedLoot.GetComponent<ManualItemPicker>();
                        if (itemPicker != null)
                        {
                            itemPicker.UniqueID = itemId;
                            Debug.Log($"Restored loot {itemId} ({prefabName}) at {savedPosition}");
                        }
                        else
                        {
                            Debug.LogError("Spawned loot prefab does not have ManualItemPicker component!");
                            Destroy(spawnedLoot);
                        }
                    }
                }
        }

        public static void SavePickedItem(string uniqueID, bool isPicked)
        {
            ES3.Save($"{uniqueID}_state", isPicked, "PickedItems.es3");
            if (isPicked)
            {
                PickedItems.Add(uniqueID);
                // When an item is picked up, we should remove its position data
                // but keep the type data for reference
                if (ES3.KeyExists($"{uniqueID}_pos", "PickedItems.es3"))
                    ES3.DeleteKey($"{uniqueID}_pos", "PickedItems.es3");
            }
        }

        public static void SaveItemPosition(string uniqueID, Vector3 position, string prefabName)
        {
            ES3.Save($"{uniqueID}_pos", position, "PickedItems.es3");
            ES3.Save($"{uniqueID}_type", prefabName, "PickedItems.es3");
            Debug.Log($"Saved position for {uniqueID} ({prefabName}) at {position}");
        }

        public static void ResetPickedItems()
        {
            ES3.DeleteFile("PickedItems.es3");
            PickedItems.Clear();
        }

        public static bool IsItemPicked(string uniqueID)
        {
            return PickedItems.Contains(uniqueID);
        }
    }
}
