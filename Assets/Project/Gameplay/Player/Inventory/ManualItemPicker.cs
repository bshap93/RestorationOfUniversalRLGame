using System;
using MoreMountains.Feedbacks;
using MoreMountains.InventoryEngine;
using Project.Gameplay.Events;
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
        public InventoryItem Item; // The item to be picked up
        public int Quantity = 1;

        [FormerlySerializedAs("PickedMMFeedbacks")]
        [Header("Feedbacks")]
        [Tooltip("Feedbacks to play when the item is picked up")]
        public MMFeedbacks pickedMmFeedbacks; // Feedbacks to play when the item is picked up

        bool _isInRange;
        PromptManager _promptManager;
        MoreMountains.InventoryEngine.Inventory _targetInventory;
        public string UniqueID { get; set; }

        void Awake()
        {
            UniqueID = Guid.NewGuid().ToString(); // Generate a unique ID
        }

        void Start()
        {
            _promptManager = FindObjectOfType<PromptManager>();
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
        }

        void Update()
        {
            if (_isInRange && UnityEngine.Input.GetKeyDown(KeyCode.F)) PickItem();
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


        void PickItem()
        {
            if (Item == null) return;

            // Get reference to the PlayerItemPreviewManager
            var player = GameObject.FindWithTag("Player");
            if (player == null) return;

            var previewManager = player.GetComponent<PlayerItemPreviewManager>();
            if (previewManager == null) return;

            // Ensure the item being picked is the currently previewed one
            if (!previewManager.IsPreviewedItem(this))
            {
                return;
            }

            // If the item is a coin, add coins directly to PlayerStats and skip inventory
            if (Item is InventoryCoinPickup coinPickup)
            {
                var playerStats = player.GetComponent<PlayerStats>();
                if (playerStats != null)
                {
                    // Determine how many coins to add
                    var coinsToAdd = Random.Range(coinPickup.MinimumCoins, coinPickup.MaximumCoins + 1);
                    playerStats.AddCoins(coinsToAdd);
                }

                // Play feedbacks on successful pickup
                pickedMmFeedbacks?.PlayFeedbacks();

                // Destroy the item after pickup
                Destroy(gameObject);
            }
            else
            {
                // Standard inventory handling
                if (_targetInventory != null)
                {
                    if (_targetInventory.AddItem(Item, Quantity))
                    {
                        _promptManager?.HidePickupPrompt();
                        ItemEvent.Trigger("ItemPickedUp", Item, transform);
                        pickedMmFeedbacks?.PlayFeedbacks();
                        Destroy(gameObject);
                    }
                    else
                    {
                        ShowInventoryFullMessage();
                    }
                }
            }
        }


        void ShowInventoryFullMessage()
        {
            Debug.Log("Inventory is full or item cannot be picked up.");
            // Additional UI feedback for full inventory, if needed
        }
    }
}
