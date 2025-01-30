using Gameplay.ItemManagement.InventoryTypes;
using JetBrains.Annotations;
using MoreMountains.Feedbacks;
using MoreMountains.InventoryEngine;
using MoreMountains.Tools;
using Project.Core.Events;
using Project.Gameplay.Interactivity.Items;
using Project.Gameplay.ItemManagement.InventoryTypes.Cooking;
using Project.Gameplay.ItemManagement.InventoryTypes.Fuel;
using Project.Gameplay.SaveLoad.Triggers;
using TMPro;
using UnityEngine;

namespace Gameplay.Crafting.Cooking
{
    public class CookingStationController : MonoBehaviour, ISelectableTrigger, MMEventListener<CookingStationEvent>
    {
        CookingQueueInventory _queueInventory; // Uncooked items
        CookingDepositInventory _depositInventory; // Cooked items
        FuelInventory _fuelInventory; // Firewood

        public CanvasGroup CookingUICanvasGroup;
        public CanvasGroup CookingUIPanelCanvasGroup;
        public GameObject CookingUIPanel;


        [CanBeNull] public FuelMaterial FuelItemAlreadyAdded;

        public CookingStation CookingStation;


        [Header("Fuel & Progress Tracking")] public float fuelBurnRate = 1f; // Time in seconds to burn one unit of fuel


        public TextMeshProUGUI previewText;
        public MMFeedbacks interactFeedbacks;
        public MMFeedbacks craftingFeedbacks;
        public MMFeedbacks completionFeedbacks;
        public MMFeedbacks newRecipeSetFeedbacks;
        CookingRecipe _currentRecipe; // The recipe to craft

        bool _isCrafting;

        bool _isInPlayerRange;
        Inventory _playerInventory; // Reference to the player's inventory


        void Start()
        {
            InitializeInventories();
            HideCookingUI();

            if (_fuelInventory.GetQuantity("Firewood") > 0)
                FuelItemAlreadyAdded =
                    new FuelMaterial(
                        _fuelInventory.fuelItemAllowed, _fuelInventory.GetQuantity(_fuelInventory.fuelItemAllowed.ItemID));
            else
                FuelItemAlreadyAdded = null;


            if (FuelItemAlreadyAdded != null)
                _fuelInventory.TreatAddedItem(
                    FuelItemAlreadyAdded.FuelItem.Item,
                    FuelItemAlreadyAdded.Quantity);
        }

        void OnEnable()
        {
            InitializeInventories();

            this.MMEventStartListening();
        }

        void OnDisable()
        {
            this.MMEventStopListening();
        }

        void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                _isInPlayerRange = true;

                if (CookingStation.CraftingStationId == null)
                {
                    Debug.LogError("CraftingStationId is null");
                    return;
                }


                CookingStationEvent.Trigger("CookingStationInRange", CookingStationEventType.CookingStationInRange, this);

                // Display the inventories when interacting.

                if (!ValidateFuel())
                {
                    ShowPreview("No fuel available. Deposit fuel to start cooking.");
                    // Prompt the player to deposit fuel
                    PromptFuelDeposit();
                }

