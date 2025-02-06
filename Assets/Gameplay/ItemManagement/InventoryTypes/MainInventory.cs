using MoreMountains.InventoryEngine;
using Project.Gameplay.Interactivity.Items;
using UnityEngine;

namespace Gameplay.ItemManagement.InventoryTypes
{
    public class MainInventory : Inventory
    {
        const string PlayerID = "Player1";
        public const string MainInventoryObjectName = "MainPlayerInventory";
        public const string MainInventoryTag = "MainPlayerInventory";

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

        public override void EquipItem(InventoryItem item, int index, InventorySlot slot = null)
        {
            if (InventoryType == InventoryTypes.Main)
            {
                if (item == null) return;

                // Ensure item is NOT removed from the Hotbar
                if (name.Contains("Hotbar")) // Make sure "Hotbar" is in the inventory name
                {
                    Debug.Log($"Equipping {item.ItemID} from Hotbar - keeping in inventory.");
                    MMInventoryEvent.Trigger(MMInventoryEventType.ItemEquipped, slot, name, item, 0, index, PlayerID);
                    return; // Don't move it to another inventory
                }

                InventoryItem oldItem = null;
                if (!InventoryItem.IsNull(item))
                    // Normal Equip logic if NOT coming from hotbar
                    if (!item.TargetEquipmentInventory(PlayerID).IsFull)
                    {
                        oldItem = item.TargetEquipmentInventory(PlayerID).Content[0];
                        item.TargetEquipmentInventory(PlayerID).MoveItemToInventory(
                            index, item.TargetEquipmentInventory(PlayerID));
                    }

                // Fire Equip Event
                MMInventoryEvent.Trigger(MMInventoryEventType.ItemEquipped, slot, name, item, 0, index, PlayerID);
            }
        }
    }
}
