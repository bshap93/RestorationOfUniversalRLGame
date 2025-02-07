using Gameplay.ItemManagement.InventoryTypes;
using MoreMountains.InventoryEngine;
using MoreMountains.Tools;
using MoreMountains.TopDownEngine;
using Project.Animation_Effects.CharacterAnimation.AnimationController.WeaponAnimators;
using Project.Gameplay.Combat.Shields;
using Project.Gameplay.Combat.Tools;
using Project.Gameplay.Combat.Weapons;
using Project.Gameplay.Interactivity.Items;
using UnityEngine;
using UnityEngine.Serialization;

namespace Gameplay.SaveLoad
{
    public class InventoryPersistenceManager : MonoBehaviour, MMEventListener<MMGameEvent>,
        MMEventListener<MMCameraEvent>
    {
        public static InventoryPersistenceManager Instance;


        [Header("Inventories")] [SerializeField]
        Inventory mainInventory; // Assign your Main Inventory here
        [SerializeField] Inventory equipmentInventory; // Assign your Right Hand Inventory here
        [SerializeField] HotbarInventory hotbarInventory; // Assign your Hotbar Inventory here

        [FormerlySerializedAs("customInventoryHotbar")] [Header("Inventory Displays")] [SerializeField]
        AltCharacterHandleWeapon _altCharacterHandleWeapon;
        [SerializeField] CharacterHandleShield _characterHandleShield;
        [SerializeField] CharacterHandleTorch _characterHandleTorch;

        [SerializeField] string PlayerID = "Player1";

        InventoryItem[] _equipmentInventorySavedState;
        InventoryItem[] _hotbarInventorySavedState;
        InventoryItem[] _leftHandInventorySavedState;
        InventoryItem[] _mainInventorySavedState;


        void Awake()
        {
            if (Instance == null)
                Instance = this;
            else
                Destroy(gameObject);
        }


        void OnEnable()
        {
            // Subscribe to global save/load events
            this.MMEventStartListening<MMGameEvent>();
            this.MMEventStartListening<MMCameraEvent>();
        }

        void OnDisable()
        {
            // Unsubscribe to prevent leaks
            this.MMEventStopListening<MMGameEvent>();
            this.MMEventStopListening<MMCameraEvent>();
        }
        public void OnMMEvent(MMCameraEvent mmEvent)
        {
            if (mmEvent.EventType == MMCameraEventTypes.SetTargetCharacter)
            {
                // Debug.Log("UnEquipping items in equipment inventory...");
                // UnEquipItemsInEquipmentInventory();
            }
        }

        public void OnMMEvent(MMGameEvent mmEvent)
        {
            if (mmEvent.EventName == "SaveInventory")
            {
                Debug.Log("Saving inventories...");

                SaveInventories();
            }
        }

        static string GetSaveFilePath()
        {
            var slotPath = ES3SlotManager.selectedSlotPath;
            return string.IsNullOrEmpty(slotPath) ? "InventorySave.es3" : $"{slotPath}/InventorySave.es3";
        }

        public void SaveInventories()
        {
            // Save Main Inventory
            _mainInventorySavedState = SaveInventoryState(mainInventory);

            // Save Equipment Inventory
            _equipmentInventorySavedState = SaveInventoryState(equipmentInventory);
            _hotbarInventorySavedState = SaveInventoryState(hotbarInventory);

            mainInventory.SaveInventory();
            equipmentInventory.SaveInventory();
            hotbarInventory.SaveInventory();
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

            // Save the hotbar display slots

            return savedState;
        }

        void UnEquipItemsInEquipmentInventory()
        {
            UnEquipInventory(equipmentInventory);

            // Reset the animator to the default state
            var animatorEquipHandler = FindObjectOfType<AnimatorEquipHandler>();
            if (animatorEquipHandler != null)
                animatorEquipHandler.ResetToDefaultAnimator();
            else
                Debug.LogWarning("AnimatorEquipHandler not found during unequipping.");
        }

        void UnEquipInventory(Inventory inventory)
        {
            if (inventory == null || inventory.Content == null)
            {
                Debug.LogWarning("Inventory or Content is null, skipping...");
                return;
            }

            for (var i = 0; i < inventory.Content.Length; i++)
            {
                Debug.Log("UnEquipping item at index: " + i + ", item: " + inventory.Content[i]);
                var item = inventory.Content[i];
                if (item != null)
                {
                    var targetInventoryName = item.TargetInventoryName;
                    var targetInventory = Inventory.FindInventory(targetInventoryName, PlayerID);

                    Debug.Log("Target Inventory Name: " + targetInventoryName);
                    var freeIndex = FirstFreeIndex(targetInventory);
                    if (targetInventory != null && freeIndex != -1)
                    {
                        inventory.RemoveItemByID(item.ItemID, item.Quantity);
                        targetInventory.AddItemAt(item, item.Quantity, freeIndex);
                        Debug.Log($"Unequipped {item.ItemID} from {inventory.name} and added to {targetInventoryName}");
                    }
                    else
                    {
                        Debug.LogWarning(
                            $"Target inventory {targetInventoryName} not found so item was left in equipment inventory");
                    }
                }
                else
                {
                    Debug.LogWarning($"Null item found in {inventory.name} at position {i}");
                }
            }
        }

        int FirstFreeIndex(Inventory inventory)
        {
            for (var i = 0; i < inventory.Content.Length; i++)
                if (InventoryItem.IsNull(inventory.Content[i]))
                    return i;

            return -1;
        }


        public bool HasSavedData()
        {
            // Replace with actual file or key checks for inventory save data
            return ES3.FileExists(GetSaveFilePath());
        }

        public void ResetInventory()
        {
            Debug.Log("[InventoryPersistenceManager] Resetting all inventories to an empty state...");

            mainInventory.ResetSavedInventory();
            equipmentInventory.ResetSavedInventory();
            hotbarInventory.ResetSavedInventory();

            ES3.DeleteFile(GetSaveFilePath());

            Debug.Log("All inventories have been reset.");
        }
        public void LoadInventories()
        {
            
            
            
        }
    }
}
