using System;
using Project.Gameplay.ItemManagement.InventoryItemTypes;
using UnityEngine;

namespace Project.Gameplay.Interactivity.Food
{
    [CreateAssetMenu(
        fileName = "RawFood", menuName = "Crafting/Food/RawFood", order = 2)]
    [Serializable]
    public class RawFood : InventoryConsumable
    {
    }
}
