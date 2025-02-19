using System;
using System.Collections;
using Gameplay.ItemManagement.InventoryTypes;
using MoreMountains.InventoryEngine;
using MoreMountains.Tools;
using Project.Gameplay.Combat.Shields;
using Project.Gameplay.Combat.Tools;
using Project.Gameplay.Combat.Weapons;
using UnityEngine;
using UnityEngine.Serialization;

namespace Gameplay.SaveLoad
{
    [Serializable]
    public class SerializedInventoryES3
    {
        public string[] ItemIDs;
        public int[] Quantities;

        public SerializedInventoryES3(InventoryItem[] content)
        {
            ItemIDs = new string[content.Length];
            Quantities = new int[content.Length];

            for (var i = 0; i < content.Length; i++)
                if (!InventoryItem.IsNull(content[i]))
                {
                    ItemIDs[i] = content[i].ItemID;
                    Quantities[i] = content[i].Quantity;
                }
                else
                {
                    ItemIDs[i] = null;
                    Quantities[i] = 0;
                }
        }
    }

    public class InventoryPersistenceManager : MonoBehaviour, MMEventListener<MMGameEvent>,
        MMEventListener<MMSceneLoadingManager.LoadingSceneEvent>

    {
        const string MAIN_INVENTORY_KEY = "MainInventory";
        const string EQUIPMENT_INVENTORY_KEY = "EquipmentInventory";
        const string HOTBAR_INVENTORY_KEY = "HotbarInventory";
        const string RESOURCE_PATH = "Items/";
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
        InventoryItem[] _mainInventorySavedState;


        void Awake()
        {
            if (Instance == null)
                Instance = this;
            else
                Destroy(gameObject);
        }

        void Start()
        {
            // Add a delay to ensure other systems are initialized
            StartCoroutine(LoadInventoriesWithDelay());
        }


        void OnEnable()
        {
            // Existing subscriptions
            this.MMEventStartListening<MMGameEvent>();
            this.MMEventStartListening<MMSceneLoadingManager.LoadingSceneEvent>();
        }

        void OnDisable()
        {
            // Existing unsubscriptions
            this.MMEventStopListening<MMGameEvent>();
            this.MMEventStopListening<MMSceneLoadingManager.LoadingSceneEvent>();
        }
        void OnApplicationQuit()
        {
            SaveInventories();
        }

        public void OnMMEvent(MMSceneLoadingManager.LoadingSceneEvent loadingEvent)
        {
            // We want to reload after the transition is complete
            if (loadingEvent.Status == MMSceneLoadingManager.LoadingStatus.LoadTransitionComplete)
                StartCoroutine(ReloadInventoriesAfterDelay());

            if (loadingEvent.Status == MMSceneLoadingManager.LoadingStatus.LoadStarted)
                SaveInventories();
        }


        public void OnMMEvent(MMGameEvent mmEvent)
        {
            if (mmEvent.EventName == "SaveInventory")
            {
                Debug.Log("Saving inventories...");

                SaveInventories();
            }
        }

        IEnumerator ReloadInventoriesAfterDelay()
        {
            // Wait for 2 frames to ensure character initialization
            yield return null;
            yield return null;

            // Clear and reload inventories
            LoadInventories();
        }

        IEnumerator LoadInventoriesWithDelay()
        {
            // Wait for 2 frames to ensure character initialization
            yield return null;
            yield return null;

            LoadInventories();
        }

        static string GetSaveFilePath()
        {
            var slotPath = ES3SlotManager.selectedSlotPath;
            return string.IsNullOrEmpty(slotPath) ? "InventorySave.es3" : $"{slotPath}/InventorySave.es3";
        }

        public void SaveInventories()
        {
            var savePath = GetSaveFilePath();

            // Save each inventory's state
            var mainInventoryData = new SerializedInventoryES3(mainInventory.Content);
            var equipmentInventoryData = new SerializedInventoryES3(equipmentInventory.Content);
            var hotbarInventoryData = new SerializedInventoryES3(hotbarInventory.Content);

            // Save to Easy Save
            ES3.Save(MAIN_INVENTORY_KEY, mainInventoryData, savePath);
            ES3.Save(EQUIPMENT_INVENTORY_KEY, equipmentInventoryData, savePath);
            ES3.Save(HOTBAR_INVENTORY_KEY, hotbarInventoryData, savePath);

            Debug.Log($"Inventories saved successfully to {savePath}");
        }


        public bool HasSavedData()
        {
            return ES3.FileExists(GetSaveFilePath());
        }

        public void ResetInventory()
        {
            Debug.Log("[InventoryPersistenceManager] Resetting all inventories to an empty state...");


            ES3.Save(
                MAIN_INVENTORY_KEY, new SerializedInventoryES3(new InventoryItem[20]),
                GetSaveFilePath());

            ES3.Save(
                EQUIPMENT_INVENTORY_KEY, new SerializedInventoryES3(new InventoryItem[1]),
                GetSaveFilePath());

            ES3.Save(
                HOTBAR_INVENTORY_KEY, new SerializedInventoryES3(new InventoryItem[10]),
                GetSaveFilePath());


            Debug.Log("All inventories have been reset.");
        }
        public void LoadInventories()
        {
            var savePath = GetSaveFilePath();

            if (!ES3.FileExists(savePath))
            {
                Debug.LogWarning($"No save file found at {savePath}");
                return;
            }

            try
            {
                // Load each inventory's serialized data
                var mainInventoryData = ES3.Load<SerializedInventoryES3>(MAIN_INVENTORY_KEY, savePath);
                var equipmentInventoryData = ES3.Load<SerializedInventoryES3>(EQUIPMENT_INVENTORY_KEY, savePath);
                var hotbarInventoryData = ES3.Load<SerializedInventoryES3>(HOTBAR_INVENTORY_KEY, savePath);

                // Clear current inventories
                mainInventory.EmptyInventory();
                equipmentInventory.EmptyInventory();
                hotbarInventory.EmptyInventory();

                // Restore main inventory
                RestoreInventory(mainInventory, mainInventoryData);
                RestoreInventory(equipmentInventory, equipmentInventoryData);
                RestoreInventory(hotbarInventory, hotbarInventoryData);

                // Trigger events to notify that inventories have been loaded
                MMInventoryEvent.Trigger(
                    MMInventoryEventType.InventoryLoaded, null, mainInventory.name, null, 0, 0, PlayerID);

                MMInventoryEvent.Trigger(
                    MMInventoryEventType.InventoryLoaded, null, equipmentInventory.name, null, 0, 0, PlayerID);

                MMInventoryEvent.Trigger(
                    MMInventoryEventType.InventoryLoaded, null, hotbarInventory.name, null, 0, 0, PlayerID);

                Debug.Log("Inventories loaded successfully");
            }
            catch (Exception e)
            {
                Debug.LogError($"Error loading inventories: {e.Message}");
            }
        }

        void RestoreInventory(Inventory inventory, SerializedInventoryES3 serializedData)
        {
            if (serializedData == null || serializedData.ItemIDs == null) return;

            for (var i = 0; i < serializedData.ItemIDs.Length; i++)
            {
                if (string.IsNullOrEmpty(serializedData.ItemIDs[i])) continue;


                // Load the item from Resources
                var itemPrefab = Resources.Load<InventoryItem>($"{RESOURCE_PATH}{serializedData.ItemIDs[i]}");
                if (itemPrefab == null)
                {
                    Debug.LogError($"Failed to load item: {serializedData.ItemIDs[i]}");
                    continue;
                }

                // Create a copy and set its quantity
                var item = itemPrefab.Copy();
                item.Quantity = serializedData.Quantities[i];

                // Add to inventory at specific slot
                inventory.AddItemAt(item, item.Quantity, i);

                // If this is an equipment inventory, we need to actually equip the item
                if (inventory.InventoryType == Inventory.InventoryTypes.Equipment)
                {
                    Debug.Log($"Triggering equip for item {item.ItemID}");
                    // Trigger Equip on the item itself
                    item.Equip(PlayerID);

                    // Also trigger the equip event
                    StartCoroutine(TriggerEquipWithDelay(inventory, item, i));
                }
            }
        }

        IEnumerator TriggerEquipWithDelay(Inventory inventory, InventoryItem item, int index)
        {
            yield return new WaitForSeconds(0.5f); // Small delay to ensure systems are ready
            MMInventoryEvent.Trigger(
                MMInventoryEventType.ItemEquipped,
                null,
                inventory.name,
                item,
                item.Quantity,
                index,
                PlayerID
            );
        }
    }
}
