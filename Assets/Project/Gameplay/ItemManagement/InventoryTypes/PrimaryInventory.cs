using Project.Gameplay.Interactivity.Items;

namespace Project.Gameplay.ItemManagement.InventoryTypes
{
    public class PrimaryInventory : BaseInventory
    {
        public override bool AddItem(InventoryItem item, int quantity)
        {
            return base.AddItem(item, quantity);
        }

        public override bool RemoveItem(int itemIndex, int quantity)
        {
            return base.RemoveItem(itemIndex, quantity);
        }
        
    }
}
