using Project.Gameplay.ItemManagement.InventoryTypes.Cooking;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RecipeEntry : MonoBehaviour
{
    public TMP_Text recipeName;
    public Image recipeImage;

    public void SetRecipe(CookingRecipe recipe)
    {
        recipeName.text = recipe.recipeName;
        recipeImage.sprite = recipe.finishedFoodItem.FinishedFood.Icon;
    }
}
