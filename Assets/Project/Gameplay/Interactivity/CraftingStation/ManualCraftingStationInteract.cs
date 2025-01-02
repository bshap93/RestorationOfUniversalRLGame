using System;
using MoreMountains.Feedbacks;
using MoreMountains.InventoryEngine;
using MoreMountains.Tools;
using Project.Gameplay.ItemManagement;
using Project.UI.HUD;
using UnityEngine;

namespace Project.Gameplay.Interactivity.CraftingStation
{
    public class ManualCraftingStationInteract : MonoBehaviour
    {
        public CraftingStation CraftingStation;

        [Header("Feedbacks")] [Tooltip("Feedbacks to play when the item is picked up")]
        public MMFeedbacks pickedMmFeedbacks; // Feedbacks to play when the item is picked up

        bool _isInRange;
        PromptManager _promptManager;

        Inventory _sourceInventory;
        Inventory _targetInventory;

        public string UniqueID { get; set; }

        void Awake()
        {
            UniqueID = Guid.NewGuid().ToString(); // Generate a unique ID
        }

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
                if (CraftingStation.TargetInventoryName == "MainPlayerInventory")
                    _targetInventory = GameObject.FindWithTag("MainPlayerInventory")
                        ?.GetComponent<Inventory>();
                else if (CraftingStation.TargetInventoryName == "HotbarInventory")
                    _targetInventory = GameObject.FindWithTag("HotbarInventory")
                        ?.GetComponent<HotbarInventory>();
                else
                    _targetInventory = GameObject.FindWithTag("MainPlayerInventory")
                        ?.GetComponent<Inventory>();


                if (CraftingStation.SourceInventoryName == CraftingStation.TargetInventoryName)
                {
                    _sourceInventory = _targetInventory;
                }
                else
                {
                    if (CraftingStation.SourceInventoryName == "MainPlayerInventory")
                        _sourceInventory = GameObject.FindWithTag("MainPlayerInventory")
                            ?.GetComponent<Inventory>();
                    else if (CraftingStation.SourceInventoryName == "HotbarInventory")
                        _sourceInventory = GameObject.FindWithTag("HotbarInventory")
                            ?.GetComponent<HotbarInventory>();
                    else
                        _sourceInventory = GameObject.FindWithTag("MainPlayerInventory")
                            ?.GetComponent<Inventory>();
                }

                if (_targetInventory == null) Debug.LogWarning("Target inventory not found in PortableSystems.");
                if (_sourceInventory == null) Debug.LogWarning("Source inventory not found in PortableSystems.");

                // Initialize feedbacks
                if (pickedMmFeedbacks != null) pickedMmFeedbacks.Initialization(gameObject);
            }
        }
        void Update()
        {
            if (_isInRange && UnityEngine.Input.GetKeyDown(KeyCode.F)) Interact();
        }

        void OnDestroy()
        {
            _isInRange = false;
            enabled = false;
        }

        void OnTriggerEnter(Collider itemPickerCollider)
        {
            if (itemPickerCollider.CompareTag("Player"))
            {
                _isInRange = true;
                _promptManager?.ShowInteractPrompt("Press F to interact");
                MMGameEvent.Trigger(
                    "CraftingStationRangeEntered",
                    stringParameter: CraftingStation.CraftingStationId, vector3Parameter: transform.position);
            }
        }

        void OnTriggerExit(Collider collider)
        {
            if (collider.CompareTag("Player"))
            {
                _isInRange = false;
                _promptManager?.HideInteractPrompt();
                MMGameEvent.Trigger(
                    "CraftingStationRangeExited",
                    stringParameter: CraftingStation.CraftingStationId, vector3Parameter: transform.position);
            }
        }

        void Interact()
        {
            CraftingStation.Interact();
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
    }
}
