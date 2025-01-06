using System;
using Project.Gameplay.Interactivity.Items;

namespace Project.Gameplay.ItemManagement.InventoryTypes.Materials
{
    [Serializable]
    public class RawFoodMaterial : CraftingMaterial
    {
        public RawFoodMaterial(InventoryItem item, int quantity, float craftingTime = 1) : base(
            item, quantity, craftingTime)
        {
        }
    }
}
