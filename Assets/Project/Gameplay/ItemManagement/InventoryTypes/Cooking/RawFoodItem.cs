using Project.Gameplay.Interactivity.Items;

namespace Project.Gameplay.ItemManagement.InventoryTypes.Cooking
{
    public class RawFoodItem
    {
        public readonly float defaultCookDuration = 10f;


        public RawFoodItem(InventoryItem item)
        {
            GetCookDurationForItem(item);
        }

        public float GetCookDurationForItem(InventoryItem item)
        {
            return defaultCookDuration;
        }
    }
}
