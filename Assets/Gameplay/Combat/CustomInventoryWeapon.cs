using System;
using Gameplay.Combat.Tools;
using Gameplay.Player.Inventory;
using MoreMountains.InventoryEngine;
using Project.Gameplay.Combat.Weapons;
using UnityEngine;
using UnityEngine.Serialization;

namespace Project.Gameplay.Combat
{
    [CreateAssetMenu(
        fileName = "CustomInventoryWeapon", menuName = "Roguelike/Weapons/CustomInventoryWeapon", order = 3)]
    [Serializable]
    public class CustomInventoryWeapon : BaseWeapon
    {
        [FormerlySerializedAs("WeaponAttachmentType")]
        public WeaponAttachmentType weaponAttachmentType;
        public bool isTwoHanded;
        [FormerlySerializedAs("overrideController")]
        public RuntimeAnimatorController runtimeAnimatorController;

        public string secondaryTargetEquipmentInventoryName;

        [SerializeField] ItemType itemType = ItemType.CustomInventoryWeapon;

        /// <summary>
        ///     When we grab the weapon, we equip it, occupying both the primary and secondary inventory slots if the weapon is
        ///     two-handed.
        /// </summary>
        public override bool Equip(string playerID)
        {
            // If a left-hand item is already equipped, unequip it
            if (!string.IsNullOrEmpty(secondaryTargetEquipmentInventoryName))
            {
                var secondaryEquipmentInventory = Inventory.FindInventory(
                    secondaryTargetEquipmentInventoryName, playerID);

                if (secondaryEquipmentInventory != null)
                    // Check if there are any items equipped in the left hand and unequip them
                    if (secondaryEquipmentInventory.InventoryContains(ItemID + "_Secondary").Count > 0)
                        secondaryEquipmentInventory.RemoveItemByID(ItemID + "_Secondary", 1);
            }

            EquipWeapon(EquippableWeapon, playerID);

            if (isTwoHanded && !string.IsNullOrEmpty(secondaryTargetEquipmentInventoryName))
            {
                var secondaryEquipmentInventory = Inventory.FindInventory(
                    secondaryTargetEquipmentInventoryName, playerID);

                if (secondaryEquipmentInventory != null)
                {
                    // We create a copy of this item and place it in the secondary inventory
                    var secondaryItem = Copy();
                    secondaryItem.ItemID = ItemID + "_Secondary"; // Unique identifier for the secondary item
                    secondaryEquipmentInventory.AddItem(secondaryItem, 1); // Add one instance of the item
                }
            }

            return true;
        }

        /// <summary>
        ///     When dropping or unequipping a weapon, we remove it from both primary and secondary inventory slots if the weapon
        ///     is two-handed.
        /// </summary>
        public override bool UnEquip(string playerID)
        {
            // UnEquip from primary inventory
            if (TargetEquipmentInventory(playerID) == null) return false;

            if (TargetEquipmentInventory(playerID).InventoryContains(ItemID).Count > 0) EquipWeapon(null, playerID);

            // If the weapon is two-handed, also remove it from the secondary equipment inventory
            if (isTwoHanded && !string.IsNullOrEmpty(secondaryTargetEquipmentInventoryName))
            {
                var secondaryEquipmentInventory = Inventory.FindInventory(
                    secondaryTargetEquipmentInventoryName, playerID);

                if (secondaryEquipmentInventory != null)
                    // Remove the secondary item from the secondary equipment inventory
                    secondaryEquipmentInventory.RemoveItemByID(
                        ItemID + "_Secondary", 1); // Remove one instance of the item
            }

            return true;
        }

        /// <summary>
        ///     Ensures that equipping another item (like a shield) will unequip both slots of a two-handed weapon.
        /// </summary>
        public override void Swap(string playerID)
        {
            if (isTwoHanded && !string.IsNullOrEmpty(secondaryTargetEquipmentInventoryName))
                // UnEquip both primary and secondary slots
                UnEquip(playerID);

            base.Swap(playerID);
        }
    }
}
