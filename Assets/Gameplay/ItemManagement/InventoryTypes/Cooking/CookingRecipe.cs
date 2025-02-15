﻿using System;
using System.Collections.Generic;
using System.Linq;
using MoreMountains.InventoryEngine;
using Project.Gameplay.Interactivity.CraftingStation;
using Project.Gameplay.ItemManagement.InventoryTypes.Materials;
using UnityEngine;

namespace Gameplay.ItemManagement.InventoryTypes.Cooking
{
    [CreateAssetMenu(fileName = "Crafting", menuName = "Crafting/CookingRecipe", order = 1)]
    [Serializable]
    public class CookingRecipe : CraftingRecipe
    {
        public string recipeID;
        public string recipeName;
        public string recipeDescription;
        public FinishedFoodItem finishedFoodItem;
        public Sprite recipeImage;
        public List<CraftingMaterial> requiredMaterials;
        public List<CraftingMaterial> requiredRawFoodItems;

        public bool CanBeCookedFrom(InventoryItem[] content)
        {
            foreach (var requiredRawFoodItem in requiredRawFoodItems)
                // Check if an item with the same ItemID exists in the content
                if (!content.Any(item => item != null && item.ItemID == requiredRawFoodItem.item.ItemID))
                    return false;

            return true;
        }
    }
}
