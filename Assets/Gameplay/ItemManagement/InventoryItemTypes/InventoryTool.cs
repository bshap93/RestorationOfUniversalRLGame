using System;
using Plugins.TopDownEngine.ThirdParty.MoreMountains.InentoryEngine.InventoryEngine.Scripts.Items;
using UnityEngine;

namespace Project.Gameplay.ItemManagement.InventoryItemTypes
{
    [CreateAssetMenu(
        fileName = "InventoryConsumable", menuName = "Roguelike/Items/InventoryTool", order = 2)]
    [Serializable]
    public class InventoryTool : BaseItem
    {
    }
}
