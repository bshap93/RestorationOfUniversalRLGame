using System;
using System.Collections.Generic;
using Project.Gameplay.Interactivity.CraftingStation;
using Project.Gameplay.Interactivity.Items;
using Project.Gameplay.ItemManagement.InventoryTypes.Materials;
using UnityEngine;

namespace Project.Gameplay.ItemManagement.InventoryTypes.Cooking
{
    [Serializable]
    [CreateAssetMenu(
        fileName = "Crafting", menuName = "Crafting/CookingRecipe", order = 1)]
    public class CookingRecipe : CraftingRecipe
    {
        public string recipeID;
        public string recipeName;
        public List<CraftingMaterial> requiredMaterials;
        public FinishedFoodItem finishedFoodItem;
        public List<RawFoodMaterial> requiredRawFoodItems;
        public bool CanBeCookedFrom(InventoryItem[] content)
        {
            foreach (var rawFoodItem in requiredRawFoodItems)
                if (!Array.Exists(content, item => item.ItemID == rawFoodItem.item.ItemID))
                    return false;

            return true;
        }
    }
}
