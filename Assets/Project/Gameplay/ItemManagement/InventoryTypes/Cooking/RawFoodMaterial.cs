using System;
using Project.Gameplay.Interactivity.Items;
using Project.Gameplay.ItemManagement.InventoryTypes.Materials;

namespace Project.Gameplay.ItemManagement.InventoryTypes.Cooking
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
