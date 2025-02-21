using System;
using Gameplay.Extensions.InventoryEngineExtensions.Craft;
using MoreMountains.Tools;

namespace Core.Events
{
    [Serializable]
    public enum RecipeEventType
    {
        RecipeLearned,
        CraftingStarted,
        CraftingFinished,
        ShowRecipeDetails,
        Error
    }

    /// <summary>
    ///     Recipe events are used to trigger events that involve cooking recipes.
    /// </summary>
    public struct RecipeEvent
    {
        static RecipeEvent e;

        public string EventName;
        public RecipeEventType EventType;
        public Recipe RecipeParameter;

        public static void Trigger(string eventName, RecipeEventType recipeEventType, Recipe recipe
        )
        {
            e.EventName = eventName;
            e.RecipeParameter = recipe;
            e.EventType = recipeEventType;
            MMEventManager.TriggerEvent(e);
        }
    }

    [Serializable]
    public enum RecipeGroupEventType
    {
        RecipeGroupLearned,
        RecipeGroupAlreadyKnown
    }

    /// <summary>
    ///     Used to reason about Craft.cs objects.
    /// </summary>
    public struct RecipeGroupEvent
    {
        static RecipeGroupEvent e;

        public RecipeGroupEventType EventType;
        public RecipeGroup RecipeGroup;

        public static void Trigger(RecipeGroupEventType recipeGroupEventType, string recipeGroupUniqueID
        )
        {
            var recipeGroup = RecipeGroup.RetrieveCraftGroup(recipeGroupUniqueID);
            e.RecipeGroup = recipeGroup;
            e.EventType = recipeGroupEventType;
            MMEventManager.TriggerEvent(e);
        }
    }
}
