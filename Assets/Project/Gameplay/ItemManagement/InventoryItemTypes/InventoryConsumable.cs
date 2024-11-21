using System;
using MoreMountains.InventoryEngine;
using UnityEngine;

namespace Project.Gameplay.ItemManagement.InventoryItemTypes
{
    [CreateAssetMenu(
        fileName = "InventoryConsumable", menuName = "Roguelike/Items/InventoryConsumable", order = 2)]
    [Serializable]
    public class InventoryConsumable : InventoryItem
    {
    }
}
