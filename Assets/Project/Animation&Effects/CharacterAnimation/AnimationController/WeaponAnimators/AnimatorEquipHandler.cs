using System;
using MoreMountains.InventoryEngine;
using MoreMountains.Tools;
using MoreMountains.TopDownEngine;
using Project.Gameplay.Combat;
using Project.Gameplay.Interactivity.Items;
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

            if (_playerAnimator == null) Debug.LogError("No Animator component found on this GameObject.");

            _weaponAnimationManager = FindObjectOfType<WeaponAnimationManager>();
            if (_weaponAnimationManager == null)
                Debug.LogWarning("No WeaponAnimationManager found in the scene. This may cause issues.");
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

        public void OnMMEvent(MMCameraEvent mmEvent)
        {
            if (mmEvent.EventType != MMCameraEventTypes.SetTargetCharacter) return;

            // Find and assign the player's animator
            _playerAnimator = mmEvent.TargetCharacter?.GetComponentInChildren<Animator>();
            if (_playerAnimator == null)
            {
                Debug.LogError("No Animator found in target character.");
                return;
            }

            // Initialize the default override controller if not already set
            if (_defaultOverrideController == null)
                _defaultOverrideController = new AnimatorOverrideController(_playerAnimator.runtimeAnimatorController);
            
            ResetToDefaultAnimator();

            // Find the WeaponAnimationManager if not already cached
            _weaponAnimationManager ??= FindObjectOfType<WeaponAnimationManager>();
            if (_weaponAnimationManager == null)
            {
                Debug.LogError("No WeaponAnimationManager found in scene.");
                return;
            }

            // Restore the weapon animator if a weapon is stored
            var storedWeaponID = _weaponAnimationManager.GetCurrentWeaponID();
            var weaponData = Array.Find(weaponDataArray, weapon => weapon.ItemID == storedWeaponID);

            if (weaponData != null)
            {
                _playerAnimator.runtimeAnimatorController = _weaponAnimationManager.GetCurrentAnimatorController();
                Debug.Log($"Restored weapon animator for: {storedWeaponID}");
            }
        }

        public void OnMMEvent(MMGameEvent mmEvent)
        {
            if (mmEvent.EventName == "ShieldUpEvent") _playerAnimator.SetBool(ShieldUp, true);

            if (mmEvent.EventName == "ShieldDownEvent") _playerAnimator.SetBool(ShieldUp, false);
        }

        public void OnMMEvent(MMInventoryEvent mmEvent)
        {
            if (mmEvent.InventoryEventType == MMInventoryEventType.ItemEquipped)
                EquipWeapon(mmEvent.EventItem);

            if (mmEvent.InventoryEventType == MMInventoryEventType.ItemUnEquipped) ResetToDefaultAnimator();
        }

        void EquipWeapon(InventoryItem item)
        {
            if (item == null)
            {
                Debug.LogWarning("Tried to equip a null item.");
                return;
            }

            // Find matching weapon data
            var weaponData = Array.Find(weaponDataArray, weapon => weapon.ItemID == item.ItemID);
            if (weaponData == null)
            {
                Debug.LogWarning($"No weapon data found for item: {item.ItemID}");
                return;
            }

            // Update animator controller
            ApplyAnimatorOverride(weaponData.runtimeAnimatorController);

            // Store current weapon data for persistence
            _customInventoryWeapon = weaponData;
            _weaponAnimationManager?.StoreCurrentWeapon(weaponData.ItemID, weaponData.runtimeAnimatorController);

            Debug.Log($"Equipped weapon: {weaponData.ItemID}");
        }

        void ApplyAnimatorOverride(RuntimeAnimatorController overrideController)
        {
            if (overrideController == null)
            {
                Debug.LogWarning("Tried to apply a null animator override controller.");
                return;
            }

            if (_playerAnimator == null)
            {
                Debug.LogError("No animator found for the player.");
                return;
            }

            _playerAnimator.runtimeAnimatorController = overrideController;
        }


        public void ResetToDefaultAnimator()
        {
            if (_defaultOverrideController == null)
            {
                Debug.LogError("Default override controller is null. Cannot reset animator.");
                return;
            }

            ApplyAnimatorOverride(_defaultOverrideController);

            _customInventoryWeapon = null;
            _weaponAnimationManager?.ClearStoredWeapon();

        }
    }
}
