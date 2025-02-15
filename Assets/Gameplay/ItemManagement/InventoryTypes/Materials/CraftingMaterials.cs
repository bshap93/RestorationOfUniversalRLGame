using System;
using MoreMountains.InventoryEngine;
using UnityEngine;

namespace Project.Gameplay.ItemManagement.InventoryTypes.Materials

{
    [Serializable]
    public class CraftingMaterial
    {
        public InventoryItem item; // The base inventory item
        public int quantity; // How many units are required
        public GameObject prefabDrop; // Optional: The prefab to drop if the inventory is full

        public CraftingMaterial(InventoryItem item, int quantity, float craftingTime = 1f)
        {
            this.item = item;
            this.quantity = quantity;
        }
    }
}
