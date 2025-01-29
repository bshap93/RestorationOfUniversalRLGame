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
                _promptManager?.ShowPickupPrompt();
                ItemEvent.Trigger("DispenserRangeEntered", DispensedItem, transform);
            }
        }

        void OnTriggerExit(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                _isInRange = false;
                _promptManager?.HidePickupPrompt();
                ItemEvent.Trigger("DispenserRangeExited", DispensedItem, transform);
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
            UpdateStockDisplay();
            // UpdateDispenserModel();

            // **Trigger ItemEvent so PickupDisplayer shows it**
            ItemEvent.Trigger("ItemPickedUp", DispensedItem, transform, DispenseAmount);

            if (TotalSupply <= 0)
            {
                Debug.Log($"{gameObject.name} has been depleted.");
                emptyFeedbacks?.PlayFeedbacks();
            }
        }


        void UpdateStockDisplay()
        {
            if (stockText != null) stockText.text = $"Stock: {TotalSupply}";
        }

        // void UpdateDispenserModel()
        // {
        //     if (FullModel) FullModel.SetActive(TotalSupply > _halfThreshold);
        //     if (HalfEmptyModel) HalfEmptyModel.SetActive(TotalSupply > 0 && TotalSupply <= _halfThreshold);
        //     if (EmptyModel) EmptyModel.SetActive(TotalSupply <= 0);
        // }
    }
}
