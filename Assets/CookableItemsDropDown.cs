using Michsky.MUIP;
using MoreMountains.Tools;
using Project.Core.Events;
using Project.Gameplay.ItemManagement.InventoryTypes.Cooking;
using UnityEngine;

public class CookableItemsDropDown : MonoBehaviour, MMEventListener<RecipeEvent>
{
    public string CraftingStationId;
    CustomDropdown _dropdown;
    bool isInitialized;

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
    public void OnMMEvent(RecipeEvent recipeEvent)
    {
        if (_dropdown == null) return;

        if (recipeEvent.EventType == RecipeEventType.ClearCookableRecipes ||
            recipeEvent.EventType == RecipeEventType.FinishedCookingRecipe)
            if (recipeEvent.CraftingStationID == CraftingStationId)
                ClearDropdownItems();

        if (recipeEvent.EventType == RecipeEventType.RecipeCookableWithCurrentIngredients)
            if (recipeEvent.CraftingStationID == CraftingStationId)
                AddRecipeToDropdown(recipeEvent.RecipeParameter);
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
                        "ChooseRecipe", RecipeEventType.ChooseRecipeFromCookable, recipe, CraftingStationId);
            });

        // Add item and setup the dropdown
        _dropdown.items.Add(newItem);

        // Only call SetupDropdown when we actually have items
        if (_dropdown.items.Count > 0) _dropdown.SetupDropdown();
    }
}
