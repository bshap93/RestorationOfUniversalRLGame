using System;
using MoreMountains.Tools;

namespace Project.Core.Events
{
    [Serializable]
    public enum CookingStationEventType
    {
        CookingStationInRange,
        CookingStationOutOfRange,
        CookingStationSelected,
        CookingStationDeselected,
        FuelBurntUpdate
    }

    /// <summary>
    ///     Recipe events are used to trigger events that involve cooking recipes.
    /// </summary>
    public struct CookingStationEvent
    {
        static CookingStationEvent e;

        public string EventName;
        public CookingStationEventType EventType;
        public CookingStationController CookingStationControllerParameter;

        public static void Trigger(string eventName,
            CookingStationEventType cookingStationEventType,
            CookingStationController cookingStationController)
        {
            e.EventName = eventName;
            e.EventType = cookingStationEventType;
            e.CookingStationControllerParameter = cookingStationController;
            MMEventManager.TriggerEvent(e);
        }
    }
}
