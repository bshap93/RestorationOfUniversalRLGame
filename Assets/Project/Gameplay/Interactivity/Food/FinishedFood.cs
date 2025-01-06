using System;
using Project.Gameplay.ItemManagement.InventoryItemTypes;
using UnityEngine;

namespace Project.Gameplay.Interactivity.Food
{
    [CreateAssetMenu(
        fileName = "FinishedFood", menuName = "Crafting/Food/FinishedFood", order = 2)]
    [Serializable]
    public class FinishedFood : InventoryConsumable
    {
    }
}
