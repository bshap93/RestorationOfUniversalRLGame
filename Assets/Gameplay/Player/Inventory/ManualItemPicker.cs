using System;
using Core.GameInitialization;
using Gameplay.ItemManagement.Cooking;
using Gameplay.ItemManagement.InventoryItemTypes;
using Gameplay.ItemManagement.InventoryTypes;
using Gameplay.ItemsInteractions;
using MoreMountains.Feedbacks;
using Plugins.TopDownEngine.ThirdParty.MoreMountains.InentoryEngine.InventoryEngine.Scripts.Items;
using Project.Gameplay.Events;
using Project.Gameplay.ItemManagement;
using Project.Gameplay.ItemManagement.InventoryItemTypes.Books;
using Project.Gameplay.Player;
using Project.UI.HUD;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

namespace Gameplay.Player.Inventory
{
    public class ManualItemPicker : MonoBehaviour
    {
        [Tooltip("Unique identifier for this item, ensure it is unique per scene.")]
        public string UniqueID;
        public BaseItem Item; // The item to be picked up
        public int Quantity = 1;


        [FormerlySerializedAs("PickedMMFeedbacks")]
        [Header("Feedbacks")]
        [Tooltip("Feedbacks to play when the item is picked up")]
        public MMFeedbacks pickedMmFeedbacks; // Feedbacks to play when the item is picked up
        [Tooltip("Feedbacks to play when the item is used")]
        public MMFeedbacks usedMmFeedbacks; // Feedbacks to play when the item is used
        public MMFeedbacks disappearMmFeedbacks; // Feedbacks to play when the item disappears
        public bool NotPickable; // If true, the item cannot be picked up

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
            var portableSystems = GameObject.Find(PortableSystems.PortableSystemsObjectName);
            if (portableSystems != null)
            {
                if (Item.TargetInventoryName == MainInventory.MainInventoryObjectName)
                    _targetInventory = GameObject.FindWithTag(MainInventory.MainInventoryObjectName)
                        ?.GetComponent<MoreMountains.InventoryEngine.Inventory>();
                else if (Item.TargetInventoryName == HotbarInventory.HotbarInventoryObjectName)
                    _targetInventory = GameObject.FindWithTag(HotbarInventory.HotbarInventoryObjectName)
                        ?.GetComponent<HotbarInventory>();
                else
                    _targetInventory = GameObject.FindWithTag(MainInventory.MainInventoryObjectName)
                        ?.GetComponent<MoreMountains.InventoryEngine.Inventory>();
            }

            if (_targetInventory == null) Debug.LogWarning("Target inventory not found in PortableSystems.");

            // Initialize feedbacks
            if (pickedMmFeedbacks != null) pickedMmFeedbacks.Initialization(gameObject);

            _promptManager = FindFirstObjectByType<PromptManager>();
        }

        void Update()
        {
            if (_isInRange && Input.GetKeyDown(KeyCode.F)) PickItem();
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
            // Use the new PickableManager method instead of direct ES3 calls
            PickableManager.SavePickedItem(uniqueID, true);
            Debug.Log($"Saved picked item: {uniqueID}");
        }
        public void PickItem()
        {
            if (Item == null || !_isInRange || _isBeingDestroyed)
            {
                Debug.LogWarning($"Cannot interact with item: {Item?.ItemName ?? "null"}");
                return;
            }

            if (NotPickable)
            {
                Debug.Log($"Item {Item.ItemName} is not pickable. Attempting to use instead.");
                UseItem();
                return;
            }

            Debug.Log($"Picking up: {Item.ItemName}");

            var player = GameObject.FindWithTag("Player");
            if (player == null) return;

            var previewManager = player.GetComponent<PlayerItemListPreviewManager>();
            if (previewManager == null) return;

            if (!previewManager.IsPreviewedItem(this)) return;

            if (!previewManager.TryPickupItem(this)) return;

            previewManager.RemoveFromItemListPreview(Item);
            previewManager.RefreshPreviewOrder();

            _isBeingDestroyed = true;
            _isInRange = false;
            enabled = false;

            if (Item is InventoryCoinPickup coinPickup)
                HandleCoinPickup(player, coinPickup);
            else if (_targetInventory != null) HandleInventoryItemPickup();
        }

        public void UseItem()
        {
            Debug.Log($"Using item: {Item?.ItemName}");

            // Play the feedbacks for using the item
            usedMmFeedbacks?.PlayFeedbacks();

            // Perform the item's use logic
            Item?.Use("Player1");

            // If it's a cookbook, show the recipe learning displayer
            if (Item is InventoryCookBook cookbook) DisplayLearnedRecipes(cookbook);

            // Optional: Destroy the item if it should disappear after use
            if (Item?.DisappearAfterUse == true)
            {
                disappearMmFeedbacks?.PlayFeedbacks();
                Destroy(gameObject, 0.1f);
            }
        }

        void DisplayLearnedRecipes(InventoryCookBook cookbook)
        {
            // Show a recipe learning display similar to the PickupDisplayer
            var displayer = FindFirstObjectByType<RecipeDisplayer>();
            if (displayer != null)
                displayer.DisplayLearnedRecipes(cookbook.CookingRecipes);
            else
                Debug.LogWarning("No RecipeDisplayer found in the scene.");
        }


        void ShowInventoryFullMessage()
        {
            Debug.Log("Inventory is full or item cannot be picked up.");
            // Additional UI feedback for full inventory, if needed
        }
    }
}
