using System.Collections.Generic;
using Core.GameInitialization;
using Gameplay.ItemManagement.InventoryTypes;
using MoreMountains.Feedbacks;
using MoreMountains.InventoryEngine;
using MoreMountains.Tools;
using Project.Gameplay.Interactivity;
using Project.Gameplay.ItemManagement.InventoryTypes.Cooking;
using Project.UI.HUD;
using UnityEngine;
using UnityEngine.Serialization;

namespace Gameplay.ItemsInteractions.CraftingStation
{
    public class ManualCraftingStationInteract : ManualInteractablePicker
    {
        [FormerlySerializedAs("craftingStation")] [FormerlySerializedAs("CraftingStation")]
        public CookingStation cookingStation;

        [FormerlySerializedAs("pickedMmFeedbacks")]
        [Header("Feedbacks")]
        [Tooltip("Feedbacks to play when the item is picked up")]
        public MMFeedbacks initialInteractionFeedbacks; // Feedbacks to play when the item is picked up
        public MMFeedbacks
            playerActivatesCraftingStationFeedbacks; // Feedbacks to play when the player activates the crafting station
        public MMFeedbacks
            playerLacksInitialResourcesFeedbacks; // Feedbacks to play when the player lacks the initial resources


        bool _isInRange;
        PromptManager _promptManager;

        Inventory _sourceInventory;
        Inventory _targetInventory;


        void Start()
        {
            _promptManager = FindObjectOfType<PromptManager>();
            if (_promptManager == null) Debug.LogWarning("Prompt manager not found in the scene.");
            if (_promptManager.InteractPromptUI == null)
                Debug.LogWarning("InteractPromptUI not found in the PromptManager.");

            // Locate PortableSystems and retrieve the appropriate inventory
            var portableSystems = GameObject.Find(PortableSystems.PortableSystemsObjectName);
            if (portableSystems != null)
            {
                if (cookingStation.TargetInventoryName == MainInventory.MainInventoryObjectName)
                    _targetInventory = GameObject.FindWithTag(MainInventory.MainInventoryTag)
                        ?.GetComponent<Inventory>();
                else if (cookingStation.TargetInventoryName == HotbarInventory.HotbarInventoryObjectName)
                    _targetInventory = GameObject.FindWithTag(HotbarInventory.HotbarInventoryTag)
                        ?.GetComponent<HotbarInventory>();
                else
                    _targetInventory = GameObject.FindWithTag(MainInventory.MainInventoryTag)
                        ?.GetComponent<Inventory>();


                if (cookingStation.SourceInventoryName == cookingStation.TargetInventoryName)
                {
                    _sourceInventory = _targetInventory;
                }
                else
                {
                    if (cookingStation.SourceInventoryName == MainInventory.MainInventoryObjectName)
                        _sourceInventory = GameObject.FindWithTag(MainInventory.MainInventoryTag)
                            ?.GetComponent<Inventory>();
                    else if (cookingStation.SourceInventoryName == HotbarInventory.HotbarInventoryObjectName)
                        _sourceInventory = GameObject.FindWithTag(HotbarInventory.HotbarInventoryTag)
                            ?.GetComponent<HotbarInventory>();
                    else
                        _sourceInventory = GameObject.FindWithTag(MainInventory.MainInventoryTag)
                            ?.GetComponent<Inventory>();
                }

                if (_targetInventory == null) Debug.LogWarning("Target inventory not found in PortableSystems.");
                if (_sourceInventory == null) Debug.LogWarning("Source inventory not found in PortableSystems.");

                // Initialize feedbacks
                if (initialInteractionFeedbacks != null) initialInteractionFeedbacks.Initialization(gameObject);
            }
        }
        void Update()
        {
            if (_isInRange && Input.GetKeyDown(KeyCode.F)) StartInteractionWithCraftingStation();
        }

        void OnDestroy()
        {
            _isInRange = false;
            enabled = false;
        }

        void OnTriggerEnter(Collider collider0)
        {
            if (collider0.CompareTag("Player"))
            {
                _isInRange = true;
                _promptManager?.ShowInteractPrompt("Press F to interact");
                MMGameEvent.Trigger(
                    "CraftingStationRangeEntered",
                    stringParameter: cookingStation.CraftingStationId, vector3Parameter: transform.position);
            }
        }

