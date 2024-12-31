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

        void HandleCoinPickup(GameObject player, InventoryCoinPickup coinPickup)
        {
            Debug.Log($"[{UniqueID}] Processing coin pickup");
            var playerStats = player.GetComponent<PlayerStats>();
            if (playerStats != null)
            {
                var coinsToAdd = Random.Range(coinPickup.MinimumCoins, coinPickup.MaximumCoins + 1);
                playerStats.AddCoins(coinsToAdd);
            }

            ItemEvent.Trigger("ItemPickedUp", Item, transform);
            pickedMmFeedbacks?.PlayFeedbacks();
            Destroy(gameObject);
        }

        void HandleInventoryItemPickup()
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
                _isInRange = true; // Reset if inventory is full
                ShowInventoryFullMessage();
            }
        }

        void FinishPickup()
        {
            _promptManager?.HidePickupPrompt();
            pickedMmFeedbacks?.PlayFeedbacks();
            ItemEvent.Trigger("ItemPickedUp", Item, transform);
            Destroy(gameObject);
        }


// Update PickItem method:
        void PickItem()
        {
            if (Item == null || !_isInRange) return;

            var player = GameObject.FindWithTag("Player");
            if (player == null) return;

            var previewManager = player.GetComponent<PlayerItemPreviewManager>();
            if (previewManager == null) return;

            // Only allow pickup if this is the currently previewed item
            if (!previewManager.IsPreviewedItem(this))
            {
                Debug.Log(
                    $"[{UniqueID}] Not currently previewed item. Current preview: {previewManager.CurrentPreviewedItemPicker?.UniqueID}");

                return;
            }

            // Lock pickup state
            _isInRange = false;

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
