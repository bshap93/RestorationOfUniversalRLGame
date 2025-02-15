using System;
using MoreMountains.InventoryEngine;
using UnityEngine;

namespace Gameplay.ItemManagement.InventoryItemTypes
{
    [CreateAssetMenu(
        fileName = "InventoryConsumable", menuName = "Roguelike/Items/InventoryTool", order = 2)]
    [Serializable]
    public class InventoryTool : BaseItem
    {
    }
}
