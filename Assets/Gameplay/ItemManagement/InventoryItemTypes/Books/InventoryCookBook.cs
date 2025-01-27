using System;
using System.Collections.Generic;
using Gameplay.ItemManagement.Cooking;
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
                // Trigger recipe learned event
                RecipeEvent.Trigger("RecipeLearned", RecipeEventType.RecipeLearned, recipe, null);

            // Play feedback for learning recipes
            RecipeLearnedFeedback?.PlayFeedbacks();

            // Display recipes on the RecipeDisplayer
            var recipeDisplayer = FindObjectOfType<RecipeDisplayer>();
            if (recipeDisplayer != null)
                recipeDisplayer.DisplayLearnedRecipes(CookingRecipes);
            else
                Debug.LogWarning("No RecipeDisplayer found in the scene.");

            return true;
        }
    }
}
