using System;
using Project.Gameplay.ItemManagement.InventoryItemTypes;
using Project.Gameplay.ItemManagement.InventoryTypes.Cooking;
using UnityEngine;

namespace Project.Gameplay.Interactivity.Food
{
    [CreateAssetMenu(
        fileName = "RawFood", menuName = "Crafting/Food/RawFood", order = 2)]
    [Serializable]
    public class RawFood : InventoryConsumable
    {
        public CookingRecipe CookedSingleRawFoodRecipe; // The recipe to cook a single raw food item
    }
}
