using System;
using System.Collections.Generic;
using MoreMountains.InventoryEngine;
using Project.Gameplay.Interactivity.Items;
using UnityEngine;

namespace Project.Gameplay.ItemsInteractions
{
    [Serializable]
    public class StarterItemsForAnInventory
    {
        public Inventory inventory;
        public List<InventoryItem> starterItems;
    }

    public class InitializeInventoriesWithStarterItems : MonoBehaviour
    {
        public List<StarterItemsForAnInventory> starterItemsForInventories;
        void Start()
        {
            foreach (var starterItemsForInventory in starterItemsForInventories)
            {
                foreach (var starterItem in starterItemsForInventory.starterItems)
                    starterItemsForInventory.inventory.AddItem(starterItem, 1);

                Debug.Log(
                    $"Added {starterItemsForInventory.starterItems.Count} items to {starterItemsForInventory.inventory.name}");
            }
        }
    }
}
