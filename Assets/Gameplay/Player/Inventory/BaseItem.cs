using System;
using Project.Gameplay.Interactivity.Items;
using UnityEngine;

namespace Plugins.TopDownEngine.ThirdParty.MoreMountains.InentoryEngine.InventoryEngine.Scripts.Items
{
    public enum ItemUsageType
    {
        Pickable, // Can be picked up
        Usable // Must be used on the spot
    }


    [CreateAssetMenu(fileName = "BaseItem", menuName = "MoreMountains/InventoryEngine/BaseItem", order = 0)]
    [Serializable]
    /// <summary>
    /// Base item class, to use when your object doesn't do anything special
    /// </summary>
    public class BaseItem : InventoryItem
    {
        public ItemUsageType UsageType; // New property to determine the item's behavior
        public bool DisappearAfterUse; // Should the item disappear after use?
    }
}
