using System;
using Project.Gameplay.Interactivity.Items;

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
