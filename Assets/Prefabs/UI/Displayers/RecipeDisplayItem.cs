using Gameplay.ItemManagement.InventoryTypes.Cooking;
using MoreMountains.InventoryEngine;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Prefabs.UI.Displayers
{
    public class RecipeDisplayItem : MonoBehaviour
    {
        [SerializeField] Image Icon;
        [SerializeField] TMP_Text Name;
        public void Display(InventoryItem item, int quantity)
        {
            Icon.sprite = item.Icon;
            Name.text = item.ItemName + "!";
        }

        public void DisplayLearned(CookingRecipe recipe)
        {
            if (recipe.recipeImage != null)
            {
                Icon.sprite = recipe.recipeImage;
                Icon.enabled = true;
            }
            else
            {
                Icon.enabled = false; // Hide the icon if none exists
            }

            Name.text = $"Learned: {recipe.recipeName}";
        }
        public void DisplayFinishedCooking(CookingRecipe recipe)
        {
            if (recipe != null)
            {
                if (recipe.recipeImage != null)
                {
                    Icon.sprite = recipe.recipeImage;
                    Icon.enabled = true;
                }
                else if (recipe.finishedFoodItem != null && recipe.finishedFoodItem.FinishedFood != null &&
                         recipe.finishedFoodItem.FinishedFood.Icon != null)
                {
                    Icon.sprite = recipe.finishedFoodItem.FinishedFood.Icon;
                    Icon.enabled = true;
                }
                else
                {
                    Icon.enabled = false; // Hide the icon if none exists
                }

                Name.text = recipe.recipeName != null ? $"Finished cooking: {recipe.recipeName}!" : "Finished cooking!";
            }
            else
            {
                Icon.enabled = false;
                Name.text = "Finished cooking!";
            }
        }
    }
}
