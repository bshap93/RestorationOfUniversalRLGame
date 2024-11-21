using MoreMountains.TopDownEngine;
using Project.Gameplay.Combat.Weapons;
using UnityEngine;
using UnityEngine.Serialization;

namespace Project.Gameplay.Combat
{
    public class CustomAltCharacterHandleWeapon : AltCharacterHandleWeapon
    {
        // Define attachment points in the character prefab
        [FormerlySerializedAs("MeleeWeaponAttachment")]
        public Transform meleeWeaponAttachment;
        [FormerlySerializedAs("RangedWeaponAttachment")]
        public Transform rangedWeaponAttachment;

        protected override void InstantiateWeapon(Weapon newWeapon, string weaponID, bool combo = false)
        {
            if (newWeapon == null) return;

            // Default attachment point
            var chosenAttachment = WeaponAttachment;

            // Decide attachment point based on weapon type
            if (newWeapon is MeleeWeapon)
            {
                chosenAttachment = meleeWeaponAttachment;

                // Disable IK for melee weapons
                if (_weaponIK != null) _weaponIK.enabled = false;
            }
            else if (newWeapon is ProjectileWeapon)
            {
                chosenAttachment = rangedWeaponAttachment;

                // Enable IK for ranged weapons
                if (_weaponIK != null) _weaponIK.enabled = true;
            }

            // Instantiate the weapon at the chosen attachment point
            CurrentWeapon = Instantiate(
                newWeapon, chosenAttachment.position + newWeapon.WeaponAttachmentOffset, chosenAttachment.rotation);

            CurrentWeapon.transform.SetParent(chosenAttachment);

            // Initialize the weapon
            CurrentWeapon.name = newWeapon.name;
            CurrentWeapon.SetOwner(_character, this);
            CurrentWeapon.WeaponID = weaponID;
            CurrentWeapon.FlipWeapon();
            _weaponAim = CurrentWeapon.GetComponent<WeaponAim>();

            HandleWeaponAim(); // Ensures aim logic is applied
            CurrentWeapon.Initialization();
            CurrentWeapon.InitializeComboWeapons();
            InitializeAnimatorParameters();
        }

        protected override void HandleWeaponAim()
        {
            if (_weaponAim != null)
            {
                // Only apply aim rotation for non-melee weapons
                if (CurrentWeapon is MeleeWeapon)
                {
                    _weaponAim.enabled = false; // Disable aim logic for melee weapons
                    _character.transform.rotation = Quaternion.identity; // Reset character's rotation
                }
                else
                {
                    _weaponAim.enabled = true; // Enable aim logic for ranged/projectile weapons
                }
            }
        }

        public void RotateTowardsTarget(Vector3 targetPosition)
        {
            var direction = (targetPosition - _character.transform.position).normalized;
            var targetRotation = Quaternion.LookRotation(direction, Vector3.up);
            _character.transform.rotation = Quaternion.Slerp(
                _character.transform.rotation, targetRotation, Time.deltaTime * 10f);
        }
    }
}
