using System.Collections.Generic;
using Michsky.MUIP;
using MoreMountains.Tools;
using Project.Core.Events;
using UnityEngine;

public class CookableItemsDropDown : MonoBehaviour, MMEventListener<RecipeEvent>
{
    CustomDropdown _dropdown;

    void Start()
    {
        _dropdown = GetComponent<CustomDropdown>();
    }
    void OnEnable()
    {
        this.MMEventStartListening();
    }

    void OnDisable()
    {
        this.MMEventStopListening();
    }

    public void OnMMEvent(RecipeEvent recipeEvent)
    {
        if (recipeEvent.EventType == RecipeEventType.ClearCookableRecipes ||
            recipeEvent.EventType == RecipeEventType.FinishedCookingRecipe)
        {
            var itemsToRemove = new List<CustomDropdown.Item>(_dropdown.items);

            foreach (var item in itemsToRemove)
            {
                _dropdown.RemoveItem(item.itemName);
                _dropdown.Animate();
            }
        }


        if (recipeEvent.EventType == RecipeEventType.RecipeCookableWithCurrentIngredients)
        {
            var recipe = recipeEvent.RecipeParameter;

            // Create a new dropdown item and add it to the list
            var newItem = new CustomDropdown.Item
            {
                itemName = recipe.recipeName,
                itemIcon = recipe.finishedFoodItem.FinishedFood.Icon
            };

            // Add the RecipeEvent.Trigger as a listener to OnItemSelection
            newItem.OnItemSelection.AddListener(
                () => { RecipeEvent.Trigger("ChooseRecipe", RecipeEventType.ChooseRecipeFromCookable, recipe); });

            _dropdown.items.Add(newItem);


            _dropdown.SetupDropdown();

            // CookingRepiceIds.Add(recipe.recipeID);
        }
    }
}
