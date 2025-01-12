using Project.Gameplay.ItemManagement.InventoryTypes.Cooking;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RecipeEntry : MonoBehaviour
{
    public TMP_Text recipeName;
    public Image recipeImage;

    CookingRecipe _recipe;


    public void SetRecipe(CookingRecipe recipe)
    {
        recipeName.text = recipe.recipeName;
        recipeImage.sprite = recipe.finishedFoodItem.FinishedFood.Icon;

        _recipe = recipe;
    }

    public void OnRecipeSelected()
    {
        Debug.Log("Recipe selected: " + _recipe.recipeName);
    }
}
