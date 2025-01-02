using System;
using System.Collections.Generic;
using MoreMountains.Feedbacks;
using MoreMountains.InventoryEngine;
using MoreMountains.Tools;
using Project.Gameplay.Interactivity.Items;
using Project.Gameplay.ItemManagement;
using Project.Gameplay.Player.Interaction;
using Project.UI.HUD;
using UnityEngine;
using UnityEngine.Serialization;

namespace Project.Gameplay.Interactivity.CraftingStation
{
    public class ManualCraftingStationInteract : ManualInteractablePicker
    {
        [FormerlySerializedAs("CraftingStation")]
        public CraftingStation craftingStation;

        [FormerlySerializedAs("pickedMmFeedbacks")]
        [Header("Feedbacks")]
        [Tooltip("Feedbacks to play when the item is picked up")]
        public MMFeedbacks initialInteractionFeedbacks; // Feedbacks to play when the item is picked up
        

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
            var portableSystems = GameObject.Find("PortableSystems");
            if (portableSystems != null)
            {
                if (craftingStation.TargetInventoryName == "MainPlayerInventory")
                    _targetInventory = GameObject.FindWithTag("MainPlayerInventory")
                        ?.GetComponent<Inventory>();
                else if (craftingStation.TargetInventoryName == "HotbarInventory")
                    _targetInventory = GameObject.FindWithTag("HotbarInventory")
                        ?.GetComponent<HotbarInventory>();
                else
                    _targetInventory = GameObject.FindWithTag("MainPlayerInventory")
                        ?.GetComponent<Inventory>();


                if (craftingStation.SourceInventoryName == craftingStation.TargetInventoryName)
                {
                    _sourceInventory = _targetInventory;
                }
                else
                {
                    if (craftingStation.SourceInventoryName == "MainPlayerInventory")
                        _sourceInventory = GameObject.FindWithTag("MainPlayerInventory")
                            ?.GetComponent<Inventory>();
                    else if (craftingStation.SourceInventoryName == "HotbarInventory")
                        _sourceInventory = GameObject.FindWithTag("HotbarInventory")
                            ?.GetComponent<HotbarInventory>();
                    else
                        _sourceInventory = GameObject.FindWithTag("MainPlayerInventory")
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
            if (_isInRange && UnityEngine.Input.GetKeyDown(KeyCode.F)) StartInteractionWithCraftingStation();
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
                    stringParameter: craftingStation.CraftingStationId, vector3Parameter: transform.position);
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
                    stringParameter: craftingStation.CraftingStationId, vector3Parameter: transform.position);
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
            var initialActionItem = craftingStation.InitialActivationResources;
            var indices = IndicesWithSpecifiedItem(_sourceInventory, initialActionItem.ActivationItem);

            for (var i = 0; i < indices.Count; i++)
                if (_sourceInventory.RemoveItem(i, 1))
                {
                    Debug.Log("Initial action item removed");
                }
                else
                {
                    Debug.Log("Player lacks the initial action item");
                    return;
                }

            FinishInitialInteraction();
        }

        List<int> IndicesWithSpecifiedItem(Inventory inventory, InventoryItem item)
        {
            var indices = new List<int>();
            for (var i = 0; i < inventory.Content.Length; i++)
                if (inventory.Content[i].ItemID == item.ItemID)
                    indices.Add(i);

            return indices;
        }

        void StartInteractionWithCraftingStation()
        {
            if (craftingStation == null || !_isInRange)
            {
                Debug.Log(
                    $"[{UniqueID}] Early exit - CraftingStation null: {craftingStation == null}, Not in range: {!_isInRange}");

                return;
            }

            var player = GameObject.FindWithTag("Player");
            if (player == null)
            {
                Debug.Log("Player not found");
                return;
            }

            var craftingStationPreviewManager = player.GetComponent<CraftingStationPreviewManager>();
            if (craftingStationPreviewManager == null)
            {
                Debug.Log("Preview manager not found");
                return;
            }

            if (!craftingStationPreviewManager.IsPreviewedCraftingStation(this))
            {
                Debug.Log("Failed to interact with crafting station");
                return;
            }

            if (!craftingStationPreviewManager.TryBeginInteraction(this))
            {
                Debug.Log("Failed to interact with crafting station");
                return;
            }

            HandleCraftingStationInitialAction();
        }

        void FinishInitialInteraction()
        {
            craftingStation.Interact();
            _promptManager?.HidePickupPrompt();
            MMGameEvent.Trigger("CraftingStationActivated", stringParameter: craftingStation.CraftingStationId);
            initialInteractionFeedbacks?.PlayFeedbacks();
        }
    }
}
