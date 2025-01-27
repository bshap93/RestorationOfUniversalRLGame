using System;
using Plugins.TopDownEngine.ThirdParty.MoreMountains.InentoryEngine.InventoryEngine.Scripts.Items;
using UnityEngine;

namespace Project.Gameplay.ItemManagement.InventoryItemTypes
{
    [CreateAssetMenu(
        fileName = "InventoryCollectable", menuName = "Roguelike/Items/InventoryCollectable", order = 1)]
    [Serializable]
    public class InventoryCollectable : BaseItem
    {
    }
}