        void OnTriggerExit(Collider collider0)
        {
            if (collider0.CompareTag("Player"))
            {
                _isInRange = false;
                _promptManager?.HideInteractPrompt();
                MMGameEvent.Trigger(
                    "CraftingStationRangeExited",
                    stringParameter: cookingStation.CraftingStationId, vector3Parameter: transform.position);
            }
        }

        public void SetInRange(bool inRange)
        {
            _isInRange = inRange;
            if (_promptManager != null)
            {
                if (inRange)
                    _promptManager.ShowInteractPrompt("Press F to interact");
                else
                    _promptManager.HideInteractPrompt();
            }
        }

        void HandleCraftingStationInitialAction()
        {
            // Check if crafting station is properly set up
            if (cookingStation == null)
            {
                Debug.LogError($"[{UniqueID}] CraftingStation is null");
                return;
            }

            // Check if initial activation resources are set up
            if (cookingStation.InitialActivationResources == null)
            {
                Debug.LogError($"[{UniqueID}] InitialActivationResources is null");
                return;
            }

            // Check if activation item is set up
            if (cookingStation.InitialActivationResources.ActivationItem == null)
            {
                Debug.LogError($"[{UniqueID}] ActivationItem is null");
                return;
            }

            // Check if source inventory is set up
            if (_sourceInventory == null)
            {
                Debug.LogError($"[{UniqueID}] Source inventory is null");
                return;
            }

            Debug.Log(
                $"[{UniqueID}] Checking for activation item: {cookingStation.InitialActivationResources.ActivationItem.ItemID}");

            var initialActionItem = cookingStation.InitialActivationResources;
            var indices = IndicesWithSpecifiedItem(_sourceInventory, initialActionItem.ActivationItem);

            if (indices == null || indices.Count == 0)
            {
                Debug.Log($"[{UniqueID}] Player lacks the initial action item");
                playerLacksInitialResourcesFeedbacks?.PlayFeedbacks();
                return;
            }

            foreach (var index in indices)
                if (_sourceInventory.RemoveItem(index, 1))
                {
                    Debug.Log($"[{UniqueID}] Initial action item removed from index {index}");
                }
                else
                {
                    Debug.Log($"[{UniqueID}] Failed to remove item at index {index}");
                    playerLacksInitialResourcesFeedbacks?.PlayFeedbacks();
                    return;
                }

            FinishInitialInteraction();
        }

        List<int> IndicesWithSpecifiedItem(Inventory inventory, InventoryItem item)
        {
            var indices = new List<int>();

            // Check if inventory is null
            if (inventory == null)
            {
                Debug.LogError($"[{UniqueID}] Inventory is null");
                return indices;
            }

            // Check if item is null
            if (item == null)
            {
                Debug.LogError($"[{UniqueID}] Item to check for is null");
                return indices;
            }

            // Check if inventory content is null
            if (inventory.Content == null)
            {
                Debug.LogError($"[{UniqueID}] Inventory content is null");
                return indices;
            }

            Debug.Log($"[{UniqueID}] Checking inventory {inventory.name} for item {item.ItemID}");
            Debug.Log($"[{UniqueID}] Inventory content length: {inventory.Content.Length}");

            for (var i = 0; i < inventory.Content.Length; i++)
                if (inventory.Content[i] != null && inventory.Content[i].ItemID == item.ItemID)
                {
                    indices.Add(i);
                    Debug.Log($"[{UniqueID}] Found matching item at index {i}");
                }

            Debug.Log($"[{UniqueID}] Found {indices.Count} matching items");
            return indices;
        }

        void StartInteractionWithCraftingStation()
        {
            if (cookingStation == null || !_isInRange)
            {
                Debug.Log(
                    $"[{UniqueID}] Early exit - CraftingStation null: {cookingStation == null}, Not in range: {!_isInRange}");

                return;
            }

            var player = GameObject.FindWithTag("Player");
            if (player == null)
            {
                Debug.Log("Player not found");
                return;
            }


            initialInteractionFeedbacks?.PlayFeedbacks();

            HandleCraftingStationInitialAction();
        }

        void FinishInitialInteraction()
        {
            cookingStation.Interact();
            _promptManager?.HidePickupPrompt();
            MMGameEvent.Trigger("CraftingStationActivated", stringParameter: cookingStation.CraftingStationId);
            playerActivatesCraftingStationFeedbacks?.PlayFeedbacks();
        }
    }
}
