using System;
using Gameplay.Combat.Tools;
using Gameplay.Player.Inventory;
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


        [SerializeField] ItemType itemType = ItemType.CustomInventoryWeapon;

        /// <summary>
        ///     When we grab the weapon, we equip it, occupying both the primary and secondary inventory slots if the weapon is
        ///     two-handed.
        /// </summary>
        public override bool Equip(string playerID)
        {
            EquipWeapon(EquippableWeapon, playerID);
            return true;
        }


        public override bool UnEquip(string playerID)
        {
            if (TargetEquipmentInventory(playerID) == null) return false;
            EquipWeapon(null, playerID);
            return true;
        }
    }
}
