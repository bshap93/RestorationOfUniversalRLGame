using UnityEngine;

namespace Project.Gameplay.Combat.Weapons
{
    [AddComponentMenu("Roguelike/Weapon Attachment Type")]
    public class WeaponAttachmentTypeComponent : MonoBehaviour
    {
        [Tooltip("Defines the attachment type for this weapon.")]
        public WeaponAttachmentType AttachmentType;
        [Tooltip("Determine if WeaponIK should be used for this weapon.")]
        public bool UseWeaponIK;
    }
}
