using System;
using Gameplay.Player.Inventory;
using MoreMountains.Feedbacks;
using Project.Gameplay.Events;
using Project.Gameplay.Interactivity.Items;
using Project.Gameplay.ItemManagement;
using Project.Gameplay.ItemManagement.InventoryItemTypes;
using Project.UI.HUD;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

namespace Project.Gameplay.Player.Inventory
{
    public class ManualItemPicker : MonoBehaviour
    {
        [Tooltip("Unique identifier for this item, ensure it is unique per scene.")]
        public string UniqueID;
        public InventoryItem Item; // The item to be picked up
        public int Quantity = 1;


        [FormerlySerializedAs("PickedMMFeedbacks")]
        [Header("Feedbacks")]
        [Tooltip("Feedbacks to play when the item is picked up")]
        public MMFeedbacks pickedMmFeedbacks; // Feedbacks to play when the item is picked up

        bool _isBeingDestroyed;

        bool _isInRange;
        PromptManager _promptManager;
        MoreMountains.InventoryEngine.Inventory _targetInventory;

        void Awake()
        {
            if (string.IsNullOrEmpty(UniqueID))
            {
                UniqueID = Guid.NewGuid().ToString(); // Generate only if unset
                Debug.LogWarning($"Generated new UniqueID for {gameObject.name}: {UniqueID}");
            }
        }


        void Start()
        {
            if (PickableManager.IsItemPicked(UniqueID))
            {
                Destroy(gameObject); // Remove the object if already picked
                return;
            }

            _promptManager = FindFirstObjectByType<PromptManager>();
            if (_promptManager == null) Debug.LogWarning("PickupPromptManager not found in the scene.");


            // Locate PortableSystems and retrieve the appropriate inventory
            var portableSystems = GameObject.Find("PortableSystems");
            if (portableSystems != null)
            {
                if (Item.TargetInventoryName == "MainPlayerInventory")
                    _targetInventory = GameObject.FindWithTag("MainPlayerInventory")
                        ?.GetComponent<MoreMountains.InventoryEngine.Inventory>();
                else if (Item.TargetInventoryName == "HotbarInventory")
                    _targetInventory = GameObject.FindWithTag("HotbarInventory")
                        ?.GetComponent<HotbarInventory>();
                else
                    _targetInventory = GameObject.FindWithTag("MainPlayerInventory")
                        ?.GetComponent<MoreMountains.InventoryEngine.Inventory>();
            }

            if (_targetInventory == null) Debug.LogWarning("Target inventory not found in PortableSystems.");

            // Initialize feedbacks
            if (pickedMmFeedbacks != null) pickedMmFeedbacks.Initialization(gameObject);

            _promptManager = FindFirstObjectByType<PromptManager>();
        }

        void Update()
        {
            if (_isInRange && UnityEngine.Input.GetKeyDown(KeyCode.F)) PickItem();
        }

        void OnDestroy()
        {
            _isBeingDestroyed = true;
            _isInRange = false;
            enabled = false;
        }


        void OnTriggerEnter(Collider itemPickerCollider)
        {
            if (itemPickerCollider.CompareTag("Player"))
            {
                _isInRange = true;
                _promptManager?.ShowPickupPrompt();
                ItemEvent.Trigger("ItemPickupRangeEntered", Item, transform);
            }
        }

        void OnTriggerExit(Collider collider)
        {
            if (collider.CompareTag("Player"))
            {
                _isInRange = false;
                _promptManager?.HidePickupPrompt();
                ItemEvent.Trigger("ItemPickupRangeExited", Item, transform);
            }
        }

        bool HasItemBeenPicked(string uniqueID)
        {
            // Use Easy Save to check if the item was picked
            return ES3.KeyExists(uniqueID, "PickedItems.es3") && ES3.Load<bool>(uniqueID, "PickedItems.es3");
        }

        public void SetInRange(bool inRange)
        {
            _isInRange = inRange;
            if (_promptManager != null)
            {
                if (inRange)
                    _promptManager.ShowPickupPrompt();
                else
                    _promptManager.HidePickupPrompt();
            }
        }

        void HandleCoinPickup(GameObject player, InventoryCoinPickup coinPickup)
        {
            var playerStats = player.GetComponent<PlayerStats>();
            if (playerStats != null)
            {
                var coinsToAdd = Random.Range(coinPickup.MinimumCoins, coinPickup.MaximumCoins + 1);
                playerStats.AddCoins(coinsToAdd);
            }

            FinishPickup();
        }

        void HandleInventoryItemPickup()
        {
            if (_isBeingDestroyed && _targetInventory.AddItem(Item, Quantity))
            {
                FinishPickup();
            }
            else
            {
                _isBeingDestroyed = false;
                _isInRange = true;
                enabled = true;
                ShowInventoryFullMessage();
            }
        }

        void FinishPickup()
        {
            SavePickedItem(UniqueID); // Save to Easy Save and manager

            _promptManager?.HidePickupPrompt();
            ItemEvent.Trigger("ItemPickedUp", Item, transform);
            pickedMmFeedbacks?.PlayFeedbacks();

            PickableManager.PickedItems.Add(UniqueID); // Update in-memory state
            Destroy(gameObject, 0.1f);
        }

        void SavePickedItem(string uniqueID)
        {
            ES3.Save(uniqueID, true, "PickedItems.es3"); // File: PickedItems.es3
        }


        void PickItem()
        {
            Debug.Log("Picking: " + Item.ItemName);
            if (Item == null || !_isInRange || _isBeingDestroyed)
            {
                Debug.Log(
                    $"[{UniqueID}] Early exit - Item null: {Item == null}, Not in range: {!_isInRange}, Being destroyed: {_isBeingDestroyed}");

                return;
            }

            var player = GameObject.FindWithTag("Player");
            if (player == null) return;

            var previewManager = player.GetComponent<PlayerItemPreviewManager>();
            if (previewManager == null) return;

            if (!previewManager.IsPreviewedItem(this))
            {
                Debug.Log("Not previewed item.");
                return;
            }

            if (!previewManager.TryPickupItem(this))
            {
                Debug.Log("Failed to pickup item.");
                return;
            }

            _isBeingDestroyed = true;
            _isInRange = false;
            enabled = false; // Disable this component

            if (Item is InventoryCoinPickup coinPickup)
                HandleCoinPickup(player, coinPickup);
            else if (_targetInventory != null) HandleInventoryItemPickup();
        }


        void ShowInventoryFullMessage()
        {
            Debug.Log("Inventory is full or item cannot be picked up.");
            // Additional UI feedback for full inventory, if needed
        }
    }
}
