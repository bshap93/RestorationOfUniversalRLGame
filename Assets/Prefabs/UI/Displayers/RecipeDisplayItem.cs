using Gameplay.Extensions.InventoryEngineExtensions.Craft;
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

        public void DisplayLearned(Recipe recipe)
        {
            if (recipe.Item.Icon != null)
            {
                Icon.sprite = recipe.Item.Icon;
                Icon.enabled = true;
            }
            else
            {
                Icon.enabled = false; // Hide the icon if none exists
            }

            Name.text = $"Learned: {recipe.Item.ItemName}!";
        }
        public void DisplayFinishedCooking(Recipe recipe)
        {
            if (recipe != null)
            {
                if (recipe.Item.Icon != null)
                {
                    Icon.sprite = recipe.Item.Icon;
                    Icon.enabled = true;
                }

                else
                {
                    Icon.enabled = false; // Hide the icon if none exists
                }

                Name.text = recipe.Item.ItemName != null
                    ? $"Finished cooking: {recipe.Item.ItemName}!"
                    : "Finished cooking!";
            }
            else
            {
                Icon.enabled = false;
                Name.text = "Finished cooking!";
            }
        }
    }
}
