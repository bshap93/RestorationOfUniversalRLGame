using System;
using Project.Gameplay.Interactivity.Items;
using UnityEngine;

namespace Project.Gameplay.ItemManagement.InventoryItemTypes.Books
{
    [CreateAssetMenu(
        fileName = "InventoryBook", menuName = "Crafting/Book", order = 1)]
    [Serializable]
    public class InventoryBook : InventoryItem
    {
        public string BookTitle;
    }
}
