using System;
using MoreMountains.InventoryEngine;
using MoreMountains.Tools;
using Project.Gameplay.Combat;
using UnityEngine;

namespace Project.Gameplay.Animation.WeaponAnimators
{
    public class AnimatorEquipHandler : MonoBehaviour, MMEventListener<MMInventoryEvent>
    {
        [Header("Weapon Data")]
        public CustomInventoryWeapon[]
            weaponDataArray; // Array of ScriptableObjects holding each weapon's override controller
        CustomInventoryWeapon _customInventoryWeapon; // Store current weapon's data
        AnimatorOverrideController _defaultOverrideController; // Store current override

        Animator _playerAnimator;

        void Start()
        {
            _playerAnimator = GetComponent<Animator>();

            if (_playerAnimator == null)
            {
                Debug.LogError("No Animator component found on this object.");
                return;
            }

            // Store the default override controller (this may be null initially)
            _defaultOverrideController = new AnimatorOverrideController(_playerAnimator.runtimeAnimatorController);
        }

        public void OnEnable()
        {
            this.MMEventStartListening();
        }

        public void OnDisable()
        {
            this.MMEventStopListening();
        }

        public void OnMMEvent(MMInventoryEvent eventType)
        {
            if (eventType.InventoryEventType == MMInventoryEventType.ItemEquipped)
                EquipWeapon(eventType.EventItem);
            else if (eventType.InventoryEventType == MMInventoryEventType.ItemUnEquipped) ResetToDefaultAnimator();
        }

        void EquipWeapon(InventoryItem item)
        {
            if (item == null)
            {
                Debug.LogWarning("Tried to equip a null item.");
                return;
            }

            // Look for matching weapon data
            var weaponData = Array.Find(weaponDataArray, weapon => weapon.ItemID == item.ItemID);
            if (weaponData != null)
            {
                Debug.Log($"Equipping {weaponData.ItemID} and applying override controller.");
                _customInventoryWeapon = weaponData;
                _playerAnimator.runtimeAnimatorController = weaponData.overrideController;
            }
            else
            {
                Debug.LogWarning($"No weapon data found for item: {item.ItemID}");
            }
        }

        void ResetToDefaultAnimator()
        {
            if (_defaultOverrideController != null)
            {
                Debug.Log("Resetting to default animator.");
                _playerAnimator.runtimeAnimatorController = _defaultOverrideController;
                _customInventoryWeapon = null;
            }
            else
            {
                Debug.LogError("Default override controller is null. This shouldn't happen.");
            }
        }
    }
}