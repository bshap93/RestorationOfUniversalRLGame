using System;
using MoreMountains.InventoryEngine;
using UnityEngine;

namespace Gameplay.ItemManagement.InventoryItemTypes
{
    [CreateAssetMenu(
        fileName = "InventoryCollectable", menuName = "Roguelike/Items/InventoryCollectable", order = 1)]
    [Serializable]
    public class InventoryCollectable : BaseItem
    {
    }
}
