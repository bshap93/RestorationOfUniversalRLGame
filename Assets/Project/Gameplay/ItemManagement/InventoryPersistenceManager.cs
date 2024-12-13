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
        [SerializeField] Inventory rightHandInventory; // Assign your Right Hand Inventory here
        [SerializeField] Inventory leftHandInventory; // Assign your Left Hand Inventory here
        [SerializeField] HotbarInventory hotbarInventory; // Assign your Hotbar Inventory here


        [SerializeField] AltCharacterHandleWeapon _altCharacterHandleWeapon;
        [SerializeField] CharacterHandleShield _characterHandleShield;
        [SerializeField] CharacterHandleTorch _characterHandleTorch;
        InventoryItem[] _hotbarInventorySavedState;
        InventoryItem[] _leftHandInventorySavedState;
        InventoryItem[] _mainInventorySavedState;

        InventoryItem[] _rightHandInventorySavedState;


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
            _rightHandInventorySavedState = SaveInventoryState(rightHandInventory);
            _leftHandInventorySavedState = SaveInventoryState(leftHandInventory);
            _hotbarInventorySavedState = SaveInventoryState(hotbarInventory);


            ReEquipItemsInEquipmentInventory();
        }

        void RevertInventoriesToLastSave()
        {
            // Revert Main Inventory
            if (_mainInventorySavedState != null) RevertInventoryState(mainInventory, _mainInventorySavedState);

            if (_rightHandInventorySavedState != null)
                RevertInventoryState(rightHandInventory, _rightHandInventorySavedState);

            if (_leftHandInventorySavedState != null)
                RevertInventoryState(leftHandInventory, _leftHandInventorySavedState);


            if (_hotbarInventorySavedState != null) RevertInventoryState(hotbarInventory, _hotbarInventorySavedState);

            ReEquipItemsInEquipmentInventory();
        }

        InventoryItem[] SaveInventoryState(Inventory inventory)
        {
            var savedState = new InventoryItem[inventory.Content.Length];
            for (var i = 0; i < inventory.Content.Length; i++)
                if (!InventoryItem.IsNull(inventory.Content[i]))
                    savedState[i] = inventory.Content[i].Copy();


            return savedState;
        }

        InventoryItem[] SaveInventoryState(HotbarInventory inventory)
        {
            if (inventory == null || inventory.Content == null)
            {
                Debug.LogWarning("HotbarInventory is null or its Content is null");
                return new InventoryItem[0];
            }

            var savedState = new InventoryItem[inventory.Content.Length];
            for (var i = 0; i < inventory.Content.Length; i++)
                if (!InventoryItem.IsNull(inventory.Content[i]))
                    savedState[i] = inventory.Content[i].Copy();

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


            ReEquipInventory(rightHandInventory);
            ReEquipInventory(leftHandInventory);
        }
        void ReEquipInventory(Inventory equipmentUInventoryLocal)
        {
            for (var i = 0; i < rightHandInventory.Content.Length; i++)
            {
                if (equipmentUInventoryLocal.Content[i] == null) continue;
                if (equipmentUInventoryLocal.Content[i] is InventoryWeapon weapon)
                    _altCharacterHandleWeapon.ChangeWeapon(weapon.EquippableWeapon, weapon.ItemID);
                else if (equipmentUInventoryLocal.Content[i] is InventoryShieldItem shield)
                    _characterHandleShield.EquipShield(shield.ShieldPrefab);
                else if (equipmentUInventoryLocal.Content[i] is TorchItem torch)
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

        void RevertInventoryState(HotbarInventory inventory, InventoryItem[] savedState)
        {
            if (inventory == null || savedState == null)
            {
                Debug.LogWarning("HotbarInventory or savedState is null");
                return;
            }

            inventory.EmptyInventory();
            for (var i = 0; i < savedState.Length; i++)
                if (!InventoryItem.IsNull(savedState[i]))
                {
                    var success = inventory.AddItem(savedState[i].Copy(), savedState[i].Quantity);
                    if (!success) Debug.LogWarning($"Failed to add item {savedState[i].ItemID} at index {i}");
                }
        }
    }
}
