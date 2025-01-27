using MoreMountains.InventoryEngine;
using MoreMountains.TopDownEngine;
using Plugins.TopDownEngine.ThirdParty.MoreMountains.InentoryEngine.InventoryEngine.Scripts.Items;
using UnityEngine;

namespace Project.Gameplay.Combat.Shields
{
    [CreateAssetMenu(fileName = "ShieldItem", menuName = "Inventory/Equipment/Shield")]
    public class InventoryShieldItem : BaseItem
    {
        [Header("Shield Settings")] [Tooltip("The shield prefab to instantiate when equipped")]
        public Shield ShieldPrefab;

        [Tooltip("Which inventory this shield should be equipped to")]
        public string EquipmentInventoryName = "EquipmentInventory";

        [Tooltip("Should this shield auto-equip when picked up")]
        public bool AutoEquipOnPickup;

        public override bool Equip(string playerID)
        {
            // Find the character that owns this inventory
            var character = TargetInventory(playerID)?.Owner?.GetComponent<Character>();
            if (character == null) return false;

            // Find shield handler
            var shieldHandler = character.FindAbility<CharacterHandleShield>();
            if (shieldHandler == null)
            {
                Debug.LogWarning($"No shield handler found on character {character.name}");
                return false;
            }

            // Equip the shield
            shieldHandler.EquipShield(ShieldPrefab);

            // Move item to equipment inventory if needed
            var equipmentInventory = Inventory.FindInventory(EquipmentInventoryName, playerID);
            if (equipmentInventory != null && TargetInventory(playerID) != equipmentInventory)
                MMInventoryEvent.Trigger(
                    MMInventoryEventType.InventoryOpens, null, equipmentInventory.name, this, 1, 0, playerID);

            return true;
        }

        public override bool UnEquip(string playerID)
        {
            var character = TargetInventory(playerID)?.Owner?.GetComponent<Character>();
            if (character == null) return false;

            var shieldHandler = character.FindAbility<CharacterHandleShield>();
            if (shieldHandler != null)
            {
                shieldHandler.EquipShield(null);
                return true;
            }

            return false;
        }
    }
}
