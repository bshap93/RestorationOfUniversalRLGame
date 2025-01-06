using System;
using Project.Gameplay.Interactivity.Items;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Serialization;

namespace Project.Gameplay.ItemManagement.InventoryTypes.Materials
{
    [Serializable]
    public class CraftingMaterial
    {
        [FormerlySerializedAs("Item")] [ShowInInspector]
        public InventoryItem item; // The base inventory item
        [FormerlySerializedAs("Quantity")] [ShowInInspector]
        public int quantity; // How many units are required
        [ShowInInspector] public GameObject prefabDrop; // Optional: The prefab to drop if the inventory is full

        public CraftingMaterial(InventoryItem item, int quantity, float craftingTime = 1f)
        {
            this.item = item;
            this.quantity = quantity;
        }
    }
}
