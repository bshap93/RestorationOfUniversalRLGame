using Gameplay.ItemManagement.InventoryTypes.Destructables;
using MoreMountains.Tools;
using UnityEngine;

namespace Gameplay.Events
{
    public struct DestructibleEvent
    {
        static DestructibleEvent e;

        public Destructible Destructible;
        public string EventName;
        public Transform DestructibleTransform;

        public static void Trigger(string eventName, Destructible destructible, Transform destructibleTransform)
        {
            e.EventName = eventName;
            e.Destructible = destructible;
            e.DestructibleTransform = destructibleTransform;

            MMEventManager.TriggerEvent(e);
        }
    }
}
