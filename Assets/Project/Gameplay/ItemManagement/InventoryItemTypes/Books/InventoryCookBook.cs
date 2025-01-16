using System;
using System.Collections.Generic;
using MoreMountains.Feedbacks;
using Project.Core.Events;
using Project.Gameplay.ItemManagement.InventoryTypes.Cooking;
using UnityEngine;

namespace Project.Gameplay.ItemManagement.InventoryItemTypes.Books
{
    public enum CookingType
    {
        BasicHumanCooking,
        SheoliteCooking
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
        public MMFeedbacks RecipeLearnedFeedback;


        public override bool Use(string playerID)
        {
            foreach (var recipe in CookingRecipes)
                RecipeEvent.Trigger("RecipeLearned", RecipeEventType.RecipeLearned, recipe, null);


            RecipeLearnedFeedback?.PlayFeedbacks();


            return true;
        }
    }
}
