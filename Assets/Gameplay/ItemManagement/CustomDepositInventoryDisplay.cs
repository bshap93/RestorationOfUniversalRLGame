using Gameplay.ItemManagement.InventoryTypes;
using MoreMountains.InventoryEngine;
using Project.Gameplay.Interactivity.Items;
using UnityEngine;

namespace Gameplay.ItemManagement
{
    public class CustomDepositInventoryDisplay : InventoryDisplay

    {
        public string MainInventoryName = MainInventory.MainInventoryObjectName;

        protected Inventory _mainInventory;

        protected override void Awake()
        {
            base.Awake();
            _mainInventory = FindMainInventory();
        }

        protected Inventory FindMainInventory()
        {
            foreach (var inventory in FindObjectsOfType<Inventory>())
                if (inventory.name == MainInventoryName && inventory.PlayerID == PlayerID)
                    return inventory;

            Debug.LogWarning("MainPlayerInventory not found!");
            return null;
        }

        public void OnRightClick(InventorySlot slot)
        {
            if (_mainInventory == null || slot == null || InventoryItem.IsNull(slot.CurrentItem)) return;

            // Try moving the item back to the main inventory
            var success = TargetInventory.MoveItemToInventory(slot.Index, _mainInventory);

            if (success)
                Debug.Log("Item moved back to MainPlayerInventory.");
            else
                Debug.Log("Failed to move item back to MainPlayerInventory.");
        }
    }
}
