using System;
using System.Collections.Generic;
using MoreMountains.InventoryEngine;
using UnityEngine;

namespace Gameplay.ItemsInteractions
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
            foreach (var starterItem in starterItemsForInventory.starterItems)
                starterItemsForInventory.inventory.AddItem(starterItem, 1);
        }
    }
}
