using System;
using System.Collections.Generic;
using System.Linq;
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
                if (!content.Contains(requiredRawFoodItem.item))
                    return false;

            return true;
        }
    }
}
