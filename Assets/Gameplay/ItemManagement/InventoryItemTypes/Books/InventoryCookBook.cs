using System;
using Core.Events;
using Gameplay.Extensions.InventoryEngineExtensions.Craft;
using Gameplay.ItemsInteractions;
using MoreMountains.Feedbacks;
using UnityEngine;
using UnityEngine.Serialization;

namespace Gameplay.ItemManagement.InventoryItemTypes.Books
{
    public enum RecipeType
    {
        Cooking
    }


    [CreateAssetMenu(
        fileName = "InventoryRecipeBook", menuName = "Crafting/Books/InventoryRecipeBook", order = 1)]
    [Serializable]
    public class InventoryCookBook : InventoryBook
    {
        public string AuthorName = "Unknown";
        public RecipeType RecipeType;

        [FormerlySerializedAs("RecipesGroup")] public RecipeGroup recipesGroup;
        [FormerlySerializedAs("RecipeLearnedFeedback")]
        public MMFeedbacks recipeLearnedFeedback;

        [Tooltip("The message to send when the book is read.")]
        public string message = "CookbookRead";
        [Tooltip("The message value to send with the message (optional).")]
        public override bool Use(string playerID)
        {
            var recipeManager = FindObjectOfType<CraftingRecipeManager>();
            if (recipeManager == null) Debug.LogWarning("JournalPersistenceManager not found in the scene.");

            var hasLearnedNewRecipes = false;

            if (CraftingRecipeManager.IsCraftGroupLearned(recipesGroup.UniqueID))
            {
                Debug.Log("Already knew these recipes.");
                RecipeGroupEvent.Trigger(
                    "RecipesAlreadyKnown", RecipeGroupEventType.RecipeGroupAlreadyKnown, recipesGroup);
            }
            else
            {
                Debug.Log("Learning new recipes.");
                CraftingRecipeManager.SaveLearnedCraftGroup(recipesGroup.UniqueID, true);
                RecipeGroupEvent.Trigger(
                    "RecipesLearned", RecipeGroupEventType.RecipeGroupLearned, recipesGroup);

                // Play feedback for newly learned recipes
                recipeLearnedFeedback?.PlayFeedbacks();
            }


            return true;
        }
    }
}
