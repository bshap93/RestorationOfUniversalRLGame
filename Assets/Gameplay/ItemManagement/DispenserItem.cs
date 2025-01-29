using System;
using MoreMountains.Feedbacks;
using Plugins.TopDownEngine.ThirdParty.MoreMountains.InentoryEngine.InventoryEngine.Scripts.Items;
using Project.Gameplay.Events;
using Project.UI.HUD;
using UnityEngine;
using UnityEngine.UI;

namespace Gameplay.Player.Inventory
{
    public class DispenserItem : MonoBehaviour
    {
        [Tooltip("Unique identifier for this dispenser, ensure it is unique per scene.")]
        public string UniqueID;

        [Tooltip("The item that will be dispensed.")]
        public BaseItem DispensedItem;

        [Tooltip("The quantity dispensed per interaction.")]
        public int DispenseAmount = 1;

        [Tooltip("The total amount available in the dispenser.")]
        public int TotalSupply = 10;

        [Tooltip("UI Text or TMP Text to display stock amount.")]
        public Text stockText;

        [Header("Dispenser Models")]
        [Tooltip("Model for a full dispenser.")]
        // public GameObject FullModel;
        // [Tooltip("Model for a half-empty dispenser.")]
        // public GameObject HalfEmptyModel;
        // [Tooltip("Model for an empty dispenser.")]
        // public GameObject EmptyModel;
        [Header("Feedbacks")]
        public MMFeedbacks dispenseFeedbacks;
        public MMFeedbacks emptyFeedbacks;
        int _halfThreshold;

        bool _isInRange;
        ListPreviewManager _listPreviewManager;
        PromptManager _promptManager;
        MoreMountains.InventoryEngine.Inventory _targetInventory;

        void Awake()
        {
            if (string.IsNullOrEmpty(UniqueID))
            {
                UniqueID = Guid.NewGuid().ToString();
                Debug.LogWarning($"Generated new UniqueID for {gameObject.name}: {UniqueID}");
            }
        }

        void Start()
        {
            _promptManager = FindFirstObjectByType<PromptManager>();
            _listPreviewManager = FindFirstObjectByType<ListPreviewManager>();
            if (_promptManager == null) Debug.LogWarning("PromptManager not found in the scene.");

            var portableSystems = GameObject.Find("PortableSystems");
            if (portableSystems != null)
                _targetInventory = GameObject.FindWithTag("MainPlayerInventory")
                    ?.GetComponent<MoreMountains.InventoryEngine.Inventory>();

            if (_targetInventory == null) Debug.LogWarning("Target inventory not found in PortableSystems.");

            _halfThreshold = TotalSupply / 2;
            UpdateStockDisplay();
            // UpdateDispenserModel();
        }

        void Update()
        {
            if (_isInRange && Input.GetKeyDown(KeyCode.F)) DispenseItem();
        }

        void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                _isInRange = true;
                ItemEvent.Trigger("DispenserRangeEntered", DispensedItem, transform);

                Debug.Log("DispenserItem event triggered" + DispensedItem);

                // Call ListPreviewManager to show DispenserItemPanel
                if (_listPreviewManager == null) _listPreviewManager = FindFirstObjectByType<ListPreviewManager>();
                Debug.Log("ListPreviewManager: " + _listPreviewManager);
                if (_listPreviewManager != null) _listPreviewManager.ShowDispenserItemPreview(this);
            }
        }

        void OnTriggerExit(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                _isInRange = false;
                ItemEvent.Trigger("DispenserRangeExited", DispensedItem, transform);

                // Ensure the ListPreviewManager reference is correct
                if (_listPreviewManager == null)
                    _listPreviewManager = FindFirstObjectByType<ListPreviewManager>();

                if (_listPreviewManager != null)
                {
                    _listPreviewManager.HideDispenserItemPreview();
                    Debug.Log("Hiding DispenserItemPanel on exit.");
                }
                else
                {
                    Debug.LogWarning("ListPreviewManager not found in OnTriggerExit.");
                }
            }
        }


        void DispenseItem()
        {
            if (TotalSupply <= 0)
            {
                Debug.Log($"{gameObject.name} is out of supply!");
                emptyFeedbacks?.PlayFeedbacks();
                return;
            }

            if (_targetInventory == null || DispensedItem == null)
            {
                Debug.LogWarning("Invalid dispenser state: No inventory or item assigned.");
                return;
            }

            Debug.Log($"Dispensing {DispenseAmount} of {DispensedItem.ItemName}");

            _targetInventory.AddItem(DispensedItem, DispenseAmount);
            TotalSupply -= DispenseAmount;

            dispenseFeedbacks?.PlayFeedbacks();

            // **Trigger ItemEvent so PickupDisplayer shows it**
            ItemEvent.Trigger("ItemPickedUp", DispensedItem, transform, DispenseAmount);

            // **Ensure the UI updates even if the player moved slightly out of range**
            if (_listPreviewManager == null)
                _listPreviewManager = FindFirstObjectByType<ListPreviewManager>();

            if (_listPreviewManager != null)
                // **Update the dispenser UI dynamically**
                _listPreviewManager.DispenserItemPanel.UpdateStock(TotalSupply);

            if (TotalSupply <= 0)
            {
                Debug.Log($"{gameObject.name} has been depleted.");
                emptyFeedbacks?.PlayFeedbacks();
            }
        }

        void UpdateStockDisplay()
        {
        }

        // void UpdateDispenserModel()
        // {
        //     if (FullModel) FullModel.SetActive(TotalSupply > _halfThreshold);
        //     if (HalfEmptyModel) HalfEmptyModel.SetActive(TotalSupply > 0 && TotalSupply <= _halfThreshold);
        //     if (EmptyModel) EmptyModel.SetActive(TotalSupply <= 0);
        // }
    }
}
