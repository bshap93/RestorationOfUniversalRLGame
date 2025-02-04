using System;
using Project.Gameplay.ItemManagement.InventoryItemTypes;
using UnityEngine;

namespace Gameplay.ItemManagement.InventoryItemTypes
{
    [CreateAssetMenu(
        fileName = "InventoryCrackedGeode", menuName = "Crafting/Stone/InventoryCrackedGeode", order = 2)]
    [Serializable]
    public class InventoryCrackedGeode : InventoryCollectable
    {
    }
}
