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
        ClearCookableRecipes,
        FinishedCookingRecipe,
        ShowRecipeDetails,
        Error,
        RecipeAlreadyKnown,
        NoNewRecipes,
        NewRecipesLearned
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
        public string CraftingStationID;

        public static void Trigger(string eventName, RecipeEventType recipeEventType, CookingRecipe recipe,
            string craftingStationID)
        {
            e.EventName = eventName;
            e.RecipeParameter = recipe;
            e.EventType = recipeEventType;
            e.CraftingStationID = craftingStationID;
            MMEventManager.TriggerEvent(e);
        }
    }
}
