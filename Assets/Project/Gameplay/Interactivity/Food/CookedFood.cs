using System;
using Project.Gameplay.ItemManagement.InventoryItemTypes;
using UnityEngine;

namespace Project.Gameplay.Interactivity.Food
{
    [CreateAssetMenu(
        fileName = "CookedFood", menuName = "Crafting/Food/CookedFood", order = 2)]
    [Serializable]
    public class CookedFood : InventoryConsumable
    {
    }
}
