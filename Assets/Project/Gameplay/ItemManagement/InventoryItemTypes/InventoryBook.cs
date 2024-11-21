using System;
using MoreMountains.InventoryEngine;
using UnityEngine;

namespace Project.Gameplay.ItemManagement.InventoryItemTypes
{
    [CreateAssetMenu(
        fileName = "InventoryCollectable", menuName = "Roguelike/Items/InventoryBook", order = 1)]
    [Serializable]
    public class InventoryBook : InventoryItem
    {
    }
}
