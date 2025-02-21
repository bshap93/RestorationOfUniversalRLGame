using Core.Events;
using Gameplay.Extensions.InventoryEngineExtensions.Craft;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RecipeEntry : MonoBehaviour
{
    public TMP_Text recipeName;
    public Image recipeImage;

    Recipe _recipe;


    public void SetRecipe(Recipe recipe)
    {
        recipeName.text = recipe.Item.ItemName;
        recipeImage.sprite = recipe.Item.Icon;

        _recipe = recipe;
    }

    public void OnRecipeSelected()
    {
        Debug.Log("Recipe selected: " + _recipe.Item.ItemName);
        RecipeEvent.Trigger("ShowRecipeDetails", RecipeEventType.ShowRecipeDetails, _recipe);
    }
}
