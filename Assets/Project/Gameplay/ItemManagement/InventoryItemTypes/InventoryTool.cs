using System;
using Project.Gameplay.Interactivity.Items;
using UnityEngine;

namespace Project.Gameplay.ItemManagement.InventoryItemTypes
{
    [CreateAssetMenu(
        fileName = "InventoryConsumable", menuName = "Roguelike/Items/InventoryTool", order = 2)]
    [Serializable]
    public class InventoryTool : PreviewableInventoryItem
    {
    }
}
