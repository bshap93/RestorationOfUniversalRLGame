// Copyright © Pixel Crushers. All rights reserved.

using UnityEngine;
using System.Collections.Generic;
using MoreMountains.InventoryEngine;
using Project.Gameplay.Interactivity.Items;

namespace PixelCrushers.InventoryEngineSupport
{

    /// <summary>
    /// Functions to work with Inventory Engine.
    /// </summary>
    public static class InventoryEngineUtils
    {

        public static bool debug = false;

        private static Dictionary<string, Inventory> inventoryCache = new Dictionary<string, Inventory>();
        private static Dictionary<string, InventoryItem> itemCache = null; // InitItemCache() will create.

        public static Inventory FindInventory(string inventoryName)
        {
            if (inventoryCache.ContainsKey(inventoryName) && inventoryCache[inventoryName] != null)
            {
                return inventoryCache[inventoryName];
            }
            var go = GameObject.Find(inventoryName) ?? GameObjectUtility.GameObjectHardFind(inventoryName);
            var inventory = (go != null) ? go.GetComponent<Inventory>() : null;
            if (inventory != null)
            {
                if (inventoryCache.ContainsKey(inventoryName))
                {
                    inventoryCache[inventoryName] = inventory;
                }
                else
                {
                    inventoryCache.Add(inventoryName, inventory);
                }
            }
            if (inventory == null && Debug.isDebugBuild) Debug.LogWarning("Can't find Inventory GameObject named '" + inventoryName + "'.");
            return inventory;
        }

        public static InventoryItem FindItem(string itemID)
        {
            if (string.IsNullOrEmpty(itemID)) return null;
            InitItemCache();
            InventoryItem item = null;
            if (itemCache.TryGetValue(itemID, out item))
            {
                return item;
            }
            item = (Resources.Load(itemID) as InventoryItem) ?? (Resources.Load("Items/" + itemID) as InventoryItem);
            if (item != null)
            {
                itemCache[itemID] = item;
                return item;
            }
            else
            {
                if (item == null && Debug.isDebugBuild) Debug.LogWarning("Can't find item type '" + itemID + "' in a Resources folder.");
                return null;
            }
        }

        public static void InitItemCache()
        {
            if (itemCache == null)
            {
                // If no cache yet, build it:
                itemCache = new Dictionary<string, InventoryItem>();
                var itemAssets = Resources.LoadAll<InventoryItem>("");
                for (int i = 0; i < itemAssets.Length; i++)
                {
                    itemCache[itemAssets[i].ItemID] = itemAssets[i];
                }
                itemAssets = Resources.LoadAll<InventoryItem>("Items");
                for (int i = 0; i < itemAssets.Length; i++)
                {
                    itemCache[itemAssets[i].ItemID] = itemAssets[i];
                }
            }
        }

        public static int FindItemIndex(Inventory inventory, string itemID)
        {
            if (inventory == null || inventory.Content == null) return -1;
            for (int i = 0; i < inventory.Content.Length; i++)
            {
                var content = inventory.Content[i];
                if (content == null) continue;
                if (string.Equals(content.ItemID, itemID)) return i;
            }
            return -1;
        }

        public static void mmAddItem(string inventoryName, string itemID, int quantity)
        {
            if (debug) Debug.Log("mmAddItem('" + inventoryName + "', '" + itemID + "', " + quantity + ")");
            var inventory = FindInventory(inventoryName);
            if (inventory == null) return;
            var itemToAdd = FindItem(itemID);
            if (itemToAdd == null) return;
            inventory.AddItem(itemToAdd, (int)quantity);
        }

        public static void mmRemoveItem(string inventoryName, string itemID, int quantity)
        {
            if (debug) Debug.Log("mmRemoveItem('" + inventoryName + "', '" + itemID + "', " + quantity + ")");
            var inventory = FindInventory(inventoryName);
            if (inventory == null) return;
            if (inventory.Content == null) return;
            var leftToRemove = (int)quantity;
            for (int i = 0; i < inventory.Content.Length; i++)
            {
                var content = inventory.Content[i];
                if (content == null) continue;
                if (string.Equals(content.ItemID, itemID))
                {
                    var amountToRemoveFromSlot = Mathf.Min(leftToRemove, content.Quantity);
                    leftToRemove -= amountToRemoveFromSlot;
                    inventory.RemoveItem(i, amountToRemoveFromSlot);
                    if (leftToRemove <= 0) return;
                }
            }
        }

        public static int mmGetQuantity(string inventoryName, string itemID)
        {
            var inventory = FindInventory(inventoryName);
            if (inventory == null) return 0;
            var quantity = inventory.GetQuantity(itemID);
            if (debug) Debug.Log("mmGetQuantity('" + inventoryName + "', '" + itemID + "') returns " + quantity);
            return inventory.GetQuantity(itemID);
        }

        public static void mmUseItem(string inventoryName, string itemID)
        {
            if (debug) Debug.Log("mmUseItem('" + inventoryName + "', '" + itemID + ")");
            var inventory = FindInventory(inventoryName);
            if (inventory == null) return;
            inventory.UseItem(itemID);
        }

        public static void mmDropItem(string inventoryName, string itemID)
        {
            mmRemoveItem(inventoryName, itemID, 1);
        }

        public static void mmEquipItem(string inventoryName, string itemID)
        {
            if (debug) Debug.Log("mmEquipItem('" + inventoryName + "', '" + itemID + ")");
            var inventory = FindInventory(inventoryName);
            if (inventory == null) return;
            var item = FindItem(itemID);
            var itemIndex = FindItemIndex(inventory, itemID);
            if (item == null || itemIndex == -1) return;
            inventory.EquipItem(item, itemIndex);
        }

        public static void mmUnEquipItem(string inventoryName, string itemID)
        {
            if (debug) Debug.Log("mmUnEquipItem('" + inventoryName + "', '" + itemID + ")");
            var inventory = FindInventory(inventoryName);
            if (inventory == null) return;
            var item = FindItem(itemID);
            var itemIndex = FindItemIndex(inventory, itemID);
            if (item == null || itemIndex == -1) return;
            inventory.UnEquipItem(item, itemIndex);
        }

        public static void mmEmptyInventory(string inventoryName)
        {
            if (debug) Debug.Log("mmEmptyInventory()");
            var inventory = FindInventory(inventoryName);
            if (inventory == null) return;
            inventory.EmptyInventory();
        }

    }
}