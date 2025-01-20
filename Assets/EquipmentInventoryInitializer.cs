using MoreMountains.InventoryEngine;
using UnityEngine;

public class EquipmentInventoryInitializer : MonoBehaviour
{
    public Inventory[] inventories;

    public InventorySlot[] inventorySlots;


    void Start()
    {
        foreach (var inventory in inventories)
            if (inventory.Content.Length > 0)
                foreach (var inventoryItem in inventory.Content)
                {
                }
    }
}
