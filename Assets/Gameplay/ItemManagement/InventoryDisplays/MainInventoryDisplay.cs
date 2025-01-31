using MoreMountains.InventoryEngine;
using MoreMountains.Tools;
using MoreMountains.TopDownEngine;
using UnityEngine;

namespace Gameplay.ItemManagement.InventoryDisplays
{
    public class MainInventoryDisplay : InventoryDisplay, MMEventListener<MMCameraEvent>
    {
        public void OnMMEvent(MMCameraEvent eventType)
        {
            if (eventType.EventType == MMCameraEventTypes.SetTargetCharacter)
            {
                Debug.Log("MainInventoryDisplay: SetTargetCharacter event received.");
                Initialization();
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
