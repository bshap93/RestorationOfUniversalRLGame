﻿using System;
using System.Collections.Generic;
using Project.Gameplay.ItemManagement.InventoryTypes.Cooking;
using UnityEngine;

namespace Project.Gameplay.ItemManagement.InventoryItemTypes.Books
{
    public enum CookingType
    {
        BasicHumanCooking,
        SheoliteCooking,
        
        
    }
    [CreateAssetMenu(
        fileName = "InventoryCookBook", menuName = "Crafting/Books/InventoryCookBook", order = 1)]
    [Serializable]
    public class InventoryCookBook : InventoryBook
    {
        public string AuthorName = "Unknown";
        public int CookingSkillLevelNeeded = 1;
        public CookingType CookingType;
        
        public List<CookingRecipe> CookingRecipes;
        
    }
}
