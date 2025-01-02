using Project.Gameplay.ItemManagement.InventoryItemTypes;
using UnityEngine;

[CreateAssetMenu(fileName = "CarrotStew", menuName = "Roguelike/Items/CarrotStew", order = 2)]
public class CarrotStew : InventoryConsumable
{
    // Inherits HealthToGive from InventoryConsumable
    // You might want to make it give more health than raw carrots

    void OnEnable()
    {
        // Set default values for a cooked item
        ItemID = "CarrotStew";
        ItemName = "Carrot Stew";
        ShortDescription = "A warm, hearty stew made from fresh carrots";
        HealthToGive = 8f; // Double the healing of raw carrots
        Usable = true;
        Consumable = true;
        ConsumeQuantity = 1;
    }
}
