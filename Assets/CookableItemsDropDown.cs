using System.Collections.Generic;
using Michsky.MUIP;
using MoreMountains.Tools;
using Project.Core.Events;
using UnityEngine;

public class CookableItemsDropDown : MonoBehaviour, MMEventListener<RecipeEvent>
{
    readonly List<string> CookingRepiceIds = new();
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
        if (recipeEvent.EventType == RecipeEventType.ClearCookableRecipes)
        {
            CookingRepiceIds.Clear();
            _dropdown.items.Clear();
        }


        if (recipeEvent.EventType == RecipeEventType.RecipeCookableWithCurrentIngredients)
        {
            var recipe = recipeEvent.RecipeParameter;

            // Instantiate the prefab only if it's not already in the list
            if (CookingRepiceIds.Contains(recipe.recipeID))
                return;


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

            CookingRepiceIds.Add(recipe.recipeID);
        }
    }
}
