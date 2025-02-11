using Gameplay.ItemManagement.InventoryTypes;
using Gameplay.ItemManagement.InventoryTypes.Cooking;
using JetBrains.Annotations;
using MoreMountains.Feedbacks;
using MoreMountains.InventoryEngine;
using MoreMountains.Tools;
using Project.Core.Events;
using Project.Gameplay.Interactivity.Items;
using Project.Gameplay.ItemManagement.InventoryTypes.Cooking;
using Project.Gameplay.ItemManagement.InventoryTypes.Fuel;
using Project.Gameplay.SaveLoad.Triggers;
using UnityEngine;
using UnityEngine.Serialization;

namespace Gameplay.Crafting.Cooking
{
    public class CookingStationController : MonoBehaviour, ISelectableTrigger, MMEventListener<CookingStationEvent>,
        MMEventListener<RecipeEvent>
    {
        [FormerlySerializedAs("CookingUIPanelCanvasGroup")]
        public CanvasGroup cookingUIPanelCanvasGroup;

        [FormerlySerializedAs("StartCookingFeedbacks")]
        public MMFeedbacks startCookingFeedbacks;
        [FormerlySerializedAs("FinishCookingFeedbacks")]
        public MMFeedbacks finishCookingFeedbacks;

        [FormerlySerializedAs("FuelItemAlreadyAdded")] [CanBeNull]
        public FuelMaterial fuelItemAlreadyAdded;

        [FormerlySerializedAs("CookingStation")]
        public CookingStation cookingStation;


        [Header("Fuel & Progress Tracking")] public float fuelBurnRate = 1f; // Time in seconds to burn one unit of fuel


        public MMFeedbacks interactFeedbacks;
        public MMFeedbacks craftingFeedbacks;
        public MMFeedbacks completionFeedbacks;
        public MMFeedbacks newRecipeSetFeedbacks;
        [FormerlySerializedAs("_playerInventory")]
        public Inventory playerInventory; // Reference to the player's inventory

        CookingRecipe _currentRecipe;

        FuelQueue _fuelQueue;

        bool _isBurningFuel;

        bool _isCrafting;

        bool _isInPlayerRange;


        void Awake()
        {
            InitializeInventories();
            HideCookingUI();
        }

        void OnEnable()
        {
            InitializeInventories();

            this.MMEventStartListening<CookingStationEvent>();
            this.MMEventStartListening<RecipeEvent>();
        }

        void OnDisable()
        {
            this.MMEventStopListening<CookingStationEvent>();
            this.MMEventStopListening<RecipeEvent>();
        }

        void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                _isInPlayerRange = true;

                if (cookingStation.CraftingStationId == null)
                {
                    Debug.LogError("CraftingStationId is null");
                    return;
                }


                CookingStationEvent.Trigger(
                    "CookingStationInRange", CookingStationEventType.CookingStationInRange, this);

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
            CookingStationEvent.Trigger(
                "CookingStationDeselected", CookingStationEventType.CookingStationDeselected, this);
        }
        public void OnMMEvent(CookingStationEvent mmEvent)
        {
            if (mmEvent.EventType == CookingStationEventType.TryAddFuel)
            {
                Debug.Log("Received TryAddFuel event");
                TryAddFuel();
            }

            if (mmEvent.EventType == CookingStationEventType.StartCooking) Debug.Log("Received StartCooking event");

            if (mmEvent.EventType == CookingStationEventType.ToggleFire)
            {
                Debug.Log("Received ToggleFire event");
                _isBurningFuel = !_isBurningFuel;
            }
        }
        public void OnMMEvent(RecipeEvent mmEvent)
        {
            if (mmEvent.EventType == RecipeEventType.FinishedCookingRecipe) finishCookingFeedbacks?.PlayFeedbacks();
        }
        public void TryAddFuel()
        {
            if (_isInPlayerRange)
                if (!ValidateFuel())
                    // Reinitialize inventories if reference is lost
                    if (playerInventory == null)
                        InitializeInventories();
        }

        // Add this method to reinitialize inventory references
        void InitializeInventories()
        {
            if (playerInventory == null)
            {
                playerInventory = Inventory.FindInventory(MainInventory.MainInventoryObjectName, "Player1");
                if (playerInventory == null) Debug.LogError("Could not find MainPlayerInventory");
            }
        }

        public bool IsPlayerInRange()
        {
            return _isInPlayerRange;
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
            return _fuelQueue.hasValidFuel();
        }


        public void TransferFuelFromPlayer(InventoryItem fuelItem, int quantity)
        {
            // Add null check at start of method
            if (playerInventory == null)
            {
                Debug.LogWarning("Cannot transfer fuel - player inventory is null");
                ShowPreview("Cannot transfer fuel - inventory error");
                return;
            }

            if (playerInventory.GetQuantity(fuelItem.ItemID) >= quantity)
            {
                playerInventory.RemoveItemByID(fuelItem.ItemID, quantity);
                _fuelQueue.AddFuelItem(fuelItem);
                ShowPreview("Fuel added to the queue.");
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
            cookingUIPanelCanvasGroup.alpha = 1;
            cookingUIPanelCanvasGroup.interactable = true;
            cookingUIPanelCanvasGroup.blocksRaycasts = true;
        }

        public void HideCookingUI()
        {
            cookingUIPanelCanvasGroup.alpha = 0;
            cookingUIPanelCanvasGroup.interactable = false;
            cookingUIPanelCanvasGroup.blocksRaycasts = false;
        }
    }
}
