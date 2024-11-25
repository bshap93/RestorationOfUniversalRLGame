using MoreMountains.InventoryEngine;
using MoreMountains.Tools;
using MoreMountains.TopDownEngine;
using Project.Gameplay.Combat.Shields;
using Project.Gameplay.Combat.Tools;
using Project.Gameplay.Combat.Weapons;
using UnityEngine;

namespace Project.Gameplay.ItemManagement
{
    public class InventoryPersistenceManager : MonoBehaviour, MMEventListener<MMGameEvent>
    {
        [SerializeField] Inventory mainInventory; // Assign your Main Inventory here

        [SerializeField] Inventory equipmentInventory; // Assign your Equipment Inventory here
        InventoryItem[] _equipmentInventorySavedState;

        InventoryItem[] _mainInventorySavedState;
        Shield _savedShieldState;
        GameObject _savedTorchState;

        // New fields to track handler states
        Weapon _savedWeaponState;
        CharacterHandleShield _shieldHandler;

        CharacterHandleTorch _torchHandler;
        AltCharacterHandleWeapon _weaponHandler;


        void OnEnable()
        {
            // Subscribe to global save/load events
            this.MMEventStartListening();
        }

        void OnDisable()
        {
            // Unsubscribe to prevent leaks
            this.MMEventStopListening();
        }

        public void OnMMEvent(MMGameEvent gameEvent)
        {
            if (gameEvent.EventName == "SaveInventory")
                SaveInventories();
            else if (gameEvent.EventName == "Revert") RevertInventoriesToLastSave();
        }

        void SaveInventories()
        {
            // Save Main Inventory
            _mainInventorySavedState = SaveInventoryState(mainInventory);

            // Save Equipment Inventory
            _equipmentInventorySavedState = SaveInventoryState(equipmentInventory);

            if (_shieldHandler == null) _shieldHandler = FindObjectOfType<CharacterHandleShield>();
            if (_torchHandler == null) _torchHandler = FindObjectOfType<CharacterHandleTorch>();
            if (_weaponHandler == null) _weaponHandler = FindObjectOfType<AltCharacterHandleWeapon>();

            // Save equipped items from handlers
            _savedShieldState = _shieldHandler?.CurrentShield;
            _savedTorchState = _torchHandler?.currentTorch;


            _savedWeaponState = _weaponHandler?.CurrentWeapon;

            if (_savedWeaponState != null)
                // Equip the weapon to the player
                _weaponHandler.ChangeWeapon(_savedWeaponState, _savedWeaponState.WeaponID);

            Debug.Log("Weapon during save inv: " + _savedWeaponState);

            Debug.Log("Inventories saved.");
        }

        void RevertInventoriesToLastSave()
        {
            // Revert Main Inventory
            if (_mainInventorySavedState != null) RevertInventoryState(mainInventory, _mainInventorySavedState);

            // Revert Equipment Inventory
            if (_equipmentInventorySavedState != null)
                RevertInventoryState(equipmentInventory, _equipmentInventorySavedState);

            if (_shieldHandler == null) _shieldHandler = FindObjectOfType<CharacterHandleShield>();
            if (_torchHandler == null) _torchHandler = FindObjectOfType<CharacterHandleTorch>();
            if (_weaponHandler == null) _weaponHandler = FindObjectOfType<AltCharacterHandleWeapon>();

            Debug.Log("Inventories reverted to last saved state.");

            _shieldHandler.CurrentShield = _savedShieldState;
            _torchHandler.currentTorch = _savedTorchState;
            _weaponHandler.CurrentWeapon = _savedWeaponState;
            Debug.Log("Weapon handler:" + _weaponHandler);
            Debug.Log("Weapon current:" + _weaponHandler.CurrentWeapon);
        }

        InventoryItem[] SaveInventoryState(Inventory inventory)
        {
            var savedState = new InventoryItem[inventory.Content.Length];
            for (var i = 0; i < inventory.Content.Length; i++)
                if (!InventoryItem.IsNull(inventory.Content[i]))
                    savedState[i] = inventory.Content[i].Copy();

            Debug.Log("Inventory state saved.");

            return savedState;
        }


        void RevertInventoryState(Inventory inventory, InventoryItem[] savedState)
        {
            inventory.EmptyInventory();
            for (var i = 0; i < savedState.Length; i++)
                if (!InventoryItem.IsNull(savedState[i]))
                    inventory.AddItem(savedState[i].Copy(), savedState[i].Quantity);
        }
    }
}
