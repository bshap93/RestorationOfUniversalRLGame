using System.Collections;
using MoreMountains.InventoryEngine;
using UnityEngine;

namespace Gameplay.ItemManagement.InventoryDisplays
{
    public class MainInventoryDisplay : InventoryDisplay
    {
        protected virtual void Start()
        {
            StartCoroutine(DelayedInventoryRefresh());
        }

        IEnumerator DelayedInventoryRefresh()
        {
            yield return new WaitForSeconds(0.1f); // Allow time for inventory system to initialize
            if (TargetInventory != null)
            {
                Initialization(true); // Forces UI redraw
            }
            else
            {
                Debug.LogWarning("[InventoryDisplay] TargetInventory is null after scene load!");
            }
        }


        public override void OnMMEvent(MMInventoryEvent mmEvent)
        {
            base.OnMMEvent(mmEvent);

            // Handle unequipping when moving items to MainInventory
            if (mmEvent.InventoryEventType == MMInventoryEventType.Move &&
                mmEvent.EventItem != null &&
                mmEvent.EventItem.Equippable)
            {
                // Trigger the unequip event
                MMInventoryEvent.Trigger(
                    MMInventoryEventType.ItemUnEquipped,
                    mmEvent.Slot,
                    TargetInventory.name,
                    mmEvent.EventItem,
                    mmEvent.Quantity,
                    mmEvent.Index,
                    PlayerID
                );

                Debug.Log($"Unequipped {mmEvent.EventItem.ItemID} when moved to MainInventory.");
            }
        }
    }
}
