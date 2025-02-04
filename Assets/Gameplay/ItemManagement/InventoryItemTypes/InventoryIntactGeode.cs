using System;
using UnityEngine;

namespace Gameplay.ItemManagement.InventoryItemTypes
{
    [CreateAssetMenu(
        fileName = "InventoryIntactGeode", menuName = "Crafting/Stone/InventoryIntactGeode", order = 1)]
    [Serializable]
    public class InventoryIntactGeode : InventoryStone
    {
    }
}
