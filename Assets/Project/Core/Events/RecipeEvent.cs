using System;
using MoreMountains.Tools;
using Project.Gameplay.ItemManagement.InventoryTypes.Cooking;

namespace Project.Core.Events
{
    [Serializable]
    public enum RecipeEventType
    {
        RecipeLearned,
        RecipeCookableWithCurrentIngredients,
        ChooseRecipeFromCookable,
        ClearCookableRecipes
    }

    /// <summary>
    ///     Recipe events are used to trigger events that involve cooking recipes.
    /// </summary>
    public struct RecipeEvent
    {
        static RecipeEvent e;

        public string EventName;
        public RecipeEventType EventType;
        public CookingRecipe RecipeParameter;

        public static void Trigger(string eventName, RecipeEventType recipeEventType, CookingRecipe recipe)
        {
            e.EventName = eventName;
            e.RecipeParameter = recipe;
            e.EventType = recipeEventType;
            MMEventManager.TriggerEvent(e);
        }
    }
}
