using System;
using Project.Gameplay.Interactivity.Food;
using UnityEngine;

namespace Project.Gameplay.ItemManagement.InventoryTypes.Cooking
{
    [Serializable]
    public class FinishedFoodItem
    {
        public FinishedFood FinishedFood;
        public int Quantity;
        public GameObject prefabDrop;
    }
}
