using System;
using JetBrains.Annotations;
using MoreMountains.Tools;
using Project.Gameplay.ItemManagement.InventoryTypes.Cooking;

namespace Project.Core.Events
{
    [Serializable]
    public enum CookingStationEventType
    {
        CookingStationInRange,
        CookingStationOutOfRange,
        CookingStationSelected,
        CookingStationDeselected,
        FuelBurntUpdate,
        TryAddFuel
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
        [CanBeNull] public CookingStation CookingStationParameter;

        public static void Trigger(string eventName,
            CookingStationEventType cookingStationEventType,
            CookingStationController cookingStationController, CookingStation cookingStation = null)
        {
            e.EventName = eventName;
            e.EventType = cookingStationEventType;
            e.CookingStationControllerParameter = cookingStationController;

            if (cookingStation != null)
                e.CookingStationParameter = cookingStation;

            MMEventManager.TriggerEvent(e);
        }
    }
}
