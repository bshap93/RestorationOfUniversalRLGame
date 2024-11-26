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
        [Header("Inventories")] [SerializeField]
        Inventory mainInventory; // Assign your Main Inventory here
        [SerializeField] Inventory equipmentInventory; // Assign your Equipment Inventory here


        [SerializeField] AltCharacterHandleWeapon _altCharacterHandleWeapon;
        [SerializeField] CharacterHandleShield _characterHandleShield;
        [SerializeField] CharacterHandleTorch _characterHandleTorch;

        InventoryItem[] _equipmentInventorySavedState;
        InventoryItem[] _mainInventorySavedState;


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

            ReEquipItemsInEquipmentInventory();


            Debug.Log("Inventories saved.");
        }

        Weapon SaveWeaponState(UnivWeapon weapon)
        {
            if (weapon == null) return null;
            return weapon.Copy();
        }

        void RevertInventoriesToLastSave()
        {
            // Revert Main Inventory
            if (_mainInventorySavedState != null) RevertInventoryState(mainInventory, _mainInventorySavedState);

            // Revert Equipment Inventory
            if (_equipmentInventorySavedState != null)
            {
                RevertInventoryState(equipmentInventory, _equipmentInventorySavedState);

                // For each item in the equipment inventory, equip it
                ReEquipItemsInEquipmentInventory();
            }
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

        void ReEquipItemsInEquipmentInventory()
        {
            if (_altCharacterHandleWeapon == null || _characterHandleShield == null || _characterHandleTorch == null)
            {
                _altCharacterHandleWeapon = FindObjectOfType<AltCharacterHandleWeapon>();
                _characterHandleShield = FindObjectOfType<CharacterHandleShield>();
                _characterHandleTorch = FindObjectOfType<CharacterHandleTorch>();
            }

            if (_altCharacterHandleWeapon == null || _characterHandleShield == null || _characterHandleTorch == null)
            {
                Debug.LogError("CharacterHandle components not found. Cannot re-equip items in equipment inventory.");
                return;
            }

            Debug.Log("Re-equipping items in equipment inventory...");
            for (var i = 0; i < equipmentInventory.Content.Length; i++)
            {
                if (equipmentInventory.Content[i] == null) continue;
                if (equipmentInventory.Content[i] is InventoryWeapon weapon)
                    _altCharacterHandleWeapon.ChangeWeapon(weapon.EquippableWeapon, weapon.ItemID);
                else if (equipmentInventory.Content[i] is InventoryShieldItem shield)
                    _characterHandleShield.EquipShield(shield.ShieldPrefab);
                else if (equipmentInventory.Content[i] is TorchItem torch)
                    _characterHandleTorch.EquipTorch(torch.TorchPrefab);
            }
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
