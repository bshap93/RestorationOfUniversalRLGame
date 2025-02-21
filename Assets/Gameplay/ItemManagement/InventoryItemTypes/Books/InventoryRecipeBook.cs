using System;
using Core.Events;
using Gameplay.Extensions.InventoryEngineExtensions.Craft;
using Gameplay.ItemsInteractions;
using MoreMountains.Feedbacks;
using UnityEngine;
using UnityEngine.Serialization;

namespace Gameplay.ItemManagement.InventoryItemTypes.Books
{
    [CreateAssetMenu(
        fileName = "InventoryRecipeBook", menuName = "Crafting/Books/InventoryRecipeBook", order = 1)]
    [Serializable]
    public class InventoryRecipeBook : InventoryBook
    {
        [FormerlySerializedAs("RecipeType")] public RecipeType recipeType;

        [FormerlySerializedAs("recipesGroup")] [FormerlySerializedAs("RecipesGroup")]
        public RecipeGroup recipesGroupLearned;
        [FormerlySerializedAs("recipeLearnedFeedback")] [FormerlySerializedAs("RecipeLearnedFeedback")]
        public MMFeedbacks recipesLearnedFeedback;

        [Tooltip("The message to send when the book is read.")]
        public string message = "CookbookRead";
        [Tooltip("The message value to send with the message (optional).")]
        public override bool Use(string playerID)
        {
            var recipeManager = FindFirstObjectByType<CraftingRecipeManager>();
            if (recipeManager == null) Debug.LogWarning("JournalPersistenceManager not found in the scene.");

            if (CraftingRecipeManager.IsCraftGroupLearned(recipesGroupLearned.UniqueID))
            {
                Debug.Log("Already knew these recipes.");
                RecipeGroupEvent.Trigger(
                    RecipeGroupEventType.RecipeGroupAlreadyKnown, recipesGroupLearned.UniqueID);
            }
            else
            {
                Debug.Log("Learning new recipes.");

                CraftingRecipeManager.SaveLearnedCraftGroup(recipesGroupLearned.UniqueID, true);
                RecipeGroupEvent.Trigger(
                    RecipeGroupEventType.RecipeGroupLearned, recipesGroupLearned.UniqueID);

                // Play feedback for newly learned recipes
                recipesLearnedFeedback?.PlayFeedbacks();
            }


            return true;
        }
    }
}
