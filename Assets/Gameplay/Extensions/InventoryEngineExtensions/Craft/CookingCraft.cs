using Core.Events;
using MoreMountains.Feedbacks;
using MoreMountains.InventoryEngine;

namespace Gameplay.Extensions.InventoryEngineExtensions.Craft
{
    public static class CookingCraft
    {
        public static void CookRecipe(
            this Inventory inventory,
            Recipe recipe,
            MMFeedbacks startCookingFeedback = null,
            MMFeedbacks negateFeedback = null)
        {
            RecipeEvent.Trigger(
                "StartCooking", RecipeEventType.CraftingStarted, recipe);

            // Play start cooking feedback
            startCookingFeedback?.PlayFeedbacks();

            // Do the actual crafting
            inventory.Craft(recipe, startCookingFeedback, negateFeedback);
        }
    }
}
