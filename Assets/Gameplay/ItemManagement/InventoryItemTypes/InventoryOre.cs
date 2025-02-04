using System;
using UnityEngine;

namespace Gameplay.ItemManagement.InventoryItemTypes
{
    [CreateAssetMenu(
        fileName = "InventoryOre", menuName = "Crafting/Stone/InventoryOre", order = 4)]
    [Serializable]
    public class InventoryOre : InventoryStone
    {
    }
}
