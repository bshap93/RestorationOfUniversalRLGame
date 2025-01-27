using Project.Gameplay.Interactivity.Items;
using Project.Gameplay.ItemManagement.InventoryTypes.Cooking;
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

        public void Display(CookingRecipe recipe)
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
    }
}
