using System;
using System.Collections.Generic;
using Gameplay.ItemManagement.InventoryItemTypes.Books;
using Gameplay.ItemManagement.InventoryTypes.Cooking;
using MoreMountains.Feedbacks;
using PixelCrushers;
using Project.Core.Events;
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

        [Tooltip("The message to send when the book is read.")]
        public string message = "CookbookRead";
        [Tooltip("The message value to send with the message (optional).")]
        public override bool Use(string playerID)
        {
            var journalManager = FindObjectOfType<JournalPersistenceManager>();
            if (journalManager == null)
            {
                Debug.LogWarning("JournalPersistenceManager not found in the scene.");
                return false;
            }

            var hasLearnedNewRecipes = false;

            foreach (var recipe in CookingRecipes)
                if (journalManager.JournalData.knownRecipes.Exists(r => r.recipeID == recipe.recipeID))
                {
                    Debug.Log($"Recipe {recipe.recipeName} is already known.");
                    // Optionally play feedback for already-known recipes
                    RecipeEvent.Trigger("RecipeAlreadyKnown", RecipeEventType.RecipeAlreadyKnown, recipe, null);
                }
                else
                {
                    Debug.Log($"Learning new recipe: {recipe.recipeName}");
                    journalManager.AddRecipeToJournal(recipe);
                    RecipeEvent.Trigger("RecipeLearned", RecipeEventType.RecipeLearned, recipe, null);
                    hasLearnedNewRecipes = true;
                }

            if (hasLearnedNewRecipes)
            {
                // Play feedback for newly learned recipes
                RecipeLearnedFeedback?.PlayFeedbacks();

                RecipeEvent.Trigger("NewRecipesLearned", RecipeEventType.NewRecipesLearned, null, null);

                MessageSystem.SendMessage(this, message, ItemID);
                Debug.Log($"Message sent: {message} with value: {ItemID}");
            }
            else
                // Play alternative feedback for no new recipes

            {
                RecipeEvent.Trigger("NoNewRecipes", RecipeEventType.NoNewRecipes, null, null);
            }

            return true;
        }
    }
}
