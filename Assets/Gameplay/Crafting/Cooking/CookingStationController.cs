using Gameplay.Extensions.InventoryEngineExtensions.Craft;
using Michsky.MUIP;
using MoreMountains.Feedbacks;
using MoreMountains.InventoryEngine;
using Project.Gameplay.SaveLoad.Triggers;
using UnityEngine;

namespace Gameplay.Crafting.Cooking
{
    public class CookingStationController : MonoBehaviour, ISelectableTrigger
    {
        [Header("Station Setup")] [Tooltip("Assign your Craft ScriptableObject with cooking recipes here")]
        public Craft stationRecipes;
        public Inventory playerInventory;

        [Header("UI References")] [Tooltip("The Canvas containing all UI elements for this station")]
        public Canvas stationCanvas;
        public CanvasGroup cookingUIPanel;

        [Header("Feedbacks")] public MMFeedbacks startCookingFeedbacks;
        public MMFeedbacks finishCookingFeedbacks;
        public MMFeedbacks interactFeedbacks;

        [Header("Cooking Settings")] public float cookingTime = 5f;
        public FuelQueue fuelQueue;
        public CraftingButtons craftingButtons;
        [Header("UI References")] public ProgressBar fuelProgressBar; // Reference to your UI progress bar
        float _currentCookingTime;
        bool _isCooking;

        bool _isInPlayerRange;

        void Awake()
        {
            // Make sure canvas starts hidden
            if (stationCanvas != null) stationCanvas.enabled = false;
            HideCookingUI();

            // Set up crafting buttons
            if (craftingButtons != null)
            {
                // Assign the recipes to the crafting buttons
                craftingButtons.craftRecipes = stationRecipes;
                craftingButtons.gameObject.SetActive(false);
            }

            InitializeInventoryReference();

            // Set up fuel progress bar
            if (fuelProgressBar != null && fuelQueue != null)
            {
                // Initialize progress bar
                fuelProgressBar.minValue = 0;
                fuelProgressBar.maxValue = 100;
                fuelProgressBar.suffix = "%";

                // Update initial value
                fuelProgressBar.SetValue(fuelQueue.GetFuelPercentage());

                // Subscribe to fuel changes
                fuelQueue.OnFuelChanged += UpdateFuelDisplay;
            }
        }

        void Update()
        {
            if (_isCooking)
            {
                _currentCookingTime += Time.deltaTime;
                if (_currentCookingTime >= cookingTime)
                {
                    _isCooking = false;
                    finishCookingFeedbacks?.PlayFeedbacks();
                }
            }
        }


        void OnDestroy()
        {
            if (fuelQueue != null) fuelQueue.OnFuelChanged -= UpdateFuelDisplay;
        }

        void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                _isInPlayerRange = true;

                if (!HasEnoughFuel())
                {
                    ShowPreview("Need fuel to cook. Add fuel first.");
                    return;
                }

                ShowPreview("Press F to interact");
            }
        }

        void OnTriggerExit(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                _isInPlayerRange = false;
                if (stationCanvas != null) stationCanvas.enabled = false;
                if (craftingButtons != null) craftingButtons.gameObject.SetActive(false);
                HideCookingUI();
                HidePreview();
            }
        }

        void OnValidate()
        {
            // Help catch setup issues in the editor
            if (stationRecipes == null)
                Debug.LogWarning($"CookingStation '{gameObject.name}': No recipes assigned!", this);

            if (stationCanvas == null)
                Debug.LogWarning($"CookingStation '{gameObject.name}': No canvas assigned!", this);

            if (craftingButtons == null)
                Debug.LogWarning($"CookingStation '{gameObject.name}': No crafting buttons assigned!", this);
        }

        public void OnSelectedItem()
        {
            if (_isInPlayerRange)
            {
                if (!HasEnoughFuel())
                {
                    ShowPreview("Add fuel before cooking");
                    return;
                }

                // Show the station's canvas
                if (stationCanvas != null) stationCanvas.enabled = true;
                ShowCookingUI();
                if (craftingButtons != null) craftingButtons.gameObject.SetActive(true);
                interactFeedbacks?.PlayFeedbacks();
            }
        }

        public void OnUnSelectedItem()
        {
            if (stationCanvas != null) stationCanvas.enabled = false;
            if (craftingButtons != null) craftingButtons.gameObject.SetActive(false);
            HideCookingUI();
        }

        void UpdateFuelDisplay(float fuelPercentage)
        {
            if (fuelProgressBar != null) fuelProgressBar.SetValue(fuelPercentage);
        }

        void InitializeInventoryReference()
        {
            if (playerInventory == null)
            {
                playerInventory = Inventory.FindInventory("MainInventory", "Player1");
                if (playerInventory == null)
                {
                    Debug.LogError($"CookingStation '{gameObject.name}': Could not find MainInventory");
                    return;
                }

                // Assign inventory to crafting buttons
                if (craftingButtons != null) craftingButtons.CraftingInventory = playerInventory;
            }
        }

        // Updated TryAddFuel to show progress bar changes
        public void TryAddFuel(int fuelItemIndex)
        {
            if (!_isInPlayerRange) return;

            var fuelItem = playerInventory.Content[fuelItemIndex];
            if (fuelItem.ItemID == fuelQueue.requiredFuelItemID)
            {
                playerInventory.RemoveItem(fuelItemIndex, 1);
                fuelQueue.AddFuelItem(fuelItem);
                ShowPreview($"Fuel added successfully ({fuelQueue.GetFuelPercentage():F0}% full)");
            }
            else
            {
                ShowPreview("This item cannot be used as fuel");
            }
        }

        bool HasEnoughFuel()
        {
            return fuelQueue.hasValidFuel();
        }

        public void ShowCookingUI()
        {
            if (cookingUIPanel != null)
            {
                cookingUIPanel.alpha = 1;
                cookingUIPanel.interactable = true;
                cookingUIPanel.blocksRaycasts = true;
            }
        }

        public void HideCookingUI()
        {
            if (cookingUIPanel != null)
            {
                cookingUIPanel.alpha = 0;
                cookingUIPanel.interactable = false;
                cookingUIPanel.blocksRaycasts = false;
            }
        }

        void ShowPreview(string message)
        {
            // Implement your UI preview system here
            Debug.Log($"Station '{gameObject.name}': {message}");
        }

        void HidePreview()
        {
            // Implement your UI preview system here
        }
    }
}
