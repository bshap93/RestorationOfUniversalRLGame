using System;
using MoreMountains.Tools;
using Project.Gameplay.ItemManagement.InventoryTypes.Cooking;

namespace Project.Core.Events
{
    [Serializable]
    public enum RecipeEventType
    {
        RecipeLearned
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

        public static void Trigger(string eventName, CookingRecipe recipe)
        {
            e.EventName = eventName;
            e.RecipeParameter = recipe;
            MMEventManager.TriggerEvent(e);
        }
    }
}
