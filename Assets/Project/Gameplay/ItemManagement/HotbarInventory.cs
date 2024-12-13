using MoreMountains.InventoryEngine;
using UnityEngine;

namespace Project.Gameplay.ItemManagement
{
    public class HotbarInventory : Inventory
    {
        public Inventory MainInventory;

        public override bool AddItem(InventoryItem itemToAdd, int quantity)
        {
            // if the item to add is null, we do nothing and exit
            if (itemToAdd == null)
            {
                Debug.LogWarning(name + " : The item you want to add to the inventory is null");
                return false;
            }

            var list = InventoryContains(itemToAdd.ItemID);
            ;
            quantity = CapMaxQuantity(itemToAdd, quantity);

            // if there's at least one item like this already in the inventory and it's stackable
            if (list.Count > 0 && itemToAdd.MaximumStack > 1)
                // we store items that match the one we want to add
                for (var i = 0; i < list.Count; i++)
                    // if there's still room in one of these items of this kind in the inventory, we add to it
                    if (Content[list[i]].Quantity < itemToAdd.MaximumStack)
                    {
                        // we increase the quantity of our item
                        Content[list[i]].Quantity += quantity;
                        // if this exceeds the maximum stack
                        if (Content[list[i]].Quantity > Content[list[i]].MaximumStack)
                        {
                            var restToAdd = itemToAdd;
                            var restToAddQuantity = Content[list[i]].Quantity - Content[list[i]].MaximumStack;
                            // we clamp the quantity and add the rest as a new item
                            Content[list[i]].Quantity = Content[list[i]].MaximumStack;
                            AddItem(restToAdd, restToAddQuantity);
                        }

                        MMInventoryEvent.Trigger(MMInventoryEventType.ContentChanged, null, name, null, 0, 0, PlayerID);
                        return true;
                    }

            // if we've reached the max size of our inventory, we try to add the item to the main inventory
            if (NumberOfFilledSlots >= Content.Length)
            {
                if (MainInventory != null)
                {
                    Debug.Log("Adding to main inventory");
                    var res = MainInventory.AddItem(itemToAdd, quantity);
                    return res;
                }

                return false;
            }

            while (quantity > 0)
                if (quantity > itemToAdd.MaximumStack)
                {
                    AddItem(itemToAdd, itemToAdd.MaximumStack);
                    quantity -= itemToAdd.MaximumStack;
                }
                else
                {
                    AddItemToArray(itemToAdd, quantity);
                    quantity = 0;
                }

            // if we're still here, we add the item in the first available slot
            MMInventoryEvent.Trigger(MMInventoryEventType.ContentChanged, null, name, null, 0, 0, PlayerID);
            return true;
        }
    }
}
