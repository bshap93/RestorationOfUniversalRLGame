using Gameplay.Extensions.InventoryEngineExtensions.Craft;
using MoreMountains.Tools;

namespace Gameplay.Events
{
    public enum CraftingEventType
    {
        CraftingStarted,
        CraftingFinished
    }

    public struct CraftingEvent
    {
        static CraftingEvent e;


        public Recipe Recipe;
        public string EventName;
        public CraftingEventType EventType;

        public static void Trigger(string eventName,
            CraftingEventType craftingEventType, Recipe recipe)
        {
            e.EventName = eventName;
            e.EventType = craftingEventType;
            e.Recipe = recipe;

            MMEventManager.TriggerEvent(e);
        }
    }
}