                ShowPreview("Press F to interact with the Cooking Station");
            }
        }


        void OnTriggerExit(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                _isInPlayerRange = false;
                CookingStationEvent.Trigger(
                    "CookingStationOutOfRange",
                    CookingStationEventType.CookingStationOutOfRange,
                    this);

                HidePreview();
            }
        }
        public void OnSelectedItem()
        {
            CookingStationEvent.Trigger("CookingStationSelected", CookingStationEventType.CookingStationSelected, this);
        }
        public void OnUnSelectedItem()
        {
            CookingStationEvent.Trigger("CookingStationDeselected", CookingStationEventType.CookingStationDeselected, this);
        }
        public void OnMMEvent(CookingStationEvent mmEvent)
        {
            if (mmEvent.EventType == CookingStationEventType.TryAddFuel)
            {
                Debug.Log("Received TryAddFuel event");
                TryAddFuel();
            }
        }
        public void TryAddFuel()
        {
            if (_isInPlayerRange)
                if (!ValidateFuel())
                {
                    // Reinitialize inventories if reference is lost
                    if (_playerInventory == null) InitializeInventories();

                    // Only try to transfer if we have a valid inventory
                    if (_playerInventory != null)
                        TransferFuelFromPlayer(_fuelInventory.fuelItemAllowed, 1);
                    else
                        Debug.LogWarning("Cannot transfer fuel - player inventory is null");
                }
        }

        // Add this method to reinitialize inventory references
        void InitializeInventories()
        {
            if (_playerInventory == null)
            {
                _playerInventory = Inventory.FindInventory(MainInventory.MainInventoryObjectName, "Player1");
                if (_playerInventory == null) Debug.LogError("Could not find MainPlayerInventory");
            }
        
            if (_fuelInventory == null)
            {
                _fuelInventory = gameObject.GetComponentInChildren<FuelInventory>();
                if (_fuelInventory == null) Debug.LogError("Could not find FuelInventory");
            }
        
            if (_depositInventory == null)
            {
                _depositInventory = gameObject.GetComponentInChildren<CookingDepositInventory>();
                if (_depositInventory == null) Debug.LogError("Could not find CookingDepositInventory");
            }
        
            if (_queueInventory == null)
            {
                _queueInventory = gameObject.GetComponentInChildren<CookingQueueInventory>();
                if (_queueInventory == null) Debug.LogError("Could not find CookingQueueInventory");
            }
        }

        public bool IsPlayerInRange()
        {
            return _isInPlayerRange;
        }

        public CookingQueueInventory GetQueueInventory()
        {
            return _queueInventory;
        }
        public CookingDepositInventory GetDepositInventory()
        {
            return _depositInventory;
        }
        public FuelInventory GetFuelInventory()
        {
            return _fuelInventory;
        }


        public void PromptFuelDeposit()
        {
            // Example: Show UI to deposit fuel
        }

        void ShowPreview(string message)
        {
            // if (previewPanel != null)
            // {
            //     previewPanel.SetActive(true);
            //     if (previewText != null) previewText.text = message;
            // }
        }

        void HidePreview()
        {
            // if (previewPanel != null) previewPanel.SetActive(false);
        }


        bool ValidateFuel()
        {
            return _fuelInventory.GetQuantity(_fuelInventory.fuelItemAllowed.ItemID) > 0;
        }


        public void TransferFuelFromPlayer(InventoryItem fuelItem, int quantity)
        {
            // Add null check at start of method
            if (_playerInventory == null)
            {
                Debug.LogWarning("Cannot transfer fuel - player inventory is null");
                ShowPreview("Cannot transfer fuel - inventory error");
                return;
            }

            if (_playerInventory.GetQuantity(fuelItem.ItemID) >= quantity)
            {
                if (_fuelInventory.AddItem(fuelItem, quantity))
                {
                    _playerInventory.RemoveItemByID(fuelItem.ItemID, quantity);
                    ShowPreview($"Added {quantity} fuel to the station.");
                }
                else
                {
                    Debug.Log("Fuel inventory is full!");
                    ShowPreview("Fuel inventory is full!");
                }
            }
            else
            {
                Debug.Log("Not enough fuel in player inventory.");
                ShowPreview("Not enough fuel in your inventory.");
            }
        }
        public void SetCurrentRecipe(CookingRecipe currentRecipe)
        {
            _currentRecipe = currentRecipe;
            newRecipeSetFeedbacks?.PlayFeedbacks();
            if (_currentRecipe != null)
                Debug.Log("Current recipe set to: " + currentRecipe.recipeName);
        }
        public void ShowCookingUI()
        {
            CookingUICanvasGroup.alpha = 1;
            CookingUICanvasGroup.interactable = true;
            CookingUICanvasGroup.blocksRaycasts = true;

            CookingUIPanelCanvasGroup.alpha = 1;
            CookingUIPanelCanvasGroup.interactable = true;
            CookingUIPanelCanvasGroup.blocksRaycasts = true;
        }

        public void HideCookingUI()
        {
            CookingUICanvasGroup.alpha = 0;
            CookingUICanvasGroup.interactable = false;
            CookingUICanvasGroup.blocksRaycasts = false;

            CookingUIPanelCanvasGroup.alpha = 0;
            CookingUIPanelCanvasGroup.interactable = false;
            CookingUIPanelCanvasGroup.blocksRaycasts = false;
        }
    }
}
