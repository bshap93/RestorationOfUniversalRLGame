using MoreMountains.InventoryEngine;
using MoreMountains.Tools;
using UnityEngine;

namespace Project.Gameplay.Events
{
    public struct ItemEvent
    {
        static ItemEvent e;

        public InventoryItem Item;
        public string EventName;
        public Transform ItemTransform;
        public int Amount;

        public static void Trigger(string eventName, InventoryItem inventoryItem, Transform itemTransform,
            int amount = 1)
        {
            e.EventName = eventName;
            e.Amount = amount;
            e.Item = inventoryItem;
            e.ItemTransform = itemTransform;

            MMEventManager.TriggerEvent(e);
        }
    }
}
