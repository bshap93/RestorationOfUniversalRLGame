using System;
using MoreMountains.InventoryEngine;
using MoreMountains.Tools;
using MoreMountains.TopDownEngine;
using Project.Gameplay.Combat;
using UnityEngine;

namespace Project.Animation_Effects.CharacterAnimation.AnimationController.WeaponAnimators
{
    public class AnimatorEquipHandler : MonoBehaviour, MMEventListener<MMInventoryEvent>, MMEventListener<MMGameEvent>,
        MMEventListener<MMCameraEvent>
    {
        static readonly int ShieldUp = Animator.StringToHash("ShieldUp");
        [Header("Weapon Data")] public CustomInventoryWeapon[]
            weaponDataArray; // Array of ScriptableObjects holding each weapon's override controller
        CustomInventoryWeapon _customInventoryWeapon; // Store current weapon's data
        AnimatorOverrideController _defaultOverrideController; // Store current override

        Animator _playerAnimator;
        WeaponAnimationManager _weaponAnimationManager; // Reference to the WeaponAnimationManager

        public void Awake()
        {
            _playerAnimator = GetComponent<Animator>();
        }

        public void OnEnable()
        {
            this.MMEventStartListening<MMInventoryEvent>();
            this.MMEventStartListening<MMGameEvent>();
            this.MMEventStartListening<MMCameraEvent>();
        }

        public void OnDisable()
        {
            this.MMEventStopListening<MMInventoryEvent>();
            this.MMEventStopListening<MMGameEvent>();
        }

        public void OnMMEvent(MMCameraEvent eventType)
        {
            if (eventType.EventType == MMCameraEventTypes.SetTargetCharacter)
            {
                if (_playerAnimator == null)
                {
                    _playerAnimator = eventType.TargetCharacter.GetComponentInChildren<Animator>();

                    if (_playerAnimator == null)
                    {
                        Debug.LogError("No Animator found in target character.");
                        return;
                    }
                }

                _defaultOverrideController = new AnimatorOverrideController(_playerAnimator.runtimeAnimatorController);

                _weaponAnimationManager = FindObjectOfType<WeaponAnimationManager>();

                if (_weaponAnimationManager == null)
                {
                    Debug.LogError("No WeaponAnimationManager found in scene.");
                    return;
                }


                var storedWeaponID = _weaponAnimationManager.GetCurrentWeaponID();
                var weaponData = Array.Find(weaponDataArray, weapon => weapon.ItemID == storedWeaponID);

                if (weaponData != null)
                {
                    _playerAnimator.runtimeAnimatorController = _weaponAnimationManager.GetCurrentAnimatorController();
                    Debug.Log($"Restored weapon animator for: {storedWeaponID}");
                }
            }
        }
        public void OnMMEvent(MMGameEvent eventType)
        {
            if (eventType.EventName == "ShieldUpEvent") _playerAnimator.SetBool(ShieldUp, true);

            if (eventType.EventName == "ShieldDownEvent") _playerAnimator.SetBool(ShieldUp, false);
        }

        public void OnMMEvent(MMInventoryEvent eventType)
        {
            if (eventType.InventoryEventType == MMInventoryEventType.ItemEquipped)
                EquipWeapon(eventType.EventItem);
            else
                // print the event type
                Debug.Log(eventType.InventoryEventType);
        }

        void EquipWeapon(InventoryItem item)
        {
            if (item == null)
            {
                Debug.LogWarning("Tried to equip a null item.");
                return;
            }

            var weaponData = Array.Find(weaponDataArray, weapon => weapon.ItemID == item.ItemID);
            if (weaponData != null)
            {
                Debug.Log($"Equipping {weaponData.ItemID} and applying override controller.");
                _customInventoryWeapon = weaponData;
                _playerAnimator.runtimeAnimatorController = weaponData.runtimeAnimatorController;

                if (_weaponAnimationManager == null)
                {
                    Debug.LogError("No WeaponAnimationManager found in scene.");
                    return;
                }

                // Store the weapon data in our persistence manager
                _weaponAnimationManager.StoreCurrentWeapon(
                    weaponData.ItemID,
                    weaponData.runtimeAnimatorController
                );
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

                if (_weaponAnimationManager == null)
                {
                    Debug.LogError("No WeaponAnimationManager found in scene.");
                    return;
                }

                // Clear the stored weapon data
                _weaponAnimationManager.ClearStoredWeapon();
            }
            else
            {
                Debug.LogError("Default override controller is null. This shouldn't happen.");
            }
        }
    }
}
