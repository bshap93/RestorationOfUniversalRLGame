using System;
using Gameplay.ItemManagement.InventoryTypes.Fuel;
using MoreMountains.InventoryEngine;

namespace Project.Gameplay.ItemManagement.InventoryTypes.Fuel
{
    [Serializable]
    public class FuelMaterial
    {
        public FuelItem FuelItem;
        public int Quantity;

        public FuelMaterial(InventoryItem fuelItem, int quantity)
        {
            FuelItem = new FuelItem(fuelItem);
            Quantity = quantity;
        }
    }
}
