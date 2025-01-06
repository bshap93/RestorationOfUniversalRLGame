using System;
using Project.Gameplay.Interactivity.Items;
using UnityEngine.Serialization;

namespace Project.Gameplay.ItemManagement.InventoryTypes.Fuel
{
    [Serializable]
    public class FuelItem
    {
        public InventoryItem Item; // The base inventory item
        [FormerlySerializedAs("BurnDuration")] public float burnDuration;
        public float remainingFraction;
        public float defaultBurnDuration = 100f;


        public FuelItem(InventoryItem item)
        {
            Item = item;
            burnDuration = GetBurnDurationForItem(item);
        }

        public FuelItem(InventoryItem item, float burnDuration)
        {
            Item = item;
            this.burnDuration = burnDuration;
        }

        public float GetBurnDurationForItem(InventoryItem item)
        {
            return defaultBurnDuration;
        }
    }
}
