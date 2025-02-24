using MoreMountains.InventoryEngine;
using UnityEngine;

namespace Gameplay.ItemManagement.InventoryTypes
{
    public class MainInventory : Inventory
    {
        public const string MainInventoryObjectName = "MainPlayerInventory";

        public override bool MoveItemToInventory(int startIndex, Inventory targetInventory, int endIndex)
        {
            var itemToMove = Content[startIndex].Copy();

            // Trigger unequip event if the item is equippable
            if (itemToMove.Equippable)
            {
                MMInventoryEvent.Trigger(
                    MMInventoryEventType.ItemUnEquipped,
                    null,
                    name,
                    itemToMove,
                    itemToMove.Quantity,
                    startIndex,
                    PlayerID
                );

                Debug.Log($"Unequipped {itemToMove.ItemID} when moving to {targetInventory.name}");
            }

            if (endIndex >= 0)
                targetInventory.AddItemAt(itemToMove, itemToMove.Quantity, endIndex);
            else
                targetInventory.AddItem(itemToMove, itemToMove.Quantity);

            // Remove the item from this inventory
            RemoveItem(startIndex, itemToMove.Quantity);

            return true;
        }
    }
}
