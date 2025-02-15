using System;
using MoreMountains.InventoryEngine;
using UnityEngine;

namespace Gameplay.ItemManagement.InventoryItemTypes.Books
{
    [CreateAssetMenu(
        fileName = "InventoryBook", menuName = "Crafting/Book", order = 1)]
    [Serializable]
    public class InventoryBook : BaseItem
    {
        [TextArea] [Tooltip("the item's long description to display in the details panel")]
        public string ContentsExerpt;
    }
}
