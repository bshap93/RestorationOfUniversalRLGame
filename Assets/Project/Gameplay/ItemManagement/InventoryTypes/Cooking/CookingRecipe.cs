using System;
using System.Collections.Generic;
using Project.Gameplay.Interactivity.CraftingStation;
using Project.Gameplay.Interactivity.Items;
using Project.Gameplay.ItemManagement.InventoryTypes.Materials;
using UnityEngine;

namespace Project.Gameplay.ItemManagement.InventoryTypes.Cooking
{
    [CreateAssetMenu(fileName = "Crafting", menuName = "Crafting/CookingRecipe", order = 1)]
    [Serializable]
    public class CookingRecipe : CraftingRecipe
    {
        public string recipeID;
        public string recipeName;
        public List<CraftingMaterial> requiredMaterials;
        public FinishedFoodItem finishedFoodItem;
        public List<RawFoodMaterial> requiredRawFoodItems;

        public bool CanBeCookedFrom(InventoryItem[] content)
        {
            foreach (var requiredRawFoodItem in requiredRawFoodItems)
            {
                var rawFoodItem = requiredRawFoodItem;
                if (!Array.Exists(content, item => item.ItemID == rawFoodItem.item.ItemID))
                    return false;
            }

            return true;
        }
    }
}
