using Project.Gameplay.Interactivity.Items;

namespace Project.Gameplay.ItemManagement.InventoryTypes.Cooking
{
    public class RawFoodItem
    {
        public float cookDuration;
        public float defaultCookDuration = 10f;
        public InventoryItem Item; // The base inventory item


        public RawFoodItem(InventoryItem item)
        {
            Item = item;
            cookDuration = GetCookDurationForItem(item);
        }

        public RawFoodItem(InventoryItem item, float cookDuration)
        {
            Item = item;
            this.cookDuration = cookDuration;
        }

        public float GetCookDurationForItem(InventoryItem item)
        {
            return defaultCookDuration;
        }
    }
}
