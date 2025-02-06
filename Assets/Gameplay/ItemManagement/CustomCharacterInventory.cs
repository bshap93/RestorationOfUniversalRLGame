using MoreMountains.InventoryEngine;
using MoreMountains.TopDownEngine;
using Project.Gameplay.Interactivity.Items;

namespace Gameplay.ItemManagement
{
    public class CustomCharacterInventory : CharacterInventory
    {
        public override void EquipWeapon(string weaponID)
        {
            if (HotbarInventory == null) return;

            // Find the weapon in the Hotbar
            foreach (var slot in HotbarInventory.Content)
                if (!InventoryItem.IsNull(slot) && slot.ItemID == weaponID)
                {
                    MMInventoryEvent.Trigger(
                        MMInventoryEventType.EquipRequest, null, HotbarInventory.name, slot, 0, 0, PlayerID);

                    break;
                }
        }
    }
}
