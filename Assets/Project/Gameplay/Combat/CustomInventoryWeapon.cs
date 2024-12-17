using System;
using MoreMountains.TopDownEngine;
using Project.Gameplay.Combat.Weapons;
using UnityEngine;
using UnityEngine.Serialization;

namespace Project.Gameplay.Combat
{
    [CreateAssetMenu(
        fileName = "CustomInventoryWeapon", menuName = "Roguelike/Weapons/CustomInventoryWeapon", order = 3)]
    [Serializable]
    public class CustomInventoryWeapon : InventoryWeapon
    {
        [FormerlySerializedAs("WeaponAttachmentType")]
        public WeaponAttachmentType weaponAttachmentType;
        public AnimatorOverrideController overrideController;
    }
}
