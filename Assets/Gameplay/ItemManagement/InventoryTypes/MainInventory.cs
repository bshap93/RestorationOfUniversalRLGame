using MoreMountains.InventoryEngine;
using Project.Gameplay.Interactivity.Items;
using UnityEngine;

namespace Gameplay.ItemManagement.InventoryTypes
{
    public class MainInventory : MoreMountains.InventoryEngine.Inventory
    {
        public override bool MoveItemToInventory(int startIndex, MoreMountains.InventoryEngine.Inventory targetInventory, int endIndex)
        {
            InventoryItem itemToMove = Content[startIndex].Copy();

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
            {
                targetInventory.AddItemAt(itemToMove, itemToMove.Quantity, endIndex);
            }
            else
            {
                targetInventory.AddItem(itemToMove, itemToMove.Quantity);
            }

            // Remove the item from this inventory
            RemoveItem(startIndex, itemToMove.Quantity);

            return true;
        }
    }
}
