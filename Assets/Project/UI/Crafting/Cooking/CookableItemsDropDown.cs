using Gameplay.ItemManagement.InventoryTypes.Cooking;
using Michsky.MUIP;
using MoreMountains.Tools;
using Project.Core.Events;
using Project.Gameplay.ItemManagement.InventoryTypes.Cooking;
using UnityEngine;

namespace Project.UI.Crafting.Cooking
{
    public class CookableItemsDropDown : MonoBehaviour, MMEventListener<RecipeEvent>
    {
        public string craftingStationId;
        CustomDropdown _dropdown;
        bool isInitialized;

        void Awake()
        {
            craftingStationId = GetComponentInParent<CookStationPanelInstance>().cookingStationController.CookingStation
                .CraftingStationId;
        }

        void Start()
        {
            _dropdown = GetComponent<CustomDropdown>();
            InitializeDropdown();
        }

        void OnEnable()
        {
            this.MMEventStartListening();
        }

        void OnDisable()
        {
            this.MMEventStopListening();
        }

        void OnDestroy()
        {
            ClearDropdownItems();
        }

        // ONly update Dropdown for one specific CraftingStation,
        // and if the craftingstation id doesnt' match, ignore the event
        public void OnMMEvent(RecipeEvent mmEvent)
        {
            if (_dropdown == null) return;

            if (mmEvent.EventType == RecipeEventType.ClearCookableRecipes ||
                mmEvent.EventType == RecipeEventType.FinishedCookingRecipe)
                if (mmEvent.CraftingStationID == craftingStationId)
                    ClearDropdownItems();

            if (mmEvent.EventType == RecipeEventType.RecipeCookableWithCurrentIngredients)
                if (mmEvent.CraftingStationID == craftingStationId)
                    AddRecipeToDropdown(mmEvent.RecipeParameter);
        }

        void InitializeDropdown()
        {
            if (_dropdown != null && !isInitialized)
            {
                // Only clear the items list, don't call SetupDropdown yet
                _dropdown.items?.Clear();

                // Reset visual elements
                if (_dropdown.selectedText != null) _dropdown.selectedText.text = "";

                if (_dropdown.selectedImage != null) _dropdown.selectedImage.gameObject.SetActive(false);

                // Reset state
                _dropdown.selectedItemIndex = 0;
                _dropdown.isOn = false;

                isInitialized = true;
            }
        }

        void ClearDropdownItems()
        {
            if (_dropdown == null) return;

            // Clear child objects
            if (_dropdown.itemParent != null)
                foreach (Transform child in _dropdown.itemParent)
                    Destroy(child.gameObject);

            // Clear items list
            _dropdown.items?.Clear();

            // Reset visual elements
            if (_dropdown.selectedText != null) _dropdown.selectedText.text = "";

            if (_dropdown.selectedImage != null) _dropdown.selectedImage.gameObject.SetActive(false);

            // Reset state
            _dropdown.selectedItemIndex = 0;
            _dropdown.isOn = false;

            // Force the dropdown to close if it's open
            if (_dropdown.isOn) _dropdown.Animate();
        }

        void AddRecipeToDropdown(CookingRecipe recipe)
        {
            if (recipe == null || _dropdown == null) return;

            // Check if recipe already exists
            var recipeExists = _dropdown.items.Exists(item => item.itemName == recipe.recipeName);
            if (recipeExists) return;

            // Create a new dropdown item
            var newItem = new CustomDropdown.Item
            {
                itemName = recipe.recipeName,
                itemIcon = recipe.finishedFoodItem.FinishedFood.Icon
            };

            // Add the RecipeEvent.Trigger as a listener to OnItemSelection
            newItem.OnItemSelection.AddListener(
                () =>
                {
                    if (_dropdown.items.Contains(newItem))
                        RecipeEvent.Trigger(
                            "ChooseRecipe", RecipeEventType.ChooseRecipeFromCookable, recipe, craftingStationId);
                });

            // Add item and setup the dropdown
            _dropdown.items.Add(newItem);

            // Only call SetupDropdown when we actually have items
            if (_dropdown.items.Count > 0) _dropdown.SetupDropdown();
        }
    }
}
